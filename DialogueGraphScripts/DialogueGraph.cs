using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
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

        var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node"); });
        nodeCreateButton.text = "Create Node";
        toolBar.Add(nodeCreateButton);

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
