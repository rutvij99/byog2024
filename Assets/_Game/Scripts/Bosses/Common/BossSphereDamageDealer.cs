using System;
using UnityEngine;

namespace Bosses.Common
{
    [RequireComponent(typeof(SphereCollider))]
    public class BossSphereDamageDealer : BossDamageDealer
    {
        [SerializeField] private float range = 5;

        private void OnValidate()
        {
            GetComponent<SphereCollider>().radius = range;
        }

        private void OnDrawGizmos()
        {
            var color = new Color(1, 0, 0, 0.35f);
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, range);
        }
    }
}