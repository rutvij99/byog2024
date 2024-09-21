using System.Collections;
using UnityEngine;

namespace Bosses
{
    public enum ActionState
    {
        None,
        OnGoing,
        Completed
    }
    
    public abstract class BossAction : MonoBehaviour
    {
        public ActionState State { get; private set; }
        public System.Action OnComplete;

        [SerializeField] private bool closeRange;
        [SerializeField] private float maxDist = 999;

        public bool IsCloseRanged => closeRange;
        public float MaxDist => maxDist;
        
        
        private Animator Animator { get; set; }
        private AnimationClip idleAnim;
        
        public void DoAction()
        {
            idleAnim = GetComponent<BossBase>().IdleClip;
            Animator = GetComponentInChildren<Animator>();
            StartCoroutine(ActionRoutine());
        }

        IEnumerator ActionRoutine()
        {
            State = ActionState.OnGoing;
            yield return StartCoroutine(StartActionRoutine());
            State = ActionState.Completed;
            OnComplete?.Invoke();
        }

        protected abstract IEnumerator StartActionRoutine();

        protected void PlayAnimationClip(AnimationClip clip, float speed=1)
        {
            Animator.speed = speed;
            Animator.CrossFade(clip.name, normalizedTransitionDuration:0.15f);
        }

        protected void PlayIdleAnimation()
        {
            PlayAnimationClip(idleAnim);
        }
    }
}