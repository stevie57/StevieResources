using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;


// https://www.youtube.com/watch?v=7KHGH0fPL84 @ 17:22
public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150f, 200f);

    public DialogueGraphView()
    {        
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph")); 
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if(startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }    
        });
        return compatiblePorts;
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var dialogueNode = new DialogueNode
        {
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "Entry point",
            EntryPoint = true
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Output);
        inputPort.portName = "Next";
        dialogueNode.outputContainer.Add(inputPort);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(100, 200, 100, 150));
        
        return dialogueNode;
    }

    private void AddChoicePort(DialogueNode dialogueNode)
    {
        var generatePort = GeneratePort(dialogueNode, Direction.Output);
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        generatePort.portName = $"Choice {outputPortCount}";
        dialogueNode.outputContainer.Add(generatePort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public void CreateNode(string nodeName) 
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    private DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode()
        {
            title = nodeName,
            name = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        var button = new Button(() => { AddChoicePort(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        return dialogueNode;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); // Arbitraty type
    }
}
