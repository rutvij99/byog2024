using Unity.VisualScripting;
using UnityEngine;

namespace Bosses
{
    public class AudioManager
    {
        private static AudioSource bgSource;

        public static void PlayBackgroundMusic(AudioClip clip)
        {
            if (bgSource != null)
                Object.Destroy(bgSource.gameObject);
            
            bgSource = new GameObject("BG Music").AddComponent<AudioSource>();
            bgSource.loop = true;
            bgSource.clip = clip;
            bgSource.Play();
        }
        
        public static void PlaySFX(AudioClip clip)
        {
            if (clip == null)
                return;
            
            var sfx = new GameObject("SFX").AddComponent<AudioSource>();
            Object.Destroy(sfx.gameObject, clip.length);
            sfx.PlayOneShot(clip);
        }
    }
}