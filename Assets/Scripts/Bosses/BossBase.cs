using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bosses
{
    [RequireComponent(typeof(Entity.Entity))]
    [RequireComponent(typeof(Rigidbody))]
    public class BossBase : MonoBehaviour
    {
        [SerializeField] private bool autoActivate = true;
        
        [SerializeField] private AudioClip bossMusicClip;
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private float initialWait;
        
        [SerializeField] private float afterAttackWait;
        [SerializeField] private float afterAttackWaitDeviation;
        
        private BossTarget enemyTarget;

        public AnimationClip IdleClip => idleClip;
        private Animator animator;
        
        protected List<BossAction> availableBossActions;
        protected List<BossAction> longRangedActions;
        protected List<BossAction> closedRangeActions;
        protected BossAction currAction;
        
        protected float MaxBossClosedRangeDist { get; private set; }
        protected virtual void Start()
        {
            availableBossActions = GetComponents<BossAction>().ToList();
            availableBossActions.RemoveAll(a => !a.enabled);
            closedRangeActions = new List<BossAction>();
            longRangedActions = new List<BossAction>();
            MaxBossClosedRangeDist = 0;
            foreach (var action in availableBossActions)
            {
                if (action.IsCloseRanged)
                {
                    closedRangeActions.Add(action);
                    if (MaxBossClosedRangeDist < action.MaxDist)
                        MaxBossClosedRangeDist = action.MaxDist;
                }
                else
                    longRangedActions.Add(action);
            }
            animator = GetComponentInChildren<Animator>();
            animator.Play(idleClip.name, 0);
            
            if(autoActivate)
                ActivateBoss();
        }

        public void ActivateBoss()
        {
            AudioManager.PlayBackgroundMusic(bossMusicClip);
            StartCoroutine(BossActivation());
        }
        
        IEnumerator BossActivation()
        {
            yield return new WaitForSeconds(initialWait);
            LookAtEnemy();
            DecideAction();
        }
        
        private void NextAttack()
        {
            StartCoroutine(NextActionRoutine());
        }
        
        IEnumerator NextActionRoutine()
        {
            LookAtEnemy();
            yield return new WaitForSeconds(afterAttackWait +
                                            Random.Range(-afterAttackWaitDeviation, afterAttackWaitDeviation));
            DecideAction();
        }

        public void LookAtEnemy()
        {
            enemyTarget = BossTarget.Target;
            var enemyDir = enemyTarget.transform.position - transform.position;
            enemyDir.y = 0;
            transform.forward = enemyDir;
        }
        
        private void DecideAction()
        {
            if(currAction != null && currAction.State != ActionState.Completed)
                return;
            
            if (currAction != null)
                currAction.OnComplete -= NextAttack;
            
            LookAtEnemy();
            currAction = ChooseAction();
            currAction.OnComplete += NextAttack;
            currAction.DoAction();
        }

        protected virtual BossAction ChooseAction()
        {
            // default behaviour, choose random ones.
            return availableBossActions[Random.Range(0, availableBossActions.Count)];
        }
    }
}