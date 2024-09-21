using System.Collections;
using Bosses.Common;
using UnityEngine;

namespace Bosses
{
    public class LightningRangedAttack : BossAction
    {
        [SerializeField] private int damage;
        [SerializeField] private AnimationClip startUpClip;
        [SerializeField] private AnimationClip attackClip;
        [SerializeField] private float offsetMult;
        [SerializeField] private GameObject damagerPrefab;

        [SerializeField] private GameObject lightningFx;
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(startUpClip);
            yield return new WaitForSeconds(startUpClip.length);
            PlayAnimationClip(attackClip);
            PlaySFX("lightning");
            yield return new WaitForSeconds(attackClip.length / 4);
            var damager = Instantiate(damagerPrefab, transform.position, Quaternion.identity).GetComponent<BossDamageDealer>();
            damager.transform.parent = transform;
            damager.transform.localPosition = Vector3.zero;
            damager.transform.localRotation = Quaternion.identity;
            damager.transform.position = transform.position + transform.forward * offsetMult;
            damager.DealDamage(damage);
            var fx = Instantiate(lightningFx, transform.position, Quaternion.identity);
            fx.transform.parent = transform;
            fx.transform.localPosition = Vector3.zero;
            fx.transform.localRotation = Quaternion.identity;
            fx.transform.position = transform.position + transform.forward * offsetMult;
            yield return new WaitForSeconds(3 * attackClip.length  / 4);
            Destroy(damager.gameObject);
            Destroy(fx.gameObject);
            yield return new WaitForSeconds(startUpClip.length);
            PlayIdleAnimation();
        }
    }
}