using UnityEngine;
using System.Collections.Generic;

public class EnemyStats : CharacterStats // Inherit from CharacterStats
{
    [Header("Assign the Enemy Defaults Asset")]
    public EnemyDefaults enemyDefaults; // Drag your EnemyDefaults asset here in the Inspector

    // Meta data for the enemy (for use by other systems, e.g., for display or drop scaling)
    public string EnemyName { get; private set; }
    public EnemyDefaults.EnemyRarity Rarity { get; private set; }

    protected override void Awake() // Use Awake to initialize stats before Start
    {
        // Get the CharacterStats component (base class initialization) - important to call base.Awake()
        //base.Awake();

        if (enemyDefaults == null)
        {
            Debug.LogError("EnemyDefaults asset not assigned in the Inspector for EnemyStats on: " + gameObject.name);
            return;
        }

        // Apply meta data from the defaults.
        EnemyName = enemyDefaults.EnemyName;
        Rarity = enemyDefaults.Rarity;

        // Loop through each defined stat modifier in EnemyDefaults and set up in CharacterStats
        foreach (var modifier in enemyDefaults.StatModifiers)
        {
            // If the stat already exists (from CharacterStats base class), override its base value; otherwise, add it.
            if (Stats.ContainsKey(modifier.StatName)) // Use the inherited Stats dictionary
            {
                Stats[modifier.StatName].BaseValue = modifier.Value;
            }
            else
            {
                AddStat(modifier.StatName, modifier.Value); // Use inherited AddStat
            }
        }

        ApplyRarityScaling(); // Apply stat scaling based on rarity
        LoadSkillsFromDefaults(); // Load skills from EnemyDefaults
    }


    private void ApplyRarityScaling()
    {
        float rarityMultiplier = GetRarityMultiplier(Rarity);

        if (rarityMultiplier != 1f) // Only apply scaling if multiplier is not 1 (to avoid unnecessary calculations for Common rarity)
        {
            Debug.Log($"Applying Rarity Scaling ({Rarity}, Multiplier: {rarityMultiplier}) to {EnemyName}");
            foreach (var statName in Stats.Keys)
            {
                Stat stat = GetStat(statName); // Use inherited GetStat
                if (stat != null)
                {
                    stat.BaseValue *= rarityMultiplier; // Scale base value by rarity multiplier
                    Debug.Log($"  Scaled {statName} to {stat.GetValue()}");
                }
            }
        }
    }


    private float GetRarityMultiplier(EnemyDefaults.EnemyRarity rarity)
    {
        switch (rarity)
        {
            case EnemyDefaults.EnemyRarity.Common: return enemyDefaults.CommonRarityStatMultiplier;
            case EnemyDefaults.EnemyRarity.Uncommon: return enemyDefaults.UncommonRarityStatMultiplier;
            case EnemyDefaults.EnemyRarity.Rare: return enemyDefaults.RareRarityStatMultiplier;
            case EnemyDefaults.EnemyRarity.Elite: return enemyDefaults.EliteRarityStatMultiplier;
            case EnemyDefaults.EnemyRarity.Boss: return enemyDefaults.BossRarityStatMultiplier;
            default: return 1f; // Default to no scaling if rarity is not recognized
        }
    }

    private void LoadSkillsFromDefaults()
    {
        // --- Skill Loading from EnemyDefaults (Prefab Instantiation) ---
        EnemyController controller = GetComponent<EnemyController>(); // Get EnemyController to assign skills
        if (controller != null)
        {
            if (enemyDefaults.SkillPrefabs != null) // Check if SkillPrefabs list is not null
            {
                Debug.Log($"EnemyStats: Instantiating {enemyDefaults.SkillPrefabs.Count} skill prefabs from EnemyDefaults for {EnemyName}.");
                foreach (GameObject skillPrefab in enemyDefaults.SkillPrefabs)
                {
                    if (skillPrefab != null) // Check if prefab is not null
                    {
                        GameObject instantiatedSkillGO = Instantiate(skillPrefab, transform); // Instantiate skill prefab as child
                        Skill skillComponent = instantiatedSkillGO.GetComponent<Skill>(); // Get Skill component from instantiated prefab
                        if (skillComponent != null)
                        {
                            controller.skills.Add(skillComponent); // Add Skill component to EnemyController's list
                            Debug.Log($"  Instantiated skill prefab: {skillPrefab.name}, Skill Component: {skillComponent.skillName}");
                        }
                        else
                        {
                            Debug.LogWarning($"  Instantiated prefab {skillPrefab.name} does not have a Skill component attached!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("  Skill Prefab in EnemyDefaults is null!");
                    }
                }
                controller.InitializeSkills(); // Initialize the loaded skills in EnemyController
                Debug.Log($"EnemyStats: Total skills loaded into EnemyController for {EnemyName}: {controller.skills.Count}");
            }
            else
            {
                Debug.LogWarning($"EnemyStats: SkillPrefabs list in EnemyDefaults is null for {EnemyName}. No skills will be loaded.");
            }
        }
        else
        {
            Debug.LogWarning("EnemyStats: EnemyController component not found. Skills from EnemyDefaults will not be loaded.");
        }
    }


    // --- REMOVE REDUNDANT TakeDamage and Die from EnemyStats ---
    // Damage handling is now done by DamageReceiver and Enemy.
    // public void TakeDamage(float amount, bool isCriticalHit) { ... }
    // private void Die() { ... }
}