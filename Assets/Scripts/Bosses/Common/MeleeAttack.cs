using System.Collections;
using UnityEngine;

namespace Bosses.Common
{
    public class MeleeAttack : BossAction
    {
        [SerializeField] private int damage;
        [SerializeField] private AnimationClip attackClip;
        [SerializeField] private float offsetMult;
        [SerializeField] private GameObject damagerPrefab;
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(attackClip);
            yield return new WaitForSeconds(attackClip.length / 4);
            var damager = Instantiate(damagerPrefab, transform.position, Quaternion.identity).GetComponent<BossDamageDealer>();
            damager.transform.parent = transform;
            damager.transform.localPosition = Vector3.zero;
            damager.transform.localRotation = Quaternion.identity;
            damager.transform.position = transform.position + transform.forward * offsetMult;
            
            damager.DealDamage(damage);
            yield return new WaitForSeconds(attackClip.length  / 4);
            Destroy(damager.gameObject);
            yield return new WaitForSeconds(attackClip.length / 2);
            PlayIdleAnimation();
        }
    }
}