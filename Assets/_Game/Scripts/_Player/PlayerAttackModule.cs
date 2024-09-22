using System;
using System.Collections.Generic;
using Bosses.Upgrade;
using UnityEngine;

public class PlayerAttackModule : MonoBehaviour
{
	private PlayerStats playerStats;
	private PlayerController playerController;
	private AttackInfo currentAttackInfo;
	
	private List<PlayerTarget> allColliders = new List<PlayerTarget>();

	private void Start()
	{
		playerStats = GetComponent<PlayerStats>();
		playerController = GetComponent<PlayerController>();
	}

	public void SetAttackInfo(AttackInfo info)
	{
		currentAttackInfo = info;
		allColliders.Clear();
	}
	private void OnTriggerEnter(Collider other)
	{
		if(!playerController.IsAttacking) return;
		if(currentAttackInfo == null) return;
		var target = other.transform.GetComponentInParent<PlayerTarget>();
		if (target != null && !allColliders.Contains(target))
		{
			allColliders.Add(target);
			Debug.Log($"Player attacked -> {other.gameObject.name}");
			playerController.PlayActualDamageSFX();
			target.TakeDamage(currentAttackInfo.damage * UpgradeStats.GetDamageMultiplier(UpgradeType.Physical));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		
	}

	public void PlayProjectileHitSFX()
	{
		playerController.PlayProjectileHitSFX();
	}

	public void SpawnProjectile(GameObject projectileSpawnPoint)
	{
		if(playerStats.currentWeaponType != WeaponType.Wand) return;
		playerController.PlayProjectileSFX();
        GameObject projectile = Instantiate(playerStats.currentWeaponData.projectilePrefab, projectileSpawnPoint.transform.position, Quaternion.identity);
        PlayerProjectile proj = projectile.GetComponent<PlayerProjectile>();
        Vector3 direction;
        if (playerController.currentTarget != null)
        {
            direction = (playerController.currentTarget.transform.position - this.transform.position).normalized;
        }
        else
        {
            direction = playerController.IsFacingRight ? Vector3.right : Vector3.left;
        }
        Destroy(projectileSpawnPoint);
        proj.Launch(this, direction, (int)currentAttackInfo.damage);
	}
}
