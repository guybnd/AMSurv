using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Assign the Enemy Defaults Asset")]
    public EnemyDefaults enemyDefaults; // Drag your EnemyDefaults asset here in the Inspector

    // Cached reference to the enemy's unified stat container
    public CharacterStats characterStats { get; private set; }

    // Meta data for the enemy (for use by other systems, e.g., for display or drop scaling)
    public string EnemyName { get; private set; }
    public EnemyRarity Rarity { get; private set; }

    private void Awake()
    {
        // Get the CharacterStats component on this enemy.
        characterStats = GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("EnemyStats requires a CharacterStats component on the same GameObject.");
            return;
        }

        if (enemyDefaults == null)
        {
            Debug.LogError("EnemyDefaults asset not assigned in the Inspector!");
            return;
        }

        // Apply meta data from the defaults.
        EnemyName = enemyDefaults.EnemyName;
        Rarity = enemyDefaults.Rarity;

        // Loop through each defined stat and set it up in the CharacterStats system.
        foreach (var modifier in enemyDefaults.StatModifiers)
        {
            // If the stat already exists, override its base value; otherwise, add it.
            if (characterStats.Stats.ContainsKey(modifier.StatName))
            {
                characterStats.Stats[modifier.StatName].BaseValue = modifier.Value;
            }
            else
            {
                characterStats.AddStat(modifier.StatName, modifier.Value);
            }
        }
    }

    // Example method: take damage using the unified stat system.
    public void TakeDamage(float amount, bool isCriticalHit)
    {
        var lifeStat = characterStats.GetStat("Life");
        lifeStat.BaseValue -= amount;

        string critText = isCriticalHit ? " (CRITICAL HIT!)" : "";
        Debug.Log($"{EnemyName} took {amount} damage{critText}. Remaining Life: {lifeStat.BaseValue}");

        if (lifeStat.BaseValue <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{EnemyName} defeated!");
        Destroy(gameObject);
    }
}
