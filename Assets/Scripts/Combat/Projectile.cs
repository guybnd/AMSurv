using UnityEngine;

/// <summary>
///  Basic projectile script. Handles movement, damage application on collision, and despawning after duration.
///  Modified to respect TargetType and initialize with it.
/// </summary>
public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private AttackData attackData;
    private TargetType targetType; // Added TargetType
    private float timer;

    [Header("Projectile Settings")]
    [SerializeField] private float duration = 1f;         // Default duration (will be overridden by customDuration)
    [SerializeField] private float pierceChance = 0f;      // Pierce chance in percentage (default 0%)
                                                           // 100% guarantees a pierce, 80% means 80% chance, etc.
    private float activeDuration; // The actual duration to use for self-destruction


    private void Start()
    {
        // Schedule self-destruction after the active duration.
        Destroy(gameObject, activeDuration);
    }

    /// <summary>
    /// Initializes the projectile with direction, speed, attack data, duration, and target type.
    /// </summary>
    /// <param name="dir">Normalized direction vector.</param>
    /// <param name="projSpeed">Projectile speed.</param>
    /// <param name="data">Attack data for the projectile.</param>
    /// <param name="customDuration">Custom duration for this projectile instance.</param>
    /// <param name="targetType">Target type of the projectile.</param>
    public void Initialize(Vector2 dir, float projSpeed, AttackData data, float customDuration, TargetType targetType) // Modified Initialize to accept TargetType
    {
        direction = dir;
        speed = projSpeed;
        attackData = data;
        activeDuration = customDuration;    // Override the default duration
        this.targetType = targetType;      // Set the targetType

        timer = 0f; // Initialize timer

        // Optionally, rotate the projectile to face its movement direction.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        // Move the projectile in the given direction.
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        timer += Time.deltaTime;
        if (timer >= duration) // Ensure duration based despawn still works, even if Initialize's customDuration is used.
        {
            Destroy(gameObject); // Despawn after duration
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check Target Type before applying damage
        if (IsCorrectTargetType(other.gameObject))
        {
            CharacterStats targetStats = other.GetComponent<CharacterStats>();
            if (targetStats != null)
            {
                float damageToDeal = attackData.damage;
                if (attackData.isCriticalHit)
                {
                    Debug.Log("Critical Hit!"); // Or trigger visual/sound effect
                }

                Stat targetLife = targetStats.GetStat("Life");
                targetLife.BaseValue -= damageToDeal;
                Debug.Log($"{other.gameObject.name} took {damageToDeal} projectile damage. Remaining Life: {targetLife.BaseValue}");

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
        else
        {
            // If it's not the correct target type, destroy the projectile
            Destroy(gameObject); // Or handle passing through incorrect targets if desired
        }
    }


    /// <summary>
    /// Checks if the given GameObject is of the correct TargetType for this projectile.
    /// (Same logic as in Skill.cs)
    /// </summary>
    /// <param name="targetGameObject">The GameObject to check.</param>
    /// <returns>True if the target is of the correct type, false otherwise.</returns>
    private bool IsCorrectTargetType(GameObject targetGameObject)
    {
        if (targetType == TargetType.Both) return true; // Skill targets both

        if (targetType == TargetType.Player && targetGameObject.CompareTag("Player")) return true; // Skill targets Player and hit is Player

        if (targetType == TargetType.Enemy && targetGameObject.CompareTag("Enemy")) return true; // Skill targets Enemy and hit is Enemy

        return false; // Target type does not match
    }
}