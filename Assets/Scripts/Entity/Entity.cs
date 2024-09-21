using UnityEngine;

namespace Entity
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] 
        private EntityDataAsset DataAsset;
        
        private int currHealth;

        public System.Action OnEntityDeath;
        public System.Action OnEntityDamaged;
        
        void Start()
        {
            if(DataAsset)
                currHealth = DataAsset.MaxHealth;
        }

        public void TakeDamage(int dmg)
        {
            currHealth -= dmg;
            OnEntityDamaged?.Invoke();
            if (currHealth <= 0)
            {
                OnEntityDeath?.Invoke();
            }
        }
    }
}

