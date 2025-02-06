using System.Collections.Generic;
using UnityEngine;

public enum WeaponStat
{
    BaseAttackSpeed,
    BaseCriticalChance,
    CritMultiplier,
    ProjectileAmount,
    ProjectileSpeed,
    MinPhysicalDamage,
    MaxPhysicalDamage,
    MinFireDamage,
    MaxFireDamage,
    MinColdDamage,
    MaxColdDamage,
    MinLightningDamage,
    MaxLightningDamage,
    MinChaosDamage,
    MaxChaosDamage,
    AreaOfEffect
}

public enum WeaponType
{
    Gun,
    Rifle,
    Melee,
    None
}

public class Weapon : MonoBehaviour
{
    // Weapon properties
    public float BaseAttackSpeed { get; set; } = 1.0f;
    public WeaponType weaponType { get; set; } = WeaponType.None;
    public float BaseCriticalChance { get; set; } = 0.05f;
    public float CritMultiplier { get; set; } = 1.5f;
    public int ProjectileAmount { get; set; } = 1;
    public float ProjectileSpeed { get; set; } = 10.0f;

    public float MinPhysicalDamage { get; set; }
    public float MaxPhysicalDamage { get; set; }

    public float MinFireDamage { get; set; }
    public float MaxFireDamage { get; set; }
    public float MinColdDamage { get; set; }
    public float MaxColdDamage { get; set; }
    public float MinLightningDamage { get; set; }
    public float MaxLightningDamage { get; set; }
    public float MinChaosDamage { get; set; }
    public float MaxChaosDamage { get; set; }

    public float AreaOfEffect { get; set; }

    // Store the base stats to revert to upon unequip.
    private Dictionary<WeaponStat, float> baseWeaponStats = new Dictionary<WeaponStat, float>();

    private void Awake()
    {
        // Save the initial (base) stats for later use.
        baseWeaponStats[WeaponStat.BaseAttackSpeed] = BaseAttackSpeed;
        baseWeaponStats[WeaponStat.BaseCriticalChance] = BaseCriticalChance;
        baseWeaponStats[WeaponStat.ProjectileAmount] = ProjectileAmount;
        baseWeaponStats[WeaponStat.ProjectileSpeed] = ProjectileSpeed;
        baseWeaponStats[WeaponStat.MinPhysicalDamage] = MinPhysicalDamage;
        baseWeaponStats[WeaponStat.MaxPhysicalDamage] = MaxPhysicalDamage;
        baseWeaponStats[WeaponStat.MinFireDamage] = MinFireDamage;
        baseWeaponStats[WeaponStat.MaxFireDamage] = MaxFireDamage;
        baseWeaponStats[WeaponStat.MinColdDamage] = MinColdDamage;
        baseWeaponStats[WeaponStat.MaxColdDamage] = MaxColdDamage;
        baseWeaponStats[WeaponStat.MinLightningDamage] = MinLightningDamage;
        baseWeaponStats[WeaponStat.MaxLightningDamage] = MaxLightningDamage;
        baseWeaponStats[WeaponStat.MinChaosDamage] = MinChaosDamage;
        baseWeaponStats[WeaponStat.MaxChaosDamage] = MaxChaosDamage;
        baseWeaponStats[WeaponStat.AreaOfEffect] = AreaOfEffect;
    }

