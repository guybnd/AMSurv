using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private AttackData attackData;

    [Header("Projectile Settings")]
    [SerializeField] public float duration = 1f;       // Time (in seconds) before the projectile self-destructs.
    [SerializeField] private float pierceChance = 0f;     // Pierce chance in percentage (default 0%).
                                                          // For example, 100 means always pierce, 80 means 80% chance.
                                                          // If above 100, each successful pierce reduces this value by 100.

    private void Start()
    {
        // Automatically destroy this projectile after 'duration' seconds.
        Destroy(gameObject, duration);
    }

    /// <summary>
    /// Initializes the projectile.
    /// </summary>
    /// <param name="dir">The normalized direction vector for movement.</param>
    /// <param name="projSpeed">The projectile's speed.</param>
    /// <param name="data">Attack data that carries damage values and crit info.</param>
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply damage to the enemy.
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackData.damage, attackData.isCriticalHit);
            }

            // Check if the projectile pierces the enemy.
            if (Random.Range(0f, 100f) < pierceChance)
            {
                // The projectile pierces the enemy.
                // Reduce the pierce chance by 100 (but don't let it drop below 0).
                pierceChance = Mathf.Max(0f, pierceChance - 100f);
                // (The projectile is not destroyed, so it can hit additional enemies.)
            }
            else
            {
                // The projectile does not pierce, so destroy it.
                Destroy(gameObject);
            }
        }
    }
}
