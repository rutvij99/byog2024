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

        public void TakeDamage(int dmg)
        {
            CurrHealth -= dmg;
            healthbar.UpdateHealth((float)CurrHealth/DataAsset.MaxHealth);
            OnEntityDamaged?.Invoke();
            if (CurrHealth <= 0)
            {
                OnEntityDeath?.Invoke();
            }
        }
    }
}

