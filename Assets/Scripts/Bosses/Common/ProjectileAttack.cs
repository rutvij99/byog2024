using System.Collections;
using UnityEngine;

namespace Bosses.Common
{
    public class ProjectileAttack : BossAction
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] private float launchDelay = 0.25f;
        
        [SerializeField] private int damage;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject projectileToShoot;
        [SerializeField] private AnimationClip animClip;
        [SerializeField] private float projectileSpeed=5;
        
        protected override IEnumerator StartActionRoutine()
        {
            GetComponent<BossBase>().LookAtEnemy();
            PlayAnimationClip(animClip);
            yield return new WaitForSeconds(animClip.length * launchDelay);
            var projectile = Instantiate(projectileToShoot, spawnPoint.position, Quaternion.identity).GetComponent<Projectile>();
            var dir = (BossTarget.Target.transform.position - spawnPoint.position).normalized;
            projectile.Launch(dir, projectileSpeed, damage);
            yield return new WaitForSeconds((1.0f - launchDelay) * animClip.length);
            PlayIdleAnimation();
        }
    }
}