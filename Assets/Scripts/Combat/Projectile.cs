using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private AttackData attackData;

    [Header("Projectile Settings")]
    [SerializeField] private float duration = 1f;       // Default duration (will be overridden by customDuration)
    [SerializeField] private float pierceChance = 0f;     // Pierce chance in percentage (default 0%)
                                                          // 100% guarantees a pierce, 80% means 80% chance, etc.
    private float activeDuration; // The actual duration to use for self-destruction

    private void Start()
    {
        // Schedule self-destruction after the active duration.
        Destroy(gameObject, activeDuration);
    }

    /// <summary>
    /// Initializes the projectile.
    /// </summary>
    /// <param name="dir">Normalized direction vector.</param>
    /// <param name="projSpeed">Projectile speed.</param>
    /// <param name="data">Attack data for the projectile.</param>
    /// <param name="customDuration">Custom duration for this projectile instance.</param>
    public void Initialize(Vector2 dir, float projSpeed, AttackData data, float customDuration)
    {
        direction = dir;
        speed = projSpeed;
        attackData = data;
        activeDuration = customDuration;  // Override the default duration

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
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackData.damage, attackData.isCriticalHit);
            }

            // Check if the projectile should pierce the enemy.
            if (Random.Range(0f, 100f) < pierceChance)
            {
                // Successful pierce: reduce pierceChance by 100 (but not below 0)
                pierceChance = Mathf.Max(0f, pierceChance - 100f);
                // Do not destroy the projectile, allowing it to hit another enemy.
            }
            else
            {
                // Projectile does not pierce: destroy it.
                Destroy(gameObject);
            }
        }
    }
}
