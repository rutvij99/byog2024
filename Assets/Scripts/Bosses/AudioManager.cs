using Unity.VisualScripting;
using UnityEngine;

namespace Bosses
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioSource bgSource;

        public static void PlayBackgroundMusic(AudioClip clip)
        {
            if (bgSource != null)
                Destroy(bgSource.gameObject);
            
            bgSource = new GameObject("BG Music").AddComponent<AudioSource>();
            bgSource.loop = true;
            bgSource.clip = clip;
            bgSource.Play();
        }
    }
}