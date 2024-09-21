using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Bosses.Common
{
    public class MeleeAttack : BossAction
    {
        [SerializeField, Range(0.1f, 1.0f)] private float startDelay = 0.25f;
        [SerializeField] private int damage;
        [SerializeField] private AnimationClip attackClip;
        [SerializeField] private float offsetMult;
        [SerializeField] private GameObject damagerPrefab;
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(attackClip);
            float totalAnimTime = Mathf.Clamp01(attackClip.length * startDelay);
            yield return new WaitForSeconds(attackClip.length * startDelay);
            var damager = Instantiate(damagerPrefab, transform.position, Quaternion.identity).GetComponent<BossDamageDealer>();
            damager.transform.parent = transform;
            damager.transform.localPosition = Vector3.zero;
            damager.transform.localRotation = Quaternion.identity;
            damager.transform.position = transform.position + transform.forward * offsetMult;
            
            damager.DealDamage(damage);
            totalAnimTime -=  Mathf.Clamp01(attackClip.length * startDelay);
            yield return new WaitForSeconds(attackClip.length * startDelay);
            Destroy(damager.gameObject);
            yield return new WaitForSeconds(totalAnimTime);
            PlayIdleAnimation();
        }
    }
}