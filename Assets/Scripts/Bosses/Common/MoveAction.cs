using System.Collections;
using UnityEngine;

namespace Bosses.Common
{
    public class MoveAction : BossAction
    {
        [SerializeField] 
        private float stoppingDist = 3;
        
        [SerializeField] 
        private AnimationClip moveAnim;
        
        [SerializeField]
        private float timeBeforeAction = 1f;
        [SerializeField]
        private float speed;
        
        protected override IEnumerator StartActionRoutine()
        {
            if(timeBeforeAction > 0)
                yield return new WaitForSeconds(timeBeforeAction);
            
            var target = BossTarget.Target;
            var targetPos = Vector3.zero;
            float dist = Vector3.Distance(transform.position, target.transform.position) - stoppingDist;
            
            if (target)
            {
                targetPos = transform.position + (target.transform.position - transform.position).normalized * dist;
                targetPos.y = transform.position.y;
            }
            
            if(dist <= 0.01f)
                yield break;
            
            float dur = dist / speed;
            float step = 0;

            var currPos = transform.position;
            PlayAnimationClip(moveAnim);
            GetComponent<BossBase>().LookAtEnemy();
            while(step <= 1)
            {
                step += Time.deltaTime / dur;
                dist = Vector3.Distance(transform.position, target.transform.position) - stoppingDist;
                if (dist <= stoppingDist)
                {
                    yield return new WaitForEndOfFrame();
                    break;
                }
                transform.position = Vector3.Lerp(currPos, targetPos, step);
                yield return new WaitForEndOfFrame();
            }
            PlayIdleAnimation();
        }
    }
}