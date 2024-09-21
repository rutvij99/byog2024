using UnityEngine;

namespace Bosses.Common
{
    [RequireComponent(typeof(BoxCollider))]
    public class BossBoxDamageDealer : BossDamageDealer
    {
        [SerializeField] private BoxCollider coll;

        private void OnValidate()
        {
            coll = GetComponent<BoxCollider>();
        }
        
        public override void DealDamage(int damage)
        {
            base.DealDamage(damage);
            var all = Physics.OverlapBox(coll.bounds.center, coll.bounds.extents, transform.rotation);
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
            if (coll == null)
                coll = GetComponent<BoxCollider>();
            
            var color = new Color(1, 0, 0, 0.35f);
            Gizmos.color = color;
            Vector3 center = transform.TransformPoint(coll.center);
            Vector3 size = coll.size;
            Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, transform.localScale);
            Gizmos.DrawCube(Vector3.zero,size);
        }
    }
}