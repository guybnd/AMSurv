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
            foreach (var statName in Stats.Keys)
            {
                Stat stat = GetStat(statName); // Use inherited GetStat
                if (stat != null)
                {
                    stat.BaseValue *= rarityMultiplier; // Scale base value by rarity multiplier
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
                foreach (GameObject skillPrefab in enemyDefaults.SkillPrefabs)
                {
                    if (skillPrefab != null) // Check if prefab is not null
                    {
                        GameObject instantiatedSkillGO = Instantiate(skillPrefab, transform); // Instantiate skill prefab as child
                        Skill skillComponent = instantiatedSkillGO.GetComponent<Skill>(); // Get Skill component from instantiated prefab
                        if (skillComponent != null)
                        {
                            controller.skills.Add(skillComponent); // Add Skill component to EnemyController's list
                        }
                    }
                }
                controller.InitializeSkills(); // Initialize the loaded skills in EnemyController
            }
        }
    }
}