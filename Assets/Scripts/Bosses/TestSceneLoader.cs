using UnityEngine;

namespace Bosses
{
    public class TestSceneLoader : MonoBehaviour
    {
        public void LoadScene(string name)
        {
            SceneLoader.Request(name);
        }
    }
}