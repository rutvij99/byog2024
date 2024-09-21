using UnityEngine;

namespace Bosses
{
    public class SnakeBoss : BossBase
    {
        [SerializeField] private float stageChangeHealthVal;

        private Entity.Entity entity;
        private int stage = 1;
        
        protected override void Start()
        {
            base.Start();
            entity = GetComponent<Entity.Entity>();
            entity.OnEntityDamaged += OnDamaged;
        }

        private void OnDamaged()
        {
            if (entity.CurrHealth <= stageChangeHealthVal)
            {
                stage++;
            }
        }
        
        protected override BossAction ChooseAction()
        {
            if (stage == 1)
                return GetStage1Actions();
            else
            {
                return GetStage2Actions();
            }
        }

        private BossAction GetStage1Actions()
        {
            var dist = Vector3.Distance(transform.position, BossTarget.Target.transform.position);
            float rng = Random.Range(0, 1f);
            // favour closed range attacks 
            if (dist <= MaxBossClosedRangeDist)
            {
                if (rng <= 0.35f)
                {
                    Debug.Log("Random Range");
                    return availableBossActions[Random.Range(0, availableBossActions.Count)];
                }
                else
                {
                    Debug.Log("Close Range");
                    return closedRangeActions[Random.Range(0, closedRangeActions.Count)];
                }
            }
            
            Debug.Log("Long Range");
            return longRangedActions[Random.Range(0, longRangedActions.Count)];
        }
        
        private BossAction GetStage2Actions()
        {
            var dist = Vector3.Distance(transform.position, BossTarget.Target.transform.position);
            float rng = Random.Range(0, 1f);
            // favour closed range attacks 
            if (dist <= MaxBossClosedRangeDist)
            {
                if (rng <= 0.35f)
                {
                    Debug.Log("Random Range");
                    return availableBossActions[Random.Range(0, availableBossActions.Count)];
                }
                else
                {
                    Debug.Log("Close Range");
                    return closedRangeActions[Random.Range(0, closedRangeActions.Count)];
                }
            }
            
            Debug.Log("Long Range");
            return longRangedActions[Random.Range(0, longRangedActions.Count)];
        }
    }
}