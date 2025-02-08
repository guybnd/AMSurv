using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDefaults", menuName = "ScriptableObjects/EnemyDefaults", order = 1)]
public class EnemyDefaults : ScriptableObject
{
    public string EnemyName = "New Enemy";
    public enum EnemyRarity { Common, Uncommon, Rare, Elite, Boss }
    public EnemyRarity Rarity = EnemyRarity.Common;

    [Tooltip("List of stat modifiers to define base stats for this enemy type.")]
    public List<StatModifier> StatModifiers = new List<StatModifier>();

    [Tooltip("Experience points awarded to player for defeating this enemy type.")]
    public int ExperienceGiven = 10;
    public float TimeToIdle = 2f; // Time before enemy goes back to idle state

    [Header("Skill Prefabs")] // Changed Header Name
    [Tooltip("List of Skill Prefabs this enemy can use. Each prefab should have a Skill MonoBehaviour attached.")] // Updated Tooltip
    public List<GameObject> SkillPrefabs = new List<GameObject>(); // Changed to List<GameObject> and renamed to SkillPrefabs

    [Header("Rarity Scaling (Optional - Set to 1 for no scaling)")]
    [Tooltip("Multiplier applied to all stats for Common rarity.")]
    public float CommonRarityStatMultiplier = 1f;
    [Tooltip("Multiplier applied to all stats for Uncommon rarity.")]
    public float UncommonRarityStatMultiplier = 1.2f;
    [Tooltip("Multiplier applied to all stats for Rare rarity.")]
    public float RareRarityStatMultiplier = 1.5f;
    [Tooltip("Multiplier applied to all stats for Elite rarity.")]
    public float EliteRarityStatMultiplier = 2f;
    [Tooltip("Multiplier applied to all stats for Boss rarity.")]
    public float BossRarityStatMultiplier = 3f;
    // You can add more rarity-specific scaling factors here if needed (e.g., for experience, drops, etc.)
}