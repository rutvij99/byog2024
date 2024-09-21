using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bosses.Common
{
    public class BossDamageDealer : MonoBehaviour
    {
        private BossTarget activeTarget;
        private int dmg;
        public virtual void DealDamage(int damage)
        {
            dmg = damage;
            Debug.Log("Dealing Damage");
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var bt = other.GetComponentInParent<BossTarget>();
            if ( bt != null)
            {
                if(activeTarget == bt)
                    return;

                activeTarget = bt;
                var entity = bt.GetComponent<Entity.Entity>();
                if(entity)
                    entity.TakeDamage(dmg);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var bt = other.GetComponentInParent<BossTarget>();
            if (bt != null)
            {
                if (activeTarget == bt) 
                    activeTarget = null;
            }
        }
    }
}