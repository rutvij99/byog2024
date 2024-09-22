using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bosses.Cutscene
{
    public class Cutscene : MonoBehaviour
    {
        [SerializeField]
        private AudioClip click;
        
        [SerializeField] private GameObject body;
        [SerializeField] private TMPro.TMP_Text title;
        [SerializeField] private TMPro.TMP_Text msg;
        
        [SerializeField] private Animator speaker;
        [SerializeField] private CutsceneDataAsset asset;
        [SerializeField] private string nextLevel;

        private int id = -1;
        private bool done = false;
        
        private void Start()
        {
            NextDialog();
            AudioManager.PlayBackgroundMusic(asset.Music);
        }

        private void NextDialog()
        {
            if (id >= 0 && asset.Dialogs[id].TBC)
            {
                var go = GameObject.Find("TBC");
                if (go)
                {
                    body.SetActive(false);
                    var anim = go.GetComponent<Animator>();
                    if (!anim.enabled)
                    {
                        anim.enabled = true;
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                        return;
                    } 
                }
                
            }
            
            id++;
            if (id >= asset.Dialogs.Count)
            {
                done = true;
                LoadNextScene();
                return;
            }
            
            body.SetActive(true);
            title.text = asset.Dialogs[id].Title;
            msg.text = asset.Dialogs[id].Text;
            speaker.CrossFade(asset.Dialogs[id].mageAnim.name, 0.25f, 0);

            if (!string.IsNullOrEmpty(asset.Dialogs[id].camera))
            {
                var go = GameObject.Find(asset.Dialogs[id].camera);
                if(go)
                    go.GetComponent<CinemachineCamera>().enabled = true;
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !done)
            {
                AudioManager.PlaySFX(click);
                NextDialog();
            }
        }

        private void LoadNextScene()
        {
            SceneLoader.Request(nextLevel);
        }
    }
    
    
}