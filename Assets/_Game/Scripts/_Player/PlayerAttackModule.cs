using System;
using UnityEngine;

public class PlayerAttackModule : MonoBehaviour
{
	private PlayerStats playerStats;
	private PlayerController playerController;
	private AttackInfo currentAttackInfo;

	private void Start()
	{
		playerStats = GetComponent<PlayerStats>();
		playerController = GetComponent<PlayerController>();
	}

	public void SetAttackInfo(AttackInfo info)
	{
		currentAttackInfo = info;
	}
	private void OnTriggerEnter(Collider other)
	{
		var target = other.transform.GetComponentInParent<PlayerTarget>();
		if (target != null)
		{
			Debug.Log($"Player attacked -> {other.gameObject.name}");
			target.TakeDamage(currentAttackInfo.damage);
		}
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
        proj.Launch(direction, (int)currentAttackInfo.damage);
	}
}
