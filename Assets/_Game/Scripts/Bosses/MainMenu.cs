using System;
using UnityEngine;

namespace Bosses
{
    public class MainMenu : MonoBehaviour
    {

        [SerializeField] private AudioClip clickSFX;
        [SerializeField] private AudioClip bg;

        [SerializeField] private GameObject continueBttn;
        
        private void Start()
        {
            AudioManager.PlayBackgroundMusic(bg);
            continueBttn.SetActive(PlayerPrefs.HasKey("LastSavedScene"));
        }
        
        public void LoadScene(string name)
        {
            SceneLoader.Request(name);
        }
        
        public void StartNewGame()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            AudioManager.PlaySFX(clickSFX);
            AudioManager.PlayBackgroundMusic(null);
            SceneLoader.Request("Cutscene_1");
        }
        
        public void ContinueGame()
        {
            string currLevel = PlayerPrefs.GetString("LastSavedScene", "Cutscene_1");
            AudioManager.PlaySFX(clickSFX);
            AudioManager.PlayBackgroundMusic(null);
            SceneLoader.Request(currLevel);
        }
        
        public void QuitGame()
        {
            AudioManager.PlaySFX(clickSFX);
            Application.Quit();
        }
    }
}