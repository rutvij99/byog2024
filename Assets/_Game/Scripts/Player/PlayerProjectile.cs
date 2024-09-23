using Bosses.Upgrade;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
	public float speed = 10f;
	public float lifetime = 5f;
	private Vector3 direction;
	private int damage;
	private PlayerAttackModule _attackModule;
	private Rigidbody _rigidbody;

	// This method will be called before Start or when this script is instantiated
	private void Awake()
	{
		// Get the Rigidbody component on the projectile
		_rigidbody = GetComponent<Rigidbody>();

		// Ensure the Rigidbody is not affected by gravity and only uses velocity for movement
		_rigidbody.useGravity = false;
	}

	public void Launch(PlayerAttackModule attackModule, Vector3 launchDirection, int projectileDamage)
	{
		_attackModule = attackModule;
		direction = launchDirection.normalized;  // Normalize the direction to ensure consistent speed
		damage = projectileDamage;

		// Set the velocity of the rigidbody to move the projectile
		_rigidbody.linearVelocity = direction * speed;

		// Destroy the projectile after its lifetime expires
		Destroy(gameObject, lifetime);
	}

	private void OnTriggerEnter(Collider other)
	{
		// Detect if the projectile hit a target
		var target = other.GetComponentInParent<PlayerTarget>();
		if (target != null)
		{
			if (_attackModule != null)
				_attackModule.PlayProjectileHitSFX();

			// Apply damage to the target
			target.TakeDamage(damage * UpgradeStats.GetDamageMultiplier(UpgradeType.Electrical));

			// Destroy the projectile on hit
			Destroy(gameObject);
		}
	}
}