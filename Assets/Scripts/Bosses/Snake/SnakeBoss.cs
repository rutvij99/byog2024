using UnityEngine;

namespace Bosses
{
    public class SnakeBoss : BossBase
    {
        protected override BossAction ChooseAction()
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