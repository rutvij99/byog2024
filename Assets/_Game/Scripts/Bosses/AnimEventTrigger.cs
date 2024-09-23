using UnityEngine;

namespace Bosses
{
    public class AnimEventTrigger : MonoBehaviour
    {
        public void PlaySFX(string key)
        {
            AudioManager.PlaySFX(GetComponentInParent<BossBase>().SFXClips.GetClip(key));
        }
    }
}