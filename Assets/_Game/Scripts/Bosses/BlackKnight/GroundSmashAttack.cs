using System.Collections;
using Bosses.Common;
using UnityEngine;

namespace Bosses
{
    public class GroundSmashAttack : BossAction
    {
        [SerializeField] private int damage = 15;
        [SerializeField] private AnimationClip windUpAnim;
        [SerializeField] private AnimationClip attackAnim;
        [SerializeField] private AnimationClip attackLoopAnim;
        
        [SerializeField] private float groundAttackDelay = 0.5f;
        [SerializeField] private GameObject damageDealer;
        [SerializeField] private GameObject areaIndicator;
        [SerializeField] private GameObject damageDealerArea;
        [SerializeField] private float intialDamageOffset;
        [SerializeField] private float areaDamageOffset;
        [SerializeField] private GameObject areaDamageFx;
        
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(windUpAnim);
            yield return new WaitForSeconds(windUpAnim.length);
            PlayIdleAnimation();
            yield return new WaitForSeconds(0.5f);
            PlayAnimationClip(attackAnim);
            yield return new WaitForSeconds(attackAnim.length/2);
            var damager1 = Instantiate(damageDealer, transform.position, Quaternion.identity).GetComponent<BossDamageDealer>();
            damager1.transform.parent = transform;
            damager1.transform.localPosition = Vector3.zero;
            damager1.transform.localRotation = Quaternion.identity;
            damager1.transform.position = transform.position + transform.forward * intialDamageOffset;
            damager1.DealDamage(damage);
            Destroy(damager1.gameObject, 0.15f);
            yield return new WaitForSeconds(attackAnim.length/2);
            
            PlayAnimationClip(attackLoopAnim);
            yield return new WaitForSeconds(0.15f);
            
            var indicator = Instantiate(areaIndicator, transform.position, Quaternion.identity);
            indicator.transform.parent = transform;
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localRotation = Quaternion.identity;
            indicator.transform.position = transform.position + transform.forward * areaDamageOffset;
            Destroy(indicator, 2.5f);
            
            yield return new WaitForSeconds(2.5f);
            
            var effect = Instantiate(areaDamageFx, transform.position,
                Quaternion.identity);
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
            effect.transform.position = transform.position + transform.forward * areaDamageOffset;
            Destroy(effect, 3);
            
            var damager2 = Instantiate(damageDealerArea, transform.position, Quaternion.identity).GetComponent<BossDamageDealer>();
            damager2.transform.parent = transform;
            damager2.transform.localPosition = Vector3.zero;
            damager2.transform.localRotation = Quaternion.identity;
            damager2.transform.position = transform.position + transform.forward * areaDamageOffset;
            damager2.DealDamage(damage);
            PlaySFX("ground_slam_explosion");
            yield return new WaitForSeconds(0.35f);
            PlayIdleAnimation();
            Destroy(damager2.gameObject);
            yield return new WaitForSeconds(1.15f);
        }
    }
}