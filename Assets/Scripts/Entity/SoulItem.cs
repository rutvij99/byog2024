using System;
using System.Collections;
using Bosses;
using Bosses.Upgrade;
using UnityEngine;

namespace Entity
{
    public class SoulItem : MonoBehaviour
    {
        [SerializeField] private AudioClip rewardClip;
        [SerializeField] private string nextLevel;
        [SerializeField] private int souls=1;
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
                Destroy(go.gameObject);
                // do something here
                UpgradeStats.AddSouls(souls);
                AudioManager.PlaySFX(rewardClip);
                StartCoroutine(Kill());
            }
        }

        IEnumerator Kill()
        {
            transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(3);
            SceneLoader.Request(nextLevel);
            Destroy(this.gameObject);
        }
    }
}