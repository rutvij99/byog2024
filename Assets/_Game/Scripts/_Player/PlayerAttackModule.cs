using System;
using System.Collections.Generic;
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
		var target = other.transform.GetComponentInParent<PlayerTarget>();
		if (target != null && !allColliders.Contains(target))
		{
			allColliders.Add(target);
			Debug.Log($"Player attacked -> {other.gameObject.name}");
			target.TakeDamage(currentAttackInfo.damage);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		
	}

	public void SpawnProjectile(GameObject projectileSpawnPoint)
	{
		if(playerStats.currentWeaponType != WeaponType.Wand) return;
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
        proj.Launch(direction, (int)currentAttackInfo.damage);
	}
}
