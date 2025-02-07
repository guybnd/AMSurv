using UnityEngine;

/// <summary>
/// Represents a projectile fired by a skill.
/// Handles movement, damage application on collision, and despawning.
/// </summary>
public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private AttackData attackData; // Store the AttackData to pass damage info
    private float duration;
    private TargetType targetType;
    private float areaOfEffectRadius = 0f; // AOE Radius - will be set if skill has AOE
    private GameObject shooter; // Store the shooter GameObject

    private float currentLifeTime = 0f;


    /// <summary>
    /// Initializes the projectile with its direction, speed, damage data, duration, and target type.
    /// </summary>
    /// <param name="direction">Direction of projectile movement.</param>
    /// <param name="speed">Projectile speed.</param>
    /// <param name="attackData">Data containing damage information.</param>
    /// <param name="duration">Lifetime of the projectile in seconds.</param>
    /// <param name="targetType">Target type of the skill (Player or Enemy).</param>
    /// <param name="shooter">The GameObject that fired this projectile.</param> // Add shooter parameter
    public void Initialize(Vector2 direction, float speed, AttackData attackData, float duration, TargetType targetType, GameObject shooter) // Modify Initialize function to accept shooter
    {
        this.direction = direction.normalized; // Normalize direction to ensure consistent speed
        this.speed = speed;
        this.attackData = attackData; // Store the damage data
        this.duration = duration;
        this.targetType = targetType;
        this.shooter = shooter; // Store the shooter GameObject
        currentLifeTime = 0f; // Reset lifetime on initialize

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = this.direction * this.speed;
        }
        else
        {
            Debug.LogError("Projectile missing Rigidbody2D component!");
        }
    }


    /// <summary>
    /// Sets the area of effect radius for the projectile.
    /// </summary>
    /// <param name="radius">Area of effect radius.</param>
    public void SetAreaOfEffect(float radius)
    {
        areaOfEffectRadius = radius;
        // You might want to visualize the AOE radius here, e.g., by scaling a child sprite or enabling a visual effect.
        Debug.LogWarning("SetAreaOfEffect() called, AOE visualization not yet implemented.");
    }


    private void Update()
    {
        // Projectile despawn after duration
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= duration)
        {
            Destroy(gameObject); // Despawn projectile after lifetime
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- Collision Detection and Damage Application ---

        // Ignore collision with the shooter
        if (collision.gameObject == shooter)
        {
            return; // Exit early if the collision is with the shooter
        }

        DamageReceiver targetDamageReceiver = null;
        bool isValidTarget = false;
        bool isEnvironment = false; // Flag for environment collision

        if (targetType == TargetType.Enemy)
        {
            // Projectile is from Player, targeting Enemies
            if (collision.gameObject.CompareTag("Enemy"))
            {
                targetDamageReceiver = collision.GetComponent<DamageReceiver>();
                isValidTarget = true;
            }
        }
        else if (targetType == TargetType.Player)
        {
            // Projectile is from Enemy, targeting Player
            if (collision.gameObject.CompareTag("Player"))
            {
                targetDamageReceiver = collision.GetComponent<DamageReceiver>();
                isValidTarget = true;
            }
        }
        else if (collision.gameObject.CompareTag("Environment")) // Check for Environment tag
        {
            isEnvironment = true; // It's environment
        }


        // 2. Apply Damage and Despawn if it's a VALID target (Enemy or Player)
        if (isValidTarget && targetDamageReceiver != null)
        {
            Debug.Log($"Projectile hit VALID Target: {collision.gameObject.name}, Damage: {attackData.damage:F2}, Crit: {attackData.isCriticalHit}");
            targetDamageReceiver.TakeDamage(attackData.damage);
            Destroy(gameObject); // Destroy projectile after hitting VALID target
        }
        // 3. Despawn if it hits ENVIRONMENT
        else if (isEnvironment)
        {
            Debug.Log($"Projectile hit Environment: {collision.gameObject.name}. Despawning projectile.");
            Destroy(gameObject); // Destroy projectile on Environment collision
        }
        else
        {
            // Hit something else that is NOT the shooter, NOT a valid target, and NOT environment.
            // In this case, we should NOT despawn, let the projectile pass through.
            Debug.Log($"Projectile hit: {collision.gameObject.name}, but it's NOT shooter, valid target, or environment. Projectile PASSED THROUGH.");
            // Do NOT Destroy(gameObject) here - projectile passes through
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (areaOfEffectRadius > 0f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, areaOfEffectRadius);
        }
    }
}