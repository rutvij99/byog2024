using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Bosses.Common;
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
        [SerializeField] public AudioMapDataAsset SFXClips;
        
        [SerializeField] private bool autoActivate = true;
        
        [SerializeField] private AudioClip bossMusicClip;
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip deathClip;
        [SerializeField] private GameObject deathCam;
        
        [SerializeField] private float initialWait;
        
        [SerializeField] private float afterAttackWait;
        [SerializeField] private float afterAttackWaitDeviation;

        [SerializeField] private Vector3 soulOffset = Vector3.one;
        [SerializeField] private GameObject soul;
        
        private BossTarget enemyTarget;

        public AnimationClip IdleClip => idleClip;
        private Animator animator;

        protected List<BossAction> availableBossActions;
        protected SerializedDictionary<ActionStage,ActionSet> bossActions;
        
        protected BossAction currAction;
        protected Coroutine currentActiveActionRoutine;
        protected Entity.Entity entity;

        protected float MaxBossClosedRangeDist { get; private set; }
        protected virtual void Start()
        {
            entity = GetComponent<Entity.Entity>();
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

            GetComponent<Entity.Entity>().OnEntityDamaged += OnHealthDeplete;
            animator = GetComponentInChildren<Animator>();
            PlayIdleAnimation();
            
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
        
        protected void NextAttack()
        {
            if(currentActiveActionRoutine != null)
                StopCoroutine(currentActiveActionRoutine);
                
            currentActiveActionRoutine = StartCoroutine(NextActionRoutine());
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
            if (currAction != null)
            {
                currAction.OnComplete += NextAttack;
                currAction.DoAction();
            }
        }

        protected virtual BossAction ChooseAction()
        {
            // default behaviour, choose random ones.
            return availableBossActions[Random.Range(0, availableBossActions.Count)];
        }
        
        protected void PlayAnimationClip(AnimationClip clip, float speed=1)
        {
            animator.speed = speed;
            foreach (var animeEventvent in clip.events)
            {
                animeEventvent.objectReferenceParameter = this;
            }
            animator.CrossFade(clip.name, normalizedTransitionDuration:0.15f);
        }

        protected void PlayIdleAnimation()
        {
            PlayAnimationClip(idleClip);
        }

        private void OnHealthDeplete()
        {
            if (entity.CurrHealth > 0)
                return ;
            StartCoroutine(DeathRoutine());
            
            if(deathCam)
                StartCoroutine(DeathCamRoutine());
        }

        IEnumerator DeathRoutine()
        {
            if(currentActiveActionRoutine != null)
                StopCoroutine(currentActiveActionRoutine);

            entity.OnEntityDamaged -= OnHealthDeplete;
            if(currAction)
                currAction.OnComplete -= NextAttack;

            for (int i = 0; i < availableBossActions.Count; i++)
            {
                Destroy(availableBossActions[i]);
            }

            foreach (var dealer in GetComponentsInChildren<BossDamageDealer>())
            {
                dealer.enabled = false;
            }
            
            currentActiveActionRoutine = null;
            PlayAnimationClip(deathClip);
            yield return new WaitForSeconds(deathClip.length);
            Destroy(this.gameObject, 3f);
        }
        
        IEnumerator DeathCamRoutine()
        {
            deathCam.SetActive(true);
            yield return new WaitForSecondsRealtime(0.35f);
            
            if(soul)
                Instantiate(soul, transform.position + soulOffset, Quaternion.identity);
            
            Time.timeScale = 0.1f;
            yield return new WaitForSecondsRealtime(2);
            deathCam.SetActive(false);
            Time.timeScale = 1;
        }
        
        public void PlaySFX(string key)
        {
            AudioManager.PlaySFX(SFXClips.GetClip(key));
        }
    }
}