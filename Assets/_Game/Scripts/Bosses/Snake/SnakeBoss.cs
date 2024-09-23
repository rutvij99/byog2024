using System.Collections;
using Bosses.Upgrade;
using UnityEngine;

namespace Bosses
{
    public class SnakeBoss : BossBase
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
            if (entity.CurrHealth <= 0)
            {
                UpgradeStats.UnlockLightening();
            }
            
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
            float rng = Random.Range(0, 1f);
            var closedRangeActions = bossActions[actionStage].CloseRange;
            var longRangedActions = bossActions[actionStage].LongRange;
            // favour closed range attacks 
            if (dist <= MaxBossClosedRangeDist)
            {
                if (rng <= 0.35f)
                {
                    return availableBossActions[Random.Range(0, availableBossActions.Count)];
                }
                else
                {
                    return closedRangeActions[Random.Range(0, closedRangeActions.Count)];
                }
            }
            return longRangedActions[Random.Range(0, longRangedActions.Count)];
        }
    }
}