using UnityEngine;

namespace Bosses.Common
{
    public class BossSphereDamageDealer : BossDamageDealer
    {
        [SerializeField] private float range = 5;
        
        public virtual void DealDamage(int damage)
        {
            base.DealDamage(damage);
            var all = Physics.OverlapSphere(transform.position, range);
            foreach (Collider col in all)
            {
                var bt = col.GetComponentInParent<BossTarget>();
                if ( bt != null)
                {
                    var entity = bt.GetComponent<Entity.Entity>();
                    if(entity)
                        entity.TakeDamage(damage);
                }
            }
        }

        private void OnDrawGizmos()
        {
            var color = new Color(1, 0, 0, 0.35f);
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, range);
        }
    }
}