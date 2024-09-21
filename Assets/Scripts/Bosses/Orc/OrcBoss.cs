using System.Collections;
using UnityEngine;

namespace Bosses
{
    public class OrcBoss : BossBase
    {
        [SerializeField] private AnimationClip stageChangeAnim;
        [SerializeField] private float stageChangeHealthVal=50;
        [SerializeField] private GameObject stageChangeCamera;
        
        private int stage = 1;

        private bool stageChangeQueued;
        
        protected override void Start()
        {
            base.Start();
            entity.OnEntityDamaged += OnDamaged;
        }

        private void OnDamaged()
        {
            if (entity.CurrHealth <= stageChangeHealthVal && stage == 1)
            {
                stage = 2;
                stageChangeQueued = true;
            }
        }
        
        protected override BossAction ChooseAction()
        {
            if (stageChangeQueued)
            {
                stageChangeQueued = false;
                LookAtEnemy();
                StartCoroutine(ChangeStage());
                return null;
            }
            return GetActions((ActionStage)stage);
        }

        IEnumerator ChangeStage()
        {
            yield return new WaitForSeconds(0.15f);
            stageChangeCamera?.SetActive(true);
            PlayAnimationClip(stageChangeAnim);
            yield return new WaitForSeconds(stageChangeAnim.length);
            stageChangeCamera?.SetActive(false);
            PlayIdleAnimation();
            NextAttack();
        }
        
        private BossAction GetActions(ActionStage actionStage)
        {
            var dist = Vector3.Distance(transform.position, BossTarget.Target.transform.position);
            var closedRangeActions = bossActions[actionStage].CloseRange;
            var longRangedActions = bossActions[actionStage].LongRange;
            if (dist <= bossActions[actionStage].MaxBossClosedRangeDist)
                return closedRangeActions[Random.Range(0, closedRangeActions.Count)];
            else 
                return longRangedActions[Random.Range(0, longRangedActions.Count)];
        }
    }
}