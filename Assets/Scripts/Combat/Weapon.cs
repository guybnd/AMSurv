using System.Collections.Generic;
using UnityEngine;

public enum WeaponStat
{
    BaseAttackSpeed,
    BaseCriticalChance,
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
    // Example fields (make these [SerializeField] if you want to see them in the inspector)
    public float BaseAttackSpeed { get; set; } = 1.0f;
    public WeaponType weaponType { get; set; } = WeaponType.None;
    public float BaseCriticalChance { get; set; } = 0.05f;
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

    /// <summary>
    /// Applies bonus stats from an item. (Assumes additive stacking.)
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
    /// Removes bonus stats previously applied.
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

    // (Optional) A PrintStats method for debugging (as we set up earlier)
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
                  $"AreaOfEffect: {AreaOfEffect}");
    }
}
