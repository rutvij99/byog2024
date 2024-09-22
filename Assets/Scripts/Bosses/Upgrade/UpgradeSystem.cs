using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Bosses.Upgrade
{
    [Serializable]
    public enum UpgradeType{
        Physical,
        Electrical, 
        Fire
    }
    
    public class UpgradeSystem : MonoBehaviour
    {
        public Action UpgradeDone;

        [SerializeField] private string nextLevel;
        [SerializeField] private TMP_Text soulsTxt;

        private void Start()
        {
            soulsTxt.text = UpgradeStats.GetSouls().ToString();
        }

        public void Upgrade(UpgradeType upgradeType)
        {
            UpgradeStats.Upgrade(upgradeType);
            UpgradeStats.AddSouls(-1);
            soulsTxt.text = UpgradeStats.GetSouls().ToString();
            UpgradeDone?.Invoke();

            if (GetSouls() <= 0)
            {
                StartCoroutine(Next());
            }
        }

        IEnumerator Next()
        {
            yield return new WaitForSeconds(2f);
            SceneLoader.Request(nextLevel);
        }

        public int GetLevel(UpgradeType type)
        {
            return UpgradeStats.GetLevel(type);
        }
        
        public int GetSouls()
        {
            return UpgradeStats.GetSouls();
        }

        [NaughtyAttributes.Button("Flush")]
        public void Flush()
        {
            UpgradeStats.FlushPrefs();
            soulsTxt.text = UpgradeStats.GetSouls().ToString();
            UpgradeDone?.Invoke();
        }
        
        [NaughtyAttributes.Button("Reward")]
        public void Reward()
        {
            UpgradeStats.AddSouls(1);
            soulsTxt.text = UpgradeStats.GetSouls().ToString();
            UpgradeDone?.Invoke();
        }
    }
}