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
        [SerializeField] private GameObject chargeFx;
        [SerializeField] private Vector3 chargeFxOffset = Vector3.zero;
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(windUpAnim);
            GameObject chargo=null;
            
            if (chargeFx)
            {
                chargo = Instantiate(chargeFx, transform.position + chargeFxOffset, Quaternion.identity);
                chargo.transform.parent = transform;
            }
            
            yield return new WaitForSeconds(windUpAnim.length);
            PlayAnimationClip(attackAnim);
            Vector3 targetPos = BossTarget.Target.transform.position;
            Vector3 startPos = transform.position;
            targetPos.y = startPos.y;
            
            var dmgDealer = Instantiate(damageDealer, transform.position, Quaternion.identity, transform)
                .GetComponent<BossDamageDealer>();
            
            dmgDealer.DealDamage(damage);
            float timeStep = 0;
            var dist = Vector3.Distance(targetPos, startPos) + 1f;
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
            if(chargo)
                Destroy(chargo);
            Destroy(dmgDealer.gameObject);
            PlayIdleAnimation();
        }
    }
}