    /// <summary>
    /// Overrides all current weapon stats with the new stats.
    /// For any stat not included in the newStats dictionary, its value is set to 0.
    /// </summary>
    /// <param name="newStats">A dictionary mapping each WeaponStat to its new value.</param>
    public void SetWeaponStats(Dictionary<WeaponStat, float> newStats)
    {
        BaseAttackSpeed = newStats.ContainsKey(WeaponStat.BaseAttackSpeed) ? newStats[WeaponStat.BaseAttackSpeed] : 0f;
        BaseCriticalChance = newStats.ContainsKey(WeaponStat.BaseCriticalChance) ? newStats[WeaponStat.BaseCriticalChance] : 0f;
        ProjectileAmount = newStats.ContainsKey(WeaponStat.ProjectileAmount) ? (int)newStats[WeaponStat.ProjectileAmount] : 0;
        ProjectileSpeed = newStats.ContainsKey(WeaponStat.ProjectileSpeed) ? newStats[WeaponStat.ProjectileSpeed] : 0f;
        MinPhysicalDamage = newStats.ContainsKey(WeaponStat.MinPhysicalDamage) ? newStats[WeaponStat.MinPhysicalDamage] : 0f;
        MaxPhysicalDamage = newStats.ContainsKey(WeaponStat.MaxPhysicalDamage) ? newStats[WeaponStat.MaxPhysicalDamage] : 0f;
        MinFireDamage = newStats.ContainsKey(WeaponStat.MinFireDamage) ? newStats[WeaponStat.MinFireDamage] : 0f;
        MaxFireDamage = newStats.ContainsKey(WeaponStat.MaxFireDamage) ? newStats[WeaponStat.MaxFireDamage] : 0f;
        MinColdDamage = newStats.ContainsKey(WeaponStat.MinColdDamage) ? newStats[WeaponStat.MinColdDamage] : 0f;
        MaxColdDamage = newStats.ContainsKey(WeaponStat.MaxColdDamage) ? newStats[WeaponStat.MaxColdDamage] : 0f;
        MinLightningDamage = newStats.ContainsKey(WeaponStat.MinLightningDamage) ? newStats[WeaponStat.MinLightningDamage] : 0f;
        MaxLightningDamage = newStats.ContainsKey(WeaponStat.MaxLightningDamage) ? newStats[WeaponStat.MaxLightningDamage] : 0f;
        MinChaosDamage = newStats.ContainsKey(WeaponStat.MinChaosDamage) ? newStats[WeaponStat.MinChaosDamage] : 0f;
        MaxChaosDamage = newStats.ContainsKey(WeaponStat.MaxChaosDamage) ? newStats[WeaponStat.MaxChaosDamage] : 0f;
        AreaOfEffect = newStats.ContainsKey(WeaponStat.AreaOfEffect) ? newStats[WeaponStat.AreaOfEffect] : 0f;
    }

    /// <summary>
    /// Resets the weapon's stats back to its base values.
    /// Also resets the weaponType to None.
    /// </summary>
    public void ResetWeaponStats()
    {
        SetWeaponStats(baseWeaponStats);
        weaponType = WeaponType.None;
    }

    /// <summary>
    /// (Optional) Applies additional stats additively. Not used when overriding.
    /// </summary>
    public void ApplyItemStats(Dictionary<WeaponStat, float> itemStats)
    {
        if (itemStats == null)
            return;

        foreach (var stat in itemStats)
        {
            switch (stat.Key)
            {
                case WeaponStat.BaseAttackSpeed:
                    BaseAttackSpeed += stat.Value;
                    break;
                case WeaponStat.BaseCriticalChance:
                    BaseCriticalChance += stat.Value;
                    break;
                case WeaponStat.ProjectileAmount:
                    ProjectileAmount += (int)stat.Value;
                    break;
                case WeaponStat.ProjectileSpeed:
                    ProjectileSpeed += stat.Value;
                    break;
                case WeaponStat.MinPhysicalDamage:
                    MinPhysicalDamage += stat.Value;
                    break;
                case WeaponStat.MaxPhysicalDamage:
                    MaxPhysicalDamage += stat.Value;
                    break;
                case WeaponStat.MinFireDamage:
                    MinFireDamage += stat.Value;
                    break;
                case WeaponStat.MaxFireDamage:
                    MaxFireDamage += stat.Value;
                    break;
                case WeaponStat.MinColdDamage:
                    MinColdDamage += stat.Value;
                    break;
                case WeaponStat.MaxColdDamage:
                    MaxColdDamage += stat.Value;
                    break;
                case WeaponStat.MinLightningDamage:
                    MinLightningDamage += stat.Value;
                    break;
                case WeaponStat.MaxLightningDamage:
                    MaxLightningDamage += stat.Value;
                    break;
                case WeaponStat.MinChaosDamage:
                    MinChaosDamage += stat.Value;
                    break;
                case WeaponStat.MaxChaosDamage:
                    MaxChaosDamage += stat.Value;
                    break;
                case WeaponStat.AreaOfEffect:
                    AreaOfEffect += stat.Value;
                    break;
                default:
                    Debug.LogWarning("Weapon: Unhandled stat " + stat.Key);
                    break;
            }
        }
    }

