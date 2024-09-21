using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Bosses
{
    public class AudioManager
    {
        private static AudioSource bgSource;

        private static AudioMixer mixer;
        private static AudioMixer Mixer
        {
            get
            {
                if (mixer == null)
                    mixer = Resources.Load<AudioMixer>("Mixer");

                return mixer;
            }
        }
        
        public static void PlayBackgroundMusic(AudioClip clip)
        {
            if (bgSource != null)
                Object.Destroy(bgSource.gameObject);
            
            bgSource = new GameObject("BG Music").AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(bgSource.gameObject);

            bgSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master/BG")[0];
            
            bgSource.loop = true;
            bgSource.clip = clip;
            bgSource.Play();
        }
        
        public static void PlaySFX(AudioClip clip, bool louderSFX=false)
        {
            if (clip == null)
                return;
            
            var sfx = new GameObject("SFX").AddComponent<AudioSource>();
            if (!louderSFX)
                sfx.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master/SFX")[0];
            else
            {
                sfx.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master/SFX2")[0];
            }
            
            Object.Destroy(sfx.gameObject, clip.length);
            sfx.PlayOneShot(clip);
        }
    }
}