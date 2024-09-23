using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bosses.Upgrade
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text lvl;
        [SerializeField] private AudioSource activatingSFx;
        [SerializeField] private AudioClip upgradeClip;
        [SerializeField] private UpgradeType UpgradeType;
        [SerializeField] private Image fill;
        [SerializeField] private Animator shineAnimator;

        [SerializeField] private UpgradeSystem system;
        
        private Coroutine routine;

        private void Start()
        {
            system.UpgradeDone += RefreshData;
            RefreshData();
        }

        public void Activate()
        {
            if(routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(ActivateRoutine());
        }

        public void Leave()
        {
            if(routine != null)
                StopCoroutine(routine);
            activatingSFx.Stop();
            fill.fillAmount = 0;
        }

        private IEnumerator ActivateRoutine()
        {
            float t = 0;
            activatingSFx.Play();
            while (t <= 1)
            {
                t += Time.deltaTime / 1;
                fill.fillAmount = Mathf.Lerp(0, 1f, t);
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
            fill.fillAmount = 0;
            system.Upgrade(UpgradeType);
            shineAnimator.SetTrigger("shine");
            AudioManager.PlaySFX(upgradeClip);
        }

        private void RefreshData()
        {
            var val = system.GetLevel(UpgradeType);
            var souls = system.GetSouls();
            lvl.text = val.ToString();
            GetComponent<Button>().interactable = souls > 0;
            GetComponent<EventTrigger>().enabled = souls > 0;
        }
    }
}