using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private AttackData attackData;

    // Call this method to initialize the projectile.
    public void Initialize(Vector2 dir, float projSpeed, AttackData data)
    {
        direction = dir;
        speed = projSpeed;
        attackData = data;

        // Optionally, rotate the projectile to face its movement direction.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        // Move the projectile in the given direction.
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // When colliding with an enemy, apply the damage.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackData.damage, attackData.isCriticalHit);
            }
            Destroy(gameObject); // Destroy projectile after hit.
        }
    }
}
