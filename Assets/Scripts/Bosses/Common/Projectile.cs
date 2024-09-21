using System;
using System.Collections;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Bosses.Common
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject damager;
        [SerializeField] private GameObject destroyFX;
        
        private bool launched = false;
        private float speed = 0;
        private int damage=0;
        
        public void Launch(Vector3 dir, float speed, int damageToDeal)
        {
            this.speed = speed;
            this.damage = damageToDeal;
            transform.forward = dir;
            var rb = GetComponent<Rigidbody>();
            rb.linearVelocity = dir * this.speed;
        }

        IEnumerator KillMe()
        {
            yield return new WaitForSeconds(10);
            TakeDamage();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponentInParent<BossBase>())
                return;
            
            TakeDamage();
        }

        private void TakeDamage()
        {
            var go = Instantiate(damager, transform.position, transform.rotation).GetComponent<BossDamageDealer>();
            go.DealDamage(damage);
            Destroy(go.gameObject, 0.15f);
            if (destroyFX)
            {
                var fx = Instantiate(destroyFX, transform.position, Quaternion.identity);
                Destroy(fx.gameObject, 2);
            }
            Destroy(this.gameObject);
        }
    }
}