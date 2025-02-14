using UnityEngine;




public enum TargetType // for skills to target specific types of objects
{
    Player,
    Enemy,
    Environment,
    All // Targets both Players and Enemies
}

public struct AttackData
{
    public float damage;
    public bool isCriticalHit;
}

/// <summary>
/// Abstract base class for all skills. Provides common properties and methods for skill behavior, usable by any character.
/// Skills now target a direction and are more generic, with configurable Target Types.

/// <summary>
/// Abstract base class for all skills. Provides common properties and methods for skill behavior.
/// Skills now use SkillStats ScriptableObject for base stats and leverage owner's CharacterStats for modifiers.
/// </summary>

public abstract class Skill : MonoBehaviour
{
    [Header("Skill Base Properties")]
    [Tooltip("Name of the skill (for identification and display).")]
    public string skillName = "Skill";

    [Tooltip("Base cooldown time in seconds.")]
    public float baseCooldownTime = 1f; // Keep this field for default value in inspector, but SkillStats is preferred

    [Tooltip("Skill Stats ScriptableObject that defines base skill properties.")]
    public SkillStats skillBaseStats; // Assign SkillStats asset here in the Inspector

    [Header("Targeting")]
    [Tooltip("Determines who this skill can target.")]
    public TargetType targetType = TargetType.All; // Default to targetting both

    public float currentCooldownTime = 0f; // Tracks the current cooldown

    /// <summary>
    /// Initializes the skill. Checks for SkillStats asset assignment.
    /// </summary>
    public virtual void InitializeSkill()
    {
        currentCooldownTime = 0f; // Skill starts ready

        if (skillBaseStats == null)
        {
            Debug.LogError($"{skillName}: SkillStats ScriptableObject is not assigned! Skill will not function correctly.");
        }
    }

    /// <summary>
    /// Abstract method to define the specific action of the skill when activated.
    /// Derived classes must implement this.
    /// Now takes a direction vector as a parameter.
    /// </summary>
    /// <param name="direction">The direction in which the skill is activated.</param>
    public abstract void ActivateSkill(Vector2 direction);

    /// <summary>
    /// Reduces the current cooldown time based on elapsed time.
    /// </summary>
    public virtual void UpdateSkillCooldown()
    {
        if (currentCooldownTime > 0f)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime < 0f)
            {
                currentCooldownTime = 0f; // Ensure it doesn't go negative
            }
        }
    }

    /// <summary>
    /// Starts the skill's cooldown, factoring in attack speed bonuses from the owner's stats.
    /// </summary>
    /// <summary>
    /// Starts the skill's cooldown, factoring in attack speed bonuses from the owner's stats and weapon attack speed if applicable.
    /// </summary>
    public virtual void BeginCooldown()
    {
        float baseCooldown = skillBaseStats != null ? skillBaseStats.baseCooldownTime : this.baseCooldownTime;
        float modifiedCooldown = baseCooldown; // Start with base cooldown

        // Check if this skill is a weapon damage skill (and implicitly, if we should use weapon attack speed for cooldown)
        ProjectileSkill projectileSkill = this as ProjectileSkill; // Check if this is a ProjectileSkill (or derived class)
        Weapon equippedWeapon = GetComponentInParent<Weapon>();

        if (projectileSkill != null && projectileSkill.isWeaponDamageSkill && equippedWeapon != null)
        {
            // --- Weapon-Based Cooldown (for weapon damage skills) ---
            float weaponBaseAttackSpeed = equippedWeapon.BaseAttackSpeed; // Get weapon's base attack speed

            // Apply character attack speed bonus to weapon's base attack speed
            CharacterStats ownerStats = GetComponentInParent<CharacterStats>();
            float modifiedAttackSpeed = weaponBaseAttackSpeed;
            if (ownerStats != null)
            {
                Stat attackSpeedStat = ownerStats.GetStat("IncreasedAttackSpeed");
                if (attackSpeedStat != null)
                {
                    float attackSpeedBonus = attackSpeedStat.GetValue();
                    modifiedAttackSpeed = weaponBaseAttackSpeed * (1f + attackSpeedBonus / 100f);
                }
            }

            if (modifiedAttackSpeed > 0) // Avoid division by zero
            {
                modifiedCooldown = 1f / modifiedAttackSpeed; // Cooldown is inverse of attack speed (attacks per second)
            }
            else
            {
                modifiedCooldown = baseCooldown; // Fallback to base cooldown if attack speed is invalid
                Debug.LogWarning($"{skillName}: Weapon attack speed is zero or negative. Using base cooldown.");
            }
        }
        else
        {
            // --- Independent Cooldown (for non-weapon skills, or if no weapon) ---
            // Cooldown is only affected by character attack speed bonus (if any), using skill's baseCooldownTime
            CharacterStats ownerStats = GetComponentInParent<CharacterStats>();
            if (ownerStats != null)
            {
                Stat attackSpeedStat = ownerStats.GetStat("IncreasedAttackSpeed");
                if (attackSpeedStat != null)
                {
                    float attackSpeedBonus = attackSpeedStat.GetValue();
                    modifiedCooldown = baseCooldown / (1f + attackSpeedBonus / 100f); // Apply attack speed bonus to base cooldown
                }
            }
        }


        currentCooldownTime = modifiedCooldown;
        // Debug.Log($"{skillName} cooldown set to: {currentCooldownTime:F2} seconds (Base: {baseCooldown}s)");
    }
    /// <summary>
    /// Checks if the skill is currently off cooldown and ready to use.
    /// </summary>
    /// <returns>True if the skill is ready, false otherwise.</returns>
    public virtual bool IsSkillReady()
    {
        return currentCooldownTime <= 0f;
    }

    // You can add more generic skill properties here, like range, mana cost, etc., and methods if needed.
}