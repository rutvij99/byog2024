using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Bosses.Upgrade
{
    public class Revealer : MonoBehaviour
    {
        [SerializeField] private AudioClip revealSFX;
        [SerializeField] private CanvasGroup cg;

        private bool start = false;
        
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1.5f);
            AudioManager.PlaySFX(revealSFX);
            cg = GetComponent<CanvasGroup>();
            start = true;
        }

        private void Update()
        {
            if(!start)
                return;
            
            cg.alpha -= Time.deltaTime;
            if(cg.alpha <= 0)
                Destroy(this.gameObject);
        }
    }
}