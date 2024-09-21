using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bosses
{

    [System.Serializable]
    public class ActionSet
    {
        public List<BossAction> LongRange;
        public List<BossAction> CloseRange;
        public float MaxBossClosedRangeDist;
        public ActionSet()
        {
            MaxBossClosedRangeDist = 0;
            LongRange = new List<BossAction>();
            CloseRange = new List<BossAction>();
        }
    }
    
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
        protected SerializedDictionary<ActionStage,ActionSet> bossActions;
        
        protected BossAction currAction;
        
        protected float MaxBossClosedRangeDist { get; private set; }
        protected virtual void Start()
        {
            bossActions = new SerializedDictionary<ActionStage, ActionSet>();
            availableBossActions = GetComponents<BossAction>().ToList();
            availableBossActions.RemoveAll(a => !a.enabled);
            foreach (var action in availableBossActions)
            {
                
                if ((action.ValidStage & ActionStage.Stage1) != ActionStage.None)
                {
                    var stage = ActionStage.Stage1;
                    
                    if(!bossActions.ContainsKey(stage))
                        bossActions.Add(stage, new ActionSet());

                    if (action.IsCloseRanged)
                    {
                        bossActions[stage].CloseRange.Add(action);
                        if (bossActions[stage].MaxBossClosedRangeDist < action.MaxDist)
                            bossActions[stage].MaxBossClosedRangeDist = action.MaxDist;
                    }
                    else
                    {
                        bossActions[stage].LongRange.Add(action);
                    }
                }
                
                if ((action.ValidStage & ActionStage.Stage2) != ActionStage.None)
                {
                    var stage = ActionStage.Stage2;
                    
                    if(!bossActions.ContainsKey(stage))
                        bossActions.Add(stage, new ActionSet());

                    if (action.IsCloseRanged)
                    {
                        bossActions[stage].CloseRange.Add(action);
                        if (bossActions[stage].MaxBossClosedRangeDist < action.MaxDist)
                            bossActions[stage].MaxBossClosedRangeDist = action.MaxDist;
                    }
                    else
                    {
                        bossActions[stage].LongRange.Add(action);
                    }
                }
                    
                
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