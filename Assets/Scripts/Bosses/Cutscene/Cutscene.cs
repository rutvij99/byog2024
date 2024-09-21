using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bosses.Cutscene
{

    [Serializable]
    public class Dialog
    {
        public string Title;
        public string Text;
        public AnimationClip mageAnim;
    }
    
    [CreateAssetMenu(fileName = "CutsceneAsset", menuName = "IRIS/Cutscene")]
    public class CutsceneDataAsset : ScriptableObject
    {
        public List<Dialog> Dialogs;
    }
    
    public class Cutscene : MonoBehaviour
    {
        [SerializeField] private GameObject body;
        [SerializeField] private TMPro.TMP_Text title;
        [SerializeField] private TMPro.TMP_Text msg;
        
        [SerializeField] private Animator speaker;
        [SerializeField] private CutsceneDataAsset asset;
        [SerializeField] private string nextLevel;

        private int id = 0;
        
        private void Start()
        {
            NextDialog();
        }

        private void NextDialog()
        {
            id++;
            if (id >= asset.Dialogs.Count)
            {
                LoadNextScene();
                return;
            }
            
            body.SetActive(true);
            title.text = asset.Dialogs[id].Title;
            msg.text = asset.Dialogs[id].Text;
            speaker.CrossFade(asset.Dialogs[id].mageAnim.name, 0.25f, 0);
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E))
                NextDialog();
        }

        private void LoadNextScene()
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
    
    
}