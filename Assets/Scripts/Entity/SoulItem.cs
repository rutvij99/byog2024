using System;
using System.Collections;
using Bosses;
using UnityEngine;

namespace Entity
{
    public class SoulItem : MonoBehaviour
    {
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private float chaseSpeed = 1.5f;
        [SerializeField] private GameObject fx;
        private bool moveToPlayer;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            moveToPlayer = true;
        }

        private void Update()
        {
            if(!moveToPlayer)
                return;

            transform.position += ((BossTarget.Target.transform.position + targetOffset) - transform.position).normalized * (chaseSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<BossTarget>())
            {
                var go = Instantiate(fx, transform.position, Quaternion.identity);
                
                // do something here
                
                Destroy(go, 2);
                Destroy(this.gameObject);
            }
        }
    }
}