    /// <summary>
    /// (Optional) Removes additively applied stats.
    /// </summary>
    public void RemoveItemStats(Dictionary<WeaponStat, float> itemStats)
    {
        if (itemStats == null)
            return;

        foreach (var stat in itemStats)
        {
            switch (stat.Key)
            {
                case WeaponStat.BaseAttackSpeed:
                    BaseAttackSpeed -= stat.Value;
                    break;
                case WeaponStat.BaseCriticalChance:
                    BaseCriticalChance -= stat.Value;
                    break;
                case WeaponStat.ProjectileAmount:
                    ProjectileAmount -= (int)stat.Value;
                    break;
                case WeaponStat.ProjectileSpeed:
                    ProjectileSpeed -= stat.Value;
                    break;
                case WeaponStat.MinPhysicalDamage:
                    MinPhysicalDamage -= stat.Value;
                    break;
                case WeaponStat.MaxPhysicalDamage:
                    MaxPhysicalDamage -= stat.Value;
                    break;
                case WeaponStat.MinFireDamage:
                    MinFireDamage -= stat.Value;
                    break;
                case WeaponStat.MaxFireDamage:
                    MaxFireDamage -= stat.Value;
                    break;
                case WeaponStat.MinColdDamage:
                    MinColdDamage -= stat.Value;
                    break;
                case WeaponStat.MaxColdDamage:
                    MaxColdDamage -= stat.Value;
                    break;
                case WeaponStat.MinLightningDamage:
                    MinLightningDamage -= stat.Value;
                    break;
                case WeaponStat.MaxLightningDamage:
                    MaxLightningDamage -= stat.Value;
                    break;
                case WeaponStat.MinChaosDamage:
                    MinChaosDamage -= stat.Value;
                    break;
                case WeaponStat.MaxChaosDamage:
                    MaxChaosDamage -= stat.Value;
                    break;
                case WeaponStat.AreaOfEffect:
                    AreaOfEffect -= stat.Value;
                    break;
                default:
                    Debug.LogWarning("Weapon: Unhandled stat " + stat.Key);
                    break;
            }
        }
    }
    [ContextMenu("Show Stats in Console")]
    public void PrintStats()
    {
        Debug.Log("Weapon Stats:\n" +
                  $"BaseAttackSpeed: {BaseAttackSpeed}\n" +
                  $"BaseCriticalChance: {BaseCriticalChance}\n" +
                  $"ProjectileAmount: {ProjectileAmount}\n" +
                  $"ProjectileSpeed: {ProjectileSpeed}\n" +
                  $"MinPhysicalDamage: {MinPhysicalDamage}\n" +
                  $"MaxPhysicalDamage: {MaxPhysicalDamage}\n" +
                  $"MinFireDamage: {MinFireDamage}\n" +
                  $"MaxFireDamage: {MaxFireDamage}\n" +
                  $"MinColdDamage: {MinColdDamage}\n" +
                  $"MaxColdDamage: {MaxColdDamage}\n" +
                  $"MinLightningDamage: {MinLightningDamage}\n" +
                  $"MaxLightningDamage: {MaxLightningDamage}\n" +
                  $"MinChaosDamage: {MinChaosDamage}\n" +
                  $"MaxChaosDamage: {MaxChaosDamage}\n" +
                  $"AreaOfEffect: {AreaOfEffect}\n" +
                  $"WeaponType: {weaponType}");
    }
}
