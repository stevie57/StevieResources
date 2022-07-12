using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using System.IO;

public class GraphSaveUtility 
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        if (!SaveNodes(dialogueContainer)) return;
        SaveExposedProperties(dialogueContainer);

        // Check if file path exists. If it doesn't then create it.
        if (!AssetDatabase.IsValidFolder($"Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        if(_targetGraphView.BlackBoard != null)
            dialogueContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
    }

    private bool SaveNodes(DialogueContainer dialogueContainer)
    {
        // save edges
        if (!Edges.Any()) return false;
        var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
        for (var i = 0; i < connectedSockets.Length; i++)
        {
            var outputNode = (connectedSockets[i].output.node as DialogueNode);
            var inputNode = (connectedSockets[i].input.node as DialogueNode);
            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedSockets[i].output.portName,
                TargetNodeGUID = inputNode.GUID
            });
        }


        //Edge[] connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        //for (int i = 0; i < connectedPorts.Length; i++)
        //{
        //    var outputNode = connectedPorts[i].output.node as DialogueNode;
        //    var inputNode = connectedPorts[i].input.node as DialogueNode;

        //    dialogueContainer.NodeLinks.Add(new NodeLinkData()
        //    {
        //        BaseNodeGUID = outputNode.GUID,
        //        PortName = connectedPorts[i].output.name,
        //        TargetNodeGUID = inputNode.GUID,
        //    });
        //}

        foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            var newNode = new DialogueNodeData()
            {
                NodeGUID = dialogueNode.GUID,
                DialogueTitle = dialogueNode.DialogueTitle,
                Position = dialogueNode.GetPosition().position,
                SavedTimelineAssetName = dialogueNode.TimelineAsset.name
            };
            dialogueContainer.DialogueNodeData.Add(newNode);
        }      
        return true;
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File not Found", "Target Dialogue graph file does not exist", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        //CreateExposedProperties();
    }

    private void CreateExposedProperties()
    {
        if (_targetGraphView.BlackBoard == null) return;

        _targetGraphView.ClearBlackBoardandExposedProperty();

        foreach(var exposedProperty in _containerCache.ExposedProperties)
        {
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
    }

    private void ClearGraph()
    {
        // set entry points guid based on save. Discard exisiting guid
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUID;

        foreach(var node in Nodes)
        {
            if (node.EntryPoint) continue;

            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));
        }
    }

    [ExecuteInEditMode]
    private void CreateNodes()
    {
        foreach(var nodeData in _containerCache.DialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueTitle , nodeData.Position);
            tempNode.GUID = nodeData.NodeGUID;

            if(File.Exists(Application.dataPath + $"/Resources/Timelines/{nodeData.SavedTimelineAssetName}.playable"))
            {
                tempNode.TimelineAsset = Resources.Load<PlayableAsset>($"Timelines/{nodeData.SavedTimelineAssetName}");
                tempNode.TimelineObjectField.value = tempNode.TimelineAsset;
                _targetGraphView.AddElement(tempNode);
            }
            else
            {
                string path = Application.dataPath + $"/Resources/Timelines/{nodeData.SavedTimelineAssetName}.playable";
                Debug.Log($"timeline asset doesnt exist {path}");
            }

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            // all connections to Nodes[i]
            var currentNode = Nodes[i];
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == currentNode.GUID).ToList();
            Debug.Log($"Currnet Node Name is {currentNode.title} and I have {connections.Count} connection");

            if (connections.Count == 0) continue;
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                Port outputPort = currentNode.outputContainer[j].Q<Port>();
                Port inputPort = (Port)targetNode.inputContainer[0];
                if (outputPort != null)
                {
                    LinkNodes(outputPort, inputPort);
                }
                else
                {
                    Debug.Log($"I am unable to connect {currentNode.title} for connection {j}");
                    Debug.Log($"TargetnodeGuid is {targetNodeGuid}");
                    Debug.Log($"Target node is {targetNode}");                    
                }
                targetNode.SetPosition(new Rect(_containerCache.DialogueNodeData.First(x => x.NodeGUID == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);

        _targetGraphView.Add(tempEdge);
    }
}
