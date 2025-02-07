using UnityEngine;

/// <summary>
/// This script should be attached to the enemy’s attack hitbox (a trigger collider).
/// When the enemy’s attack collider touches a GameObject tagged "Player", it removes damage
/// from the player's Life stat based on the enemy's current AttackDamage stat (or a default value).
/// A cooldown prevents damage from being applied continuously.
/// </summary>
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Time (in seconds) between consecutive damage applications during collision.")]
    [SerializeField] private float hitCooldown = 1f;

    [Tooltip("Fallback attack damage if the enemy’s AttackDamage stat is not found.")]
    [SerializeField] private float defaultAttackDamage = 10f;

    [Header("Enemy Stats Reference")]
    [Tooltip("Reference to the enemy's CharacterStats component (to retrieve the AttackDamage stat).")]
    [SerializeField] private CharacterStats enemyCharacterStats;

    // Internal timer for managing the hit cooldown.
    private float hitTimer = 0f;

    private void Update()
    {
        // Countdown the hit timer.
        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
        }
    }

    // Called when a collider enters the trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // Called each frame the collider stays inside the trigger.
    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    /// <summary>
    /// Attempts to damage the player if the collider belongs to the player and the hit cooldown has expired.
    /// </summary>
    private void TryDamagePlayer(Collider2D other)
    {
        if (hitTimer > 0f)
            return; // Still in cooldown; do not apply damage.

        // Check if the collider is tagged "Player".
        if (other.CompareTag("Player"))
        {
            // Get the player's CharacterStats component.
            CharacterStats playerStats = other.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                // Determine enemy attack damage from its stats.
                // The "AttackDamage" stat should be set up via your EnemyDefaults asset.
                float attackDamage = defaultAttackDamage;
                if (enemyCharacterStats != null)
                {
                    // Retrieve the "AttackDamage" stat value.
                    // If the stat is not found, the fallback value is used.
                    attackDamage = enemyCharacterStats.GetStat("AttackDamage").GetValue();
                }

                // Apply the damage to the player's Life stat.
                Stat playerLife = playerStats.GetStat("Life");
                playerLife.BaseValue -= attackDamage;
                Debug.Log($"Player took {attackDamage} damage from enemy attack. Remaining Life: {playerLife.BaseValue}");

                // Reset the hit cooldown timer.
                hitTimer = hitCooldown;
            }
            else
            {
                Debug.LogWarning("Player does not have a CharacterStats component.");
            }
        }
    }
}
