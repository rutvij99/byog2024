using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private Vector3 direction;
    private int damage;

    public void Launch(Vector3 launchDirection, int projectileDamage)
    {
        direction = launchDirection;
        damage = projectileDamage;

        // Destroy the projectile after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the projectile in the given direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect if the projectile hit a target
        var target = other.GetComponent<PlayerTarget>();
        if (target != null)
        {
            // Apply damage to the target
            target.TakeDamage(damage);
            Destroy(gameObject); // Destroy projectile on hit
        }
    }
}
