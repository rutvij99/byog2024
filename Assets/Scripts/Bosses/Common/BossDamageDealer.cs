using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bosses.Common
{
    public class BossDamageDealer : MonoBehaviour
    {
        [SerializeField] private AudioClip hitSFX;
        [SerializeField] private GameObject hitFX;
        
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
                if (entity)
                {
                    AudioManager.PlaySFX(hitSFX);
                    entity.TakeDamage(dmg);

                    var go = Instantiate(hitFX, other.transform.position + new Vector3(0,1,0), Quaternion.identity);
                    Destroy(go, 2);
                }
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