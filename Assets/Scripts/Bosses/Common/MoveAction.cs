using System.Collections;
using UnityEngine;

namespace Bosses.Common
{
    public class MoveAction : BossAction
    {
        [SerializeField] 
        private AnimationClip moveAnim;
        
        [SerializeField]
        private float timeBeforeAction = 1f;
        [SerializeField]
        private float speed;
        
        protected override IEnumerator StartActionRoutine()
        {
            yield return new WaitForSeconds(timeBeforeAction);
            var target = BossTarget.Target;
            var targetPos = Vector3.zero;
            if (target)
            {
                targetPos = target.transform.position;
            }

            float dist = Vector3.Distance(transform.position, targetPos);
            if(dist <= 0.01f)
                yield break;
            
            float dur = dist / speed;
            float step = 0;
            
            PlayAnimationClip(moveAnim);
            GetComponent<BossBase>().LookAtEnemy();
            while(step <= 1)
            {
                step += Time.deltaTime / dur;
                transform.position = Vector3.Lerp(transform.position, targetPos, step);
                yield return new WaitForEndOfFrame();
            }
            PlayIdleAnimation();
        }
    }
}