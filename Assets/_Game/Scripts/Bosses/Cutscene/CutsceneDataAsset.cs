using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bosses.Cutscene
{
    [Serializable]
    public class Dialog
    {
        public string Title;
        public string Text;
        public AnimationClip mageAnim;
        public string camera;
        public bool TBC;
    }
    
    [CreateAssetMenu(fileName = "CutsceneAssetData", menuName = "IRIS/Cutscene")]
    public class CutsceneDataAsset : ScriptableObject
    {
        public AudioClip Music;
        public List<Dialog> Dialogs;
    }
}