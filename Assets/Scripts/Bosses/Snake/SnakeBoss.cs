using UnityEngine;

namespace Bosses
{
    public class SnakeBoss : BossBase
    {
        [SerializeField] private float stageChangeHealthVal;
        
        private int stage = 1;
        
        protected override void Start()
        {
            base.Start();
            entity.OnEntityDamaged += OnDamaged;
        }

        private void OnDamaged()
        {
            if (entity.CurrHealth <= stageChangeHealthVal && stage == 1)
            {
                stage=2;
            }
        }
        
        protected override BossAction ChooseAction()
        {
            return GetActions((ActionStage)stage);
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