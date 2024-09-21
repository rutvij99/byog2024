using System.Collections;
using Bosses.Common;
using UnityEngine;

namespace Bosses
{
    public class UndergroundAttack : BossAction
    {
        [SerializeField] private int damage = 30;
        
        [SerializeField] private AnimationClip diveInClip;
        [SerializeField] private AnimationClip diveOutClip;
        [SerializeField] private GameObject diveOutFx;

        [SerializeField] private GameObject attackIndicator;
        [SerializeField] private GameObject damagerPrefab;
        
        [SerializeField] private float indicatorDur = 1.5f;
        [SerializeField] private float durationBeforePlungingOut = 3.0f;


        protected override IEnumerator StartActionRoutine()
        {
            PlayAnimationClip(diveInClip);
            yield return new WaitForSeconds(diveInClip.length);

            yield return new WaitForSeconds(durationBeforePlungingOut);
            transform.position = new Vector3(BossTarget.Target.transform.position.x, transform.position.y, BossTarget.Target.transform.position.z);

            var go = Instantiate(attackIndicator, transform.position, attackIndicator.transform.rotation);
            yield return new WaitForSeconds(indicatorDur);
            Destroy(go);
            yield return new WaitForEndOfFrame();
            var fxGo = Instantiate(diveOutFx, transform.position, diveOutFx.transform.rotation);
            Destroy(go, 3f);
            PlayAnimationClip(diveOutClip);
            PlaySFX("splash");
            var damager = Instantiate(damagerPrefab, transform.position, Quaternion.identity).GetComponent<BossDamageDealer>();
            yield return new WaitForSeconds(diveOutClip.length / 4f);
            damager.DealDamage(damage);
            yield return new WaitForSeconds(diveOutClip.length / 4f);
            Destroy(damager.gameObject);
            yield return new WaitForSeconds(diveOutClip.length / 2f);
            PlayIdleAnimation();
        }
    }
}