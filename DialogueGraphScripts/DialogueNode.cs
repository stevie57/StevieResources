using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueNode : Node
{
    public string GUID;
    public string DialogueTitle;
    public bool EntryPoint = false;
    public PlayableAsset TimelineAsset;
    public ObjectField TimelineObjectField;
}
