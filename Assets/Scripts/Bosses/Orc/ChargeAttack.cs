using System.Collections;
using Bosses.Common;
using UnityEngine;

namespace Bosses
{
    public class ChargeAttack : BossAction
    {
        [SerializeField] private int damage = 15;
        [SerializeField] private AnimationClip windUpAnim;
        [SerializeField] private AnimationClip attackAnim;
        
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private GameObject damageDealer;
        
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(windUpAnim);
            yield return new WaitForSeconds(windUpAnim.length);
            PlayAnimationClip(attackAnim);
            Vector3 targetPos = BossTarget.Target.transform.position;
            Vector3 startPos = transform.position;
            targetPos.y = startPos.y;

            var dmgDealer = Instantiate(damageDealer, transform.position, Quaternion.identity, transform)
                .GetComponent<BossDamageDealer>();
            
            dmgDealer.DealDamage(damage);
            float timeStep = 0;
            var dist = Vector3.Distance(targetPos, startPos);
            var dir = (targetPos - startPos).normalized;
            var endPos = startPos + (dir * (dist < 3 ? 3 : dist));
            var dur = (dist < 3 ? 3 : dist) / speed;
            GetComponent<BossBase>().LookAtEnemy();
            while (timeStep <= 1)
            {
                timeStep += Time.deltaTime / dur;
                transform.position = Vector3.Lerp(startPos, endPos, timeStep);
                yield return new WaitForEndOfFrame();
            }
            
            yield return new WaitForEndOfFrame();
            Destroy(dmgDealer.gameObject);
            PlayIdleAnimation();
        }
    }
}