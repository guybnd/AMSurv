using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A concrete skill class for projectile attacks, usable by players or enemies.
/// Extends the base Skill class, now integrating Weapon MonoBehaviour and SkillStats.
/// </summary>
public class ProjectileSkill : Skill
{
    [Header("Projectile Skill Settings (References)")]
    [Tooltip("Prefab for the projectile to be spawned.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Transform that represents the projectile spawn point.")]
    [SerializeField] private Transform firePoint;

    [Header("Skill Damage Type")]
    [Tooltip("Is this skill damage based on the weapon or independent?")]
    public bool isWeaponDamageSkill = true; // Set to true if skill damage is based on weapon


    /// <inheritdoc />
    public override void ActivateSkill(Vector2 direction)
    {
        if (!IsSkillReady() || skillBaseStats == null) return;

        // Debug.Log($"{skillName} activated in direction: {direction}, Target Type: {targetType}!");

        // Get CharacterStats and Weapon components from the skill's owner (player)
        CharacterStats ownerStats = GetComponentInParent<CharacterStats>();
        Weapon equippedWeapon = GetComponentInParent<Weapon>(); // Get Weapon component directly

        if (ownerStats == null)
        {
            Debug.LogWarning($"{skillName}: CharacterStats not found on parent GameObject. Skill may not behave as expected.");
            return;
        }

        // --- Calculate Modified Skill Stats (Projectile Properties) ---
        float modifiedSpeed = skillBaseStats.projectileSpeed * (1f + ownerStats.IncreasedProjectileSpeed / 100f);
        float modifiedDuration = skillBaseStats.defaultProjectileDuration * (1f + ownerStats.IncreasedDuration / 100f);
        int modifiedProjectileAmount = skillBaseStats.projectileAmount; // Base projectile amount from SkillStats

        // If a weapon is equipped, override projectile amount from weapon
        if (equippedWeapon != null)
        {
            modifiedProjectileAmount = equippedWeapon.ProjectileAmount; // Override with weapon's projectile amount
        }


        // Calculate the damage (and crit status) for this attack, using owner stats and weapon stats.
        AttackData attackData = ComputeAttackDamage(ownerStats, equippedWeapon); // Pass equippedWeapon component

        // Determine the number of projectiles to fire and calculate spread (from modified SkillStats).
        int projectileCount = modifiedProjectileAmount;
        float totalSpread = (projectileCount - 1) * skillBaseStats.spreadAngle;
        float startAngle = -totalSpread / 2f;
        Vector2 baseDirection = direction;


        // Fire each projectile in a spread.
        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = startAngle + i * totalSpread / (projectileCount - 1 > 0 ? projectileCount - 1 : 1) * i; // Fix for projectileCount = 1
            Vector2 finalDirection = RotateVector(baseDirection, angleOffset);

            // Instantiate the projectile.
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                // Pass in the modified projectile speed, duration and target type.
                projectile.Initialize(finalDirection, modifiedSpeed, attackData, modifiedDuration, targetType, gameObject); // Pass 'gameObject' as the shooter
            }
        }

        // Begin Skill Cooldown - Cooldown is now affected by weapon attack speed and player attack speed bonus in Skill.cs
        BeginCooldown();
    }


    // Computes the attack damage based on the equipped weapon and player stats.
    // Computes the attack damage based on the equipped weapon and player stats.
    private AttackData ComputeAttackDamage(CharacterStats ownerStats, Weapon equippedWeapon)
    {
        AttackData data = new AttackData();
        float totalDamage = 0f;
        float totalCritChance = 0f; // Initialize to 0, will be set below
        float totalCritMultiplier = 0f; // Initialize to 0, will be set below


        if (isWeaponDamageSkill && equippedWeapon != null)
        {
            // --- Weapon Damage Skill ---
            // Get damage from equipped weapon
            float physical = equippedWeapon.MinPhysicalDamage;
            float fire = equippedWeapon.MinFireDamage;
            float cold = equippedWeapon.MinColdDamage;
            float lightning = equippedWeapon.MinLightningDamage;
            float chaos = equippedWeapon.MinChaosDamage;

            float maxPhysical = equippedWeapon.MaxPhysicalDamage;
            float maxFire = equippedWeapon.MaxFireDamage;
            float maxCold = equippedWeapon.MaxColdDamage;
            float maxLightning = equippedWeapon.MaxLightningDamage;
            float maxChaos = equippedWeapon.MaxChaosDamage;

            // Roll damage for each type
            float finalPhysical = Random.Range(physical, maxPhysical);
            float finalFire = Random.Range(fire, maxFire);
            float finalCold = Random.Range(cold, maxCold);
            float finalLightning = Random.Range(lightning, maxLightning);
            float finalChaos = Random.Range(chaos, maxChaos);

            // Sum damage from all types
            totalDamage = finalPhysical + finalFire + finalCold + finalLightning + finalChaos;

            // Use weapon's crit chance and multiplier, adding player bonuses
            totalCritChance = equippedWeapon.BaseCriticalChance + ownerStats.CritChanceBonus; // Weapon crit + player bonus
            totalCritMultiplier = equippedWeapon.CritMultiplier + ownerStats.CritMultiplierBonus; // Weapon crit multiplier + player bonus

        }
        else
        {
            // --- Independent Skill Damage --- (Skill's base damage if not weapon-based)
            // For independent skills, we will now define the base crit chance and multiplier in SkillStats
            totalCritChance = skillBaseStats.baseCriticalChance + ownerStats.CritChanceBonus; // Base crit chance from skillStats + player bonus
            totalCritMultiplier = skillBaseStats.critMultiplier + ownerStats.CritMultiplierBonus; // Base crit multiplier from skillStats + player bonus


            // For independent skills, damage will now come from SkillStats directly.
            // Make sure you set the damage ranges in the SkillStats asset for your independent skills (like Grenade).
            float physical = skillBaseStats.minPhysicalDamage;
            float fire = skillBaseStats.minFireDamage;
            float cold = skillBaseStats.minColdDamage;
            float lightning = skillBaseStats.minLightningDamage;
            float chaos = skillBaseStats.minChaosDamage;

            float maxPhysical = skillBaseStats.maxPhysicalDamage;
            float maxFire = skillBaseStats.maxFireDamage;
            float maxCold = skillBaseStats.maxColdDamage;
            float maxLightning = skillBaseStats.maxLightningDamage;
            float maxChaos = skillBaseStats.maxChaosDamage;

            // Roll damage for each type (using SkillStats base damage)
            float finalPhysical = Random.Range(physical, maxPhysical);
            float finalFire = Random.Range(fire, maxFire);
            float finalCold = Random.Range(cold, maxCold);
            float finalLightning = Random.Range(lightning, maxLightning);
            float finalChaos = Random.Range(chaos, maxChaos);

            // Sum damage from all types
            totalDamage = finalPhysical + finalFire + finalCold + finalLightning + finalChaos;


        }


        // Apply the owner's overall Damage Multiplier (from ownerStats)
        totalDamage *= ownerStats.DamageMultiplier;

        // Roll for Critical Hit
        bool isCrit = (Random.value < totalCritChance);
        data.isCriticalHit = isCrit;
        if (isCrit)
        {
            totalDamage *= totalCritMultiplier;
        }

        // Apply Skill Damage Modifier (from SkillStats - multiplicative, on top of everything)
        totalDamage *= skillBaseStats.attackDamageModifier;


        data.damage = totalDamage;
        return data;
    }

    // Rotates a 2D vector by the given angle (in degrees).
    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }


    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
        }
    }
}