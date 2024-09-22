using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bosses
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cg;

        public static void Request(string levelName)
        {
            var comp = FindAnyObjectByType<SceneLoader>();
            if (!comp)
            {
                comp = Instantiate(Resources.Load<GameObject>("SceneLoader")).GetComponent<SceneLoader>();
                DontDestroyOnLoad(comp.gameObject);
            }
            comp.LoadScene(levelName);
        }
        
        private void LoadScene(string levelName)
        {
            StartCoroutine(LoadScreenRoutine(levelName));
        }

        IEnumerator LoadScreenRoutine(string levelName)
        {
            float t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime / 0.25f;
                cg.alpha = Mathf.Lerp(0, 1, t);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene(levelName);
            yield return new WaitForEndOfFrame();
            t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime / 0.25f;
                cg.alpha = Mathf.Lerp(1, 0, t);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
            Destroy(this.gameObject);
        }
    }
}