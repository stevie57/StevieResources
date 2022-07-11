using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialgoueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
        GenerateMiniMap();
        //GenerateBlackBoard();
    }

    private void GenerateBlackBoard()
    {
        var blackBoard = new Blackboard(_graphView);
        blackBoard.Add(new BlackboardSection { title = "Exposed Properties" });
        
        blackBoard.addItemRequested = _blackboard =>
        {
            _graphView.AddPropertyToBlackboard(new ExposedProperty());
        };
        blackBoard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exist. Choose another one", "OK");
            }

            var propertyIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            _graphView.ExposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField)element).text = newValue; 
        };
        
        blackBoard.SetPosition(new Rect(10, 30, 200, 300));
        _graphView.Add(blackBoard);
        _graphView.BlackBoard = blackBoard;
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap { };
        miniMap.IsSelectable();
        miniMap.IsMovable();
        // 10 px offset from left side
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 2800, 30));
        Debug.Log($"maxsize x is {this.maxSize.x}");
        miniMap.SetPosition(new Rect(0, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolBar()
    {
        var toolBar = new Toolbar();

        var filenameTextField = new TextField("File Name");
        filenameTextField.SetValueWithoutNotify(_fileName);
        filenameTextField.MarkDirtyRepaint();
        filenameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolBar.Add(filenameTextField);

        toolBar.Add(new Button( () => RequestDataOperation(true)) { text = "SaveData" });
        toolBar.Add(new Button(() => RequestDataOperation(false)) { text = "LoadData" });

        //var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node"); });
        //nodeCreateButton.text = "Create Node";
        //toolBar.Add(nodeCreateButton);

        rootVisualElement.Add(toolBar);
    }

    private void RequestDataOperation(bool save)
    {
        if (!string.IsNullOrEmpty(_fileName))
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);

            if (save)
                saveUtility.SaveGraph(_fileName);
            else
                saveUtility.LoadGraph(_fileName);
        }
        else
        {
            EditorUtility.DisplayDialog("Invalid Filename!", "Please enter a valid file name.", "OK");
        }
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
