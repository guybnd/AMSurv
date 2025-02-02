using System.Collections.Generic;
using UnityEngine;

public enum EnemyRarity
{
    Normal,
    Magic,
    Rare,
    Unique
}

[CreateAssetMenu(fileName = "NewEnemyDefaults", menuName = "Config/EnemyDefaults", order = 1)]
public class EnemyDefaults : ScriptableObject
{
    [Header("Enemy Meta")]
    public string EnemyName = "Enemy";
    public EnemyRarity Rarity = EnemyRarity.Normal;

    [Header("Default Stats")]
    // You can add or remove stat entries in the Inspector.
    public List<StatModifier> StatModifiers = new List<StatModifier>() {
        new StatModifier() { StatName = "MoveSpeed", Value = 3f, IsMultiplicative = false },
        new StatModifier() { StatName = "AttackCooldown", Value = 1.5f, IsMultiplicative = false },
        new StatModifier() { StatName = "DetectionRange", Value = 10f, IsMultiplicative = false },
        new StatModifier() { StatName = "AttackRange", Value = 2f, IsMultiplicative = false },
        new StatModifier() { StatName = "Life", Value = 100f, IsMultiplicative = false },
        new StatModifier() { StatName = "ExperienceGiven", Value = 10f, IsMultiplicative = false }
    };
}
