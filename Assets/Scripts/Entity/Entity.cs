using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] 
        private EntityDataAsset DataAsset;

        [SerializeField] private GameObject HealthGUIPrefab;
        
        public int CurrHealth { get; private set; }

        public System.Action OnEntityDeath;
        public System.Action OnEntityDamaged;

        private Healthbar healthbar;

        [SerializeField] private bool testDamage;
        
        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            
            if(DataAsset)
                CurrHealth = DataAsset.MaxHealth;

            if (HealthGUIPrefab)
            {
                healthbar = Instantiate(HealthGUIPrefab).GetComponent<Healthbar>();
                healthbar.UpdateHealth((float)CurrHealth/DataAsset.MaxHealth);
                healthbar.SetTitle(DataAsset.EntityName);
            }
        }

        private void Update()
        {
            if (testDamage)
            {
                testDamage = false;
                TakeDamage(10);
            }
        }

        public void TakeDamage(int dmg)
        {
            CurrHealth -= dmg;
            if (CurrHealth <= 0)
                CurrHealth = 0;
            
            healthbar?.UpdateHealth((float)CurrHealth/DataAsset.MaxHealth);
            OnEntityDamaged?.Invoke();
            if (CurrHealth <= 0)
            {
                OnEntityDeath?.Invoke();
            }
        }
    }
}

