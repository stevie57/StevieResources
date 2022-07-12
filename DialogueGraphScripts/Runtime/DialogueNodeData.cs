using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace DialogueSystem
{
    [System.Serializable]
    public class DialogueNodeData 
    {
        public string NodeGUID;
        public string DialogueTitle;
        public Vector2 Position;
        public string SavedTimelineAssetName;
    }
}