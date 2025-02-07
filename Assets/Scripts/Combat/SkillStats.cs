using UnityEngine;

/// <summary>
/// ScriptableObject to hold the base stats for a skill.
/// These stats can be modified by the character's own stats and equipped weapon stats when the skill is used.
/// </summary>
[CreateAssetMenu(fileName = "NewSkillStats", menuName = "Skills/Skill Stats", order = 1)]
public class SkillStats : ScriptableObject
{
    [Header("Base Skill Properties")]
    [Tooltip("Name of the skill (for identification and display).")]
    public string skillName = "Skill";
    [Tooltip("Base cooldown time in seconds.")]
    public float baseCooldownTime = 1f;

    [Header("Projectile Settings (if applicable)")]
    [Tooltip("Base projectile speed.")]
    public float projectileSpeed = 10f;
    [Tooltip("Default projectile duration before despawning.")]
    public float defaultProjectileDuration = 1f;
    [Tooltip("Number of projectiles fired per skill activation.")]
    public int projectileAmount = 1;
    [Tooltip("Angle of spread between projectiles (in degrees).")]
    public float spreadAngle = 5f;

    [Header("Skill Modifiers (Multiplicative - Applied on top of Weapon and Character Stats)")]
    [Tooltip("Damage multiplier applied to the final calculated damage.")]
    [Range(0f, 5f)] public float attackDamageModifier = 1f; // Multiplier for skill damage

    [Header("Independent Skill Damage (if applicable - for skills that don't use weapon damage)")]
    [Tooltip("Base critical hit chance of the skill (0 to 1).")]
    [Range(0f, 1f)] public float baseCriticalChance = 0.05f; // Default 5% crit chance for skills
    [Tooltip("Critical hit damage multiplier for skills.")]
    public float critMultiplier = 2f; // 2x crit multiplier by default

    [Tooltip("Minimum physical damage of the skill (independent of weapon).")]
    public float minPhysicalDamage = 0f;
    [Tooltip("Maximum physical damage of the skill (independent of weapon).")]
    public float maxPhysicalDamage = 0f;
    [Tooltip("Minimum fire damage of the skill (independent of weapon).")]
    public float minFireDamage = 0f;
    [Tooltip("Maximum fire damage of the skill (independent of weapon).")]
    public float maxFireDamage = 0f;
    [Tooltip("Minimum cold damage of the skill (independent of weapon).")]
    public float minColdDamage = 0f;
    [Tooltip("Maximum cold damage of the skill (independent of weapon).")]
    public float maxColdDamage = 0f;
    [Tooltip("Minimum lightning damage of the skill (independent of weapon).")]
    public float minLightningDamage = 0f;
    [Tooltip("Maximum lightning damage of the skill (independent of weapon).")]
    public float maxLightningDamage = 0f;
    [Tooltip("Minimum chaos damage of the skill (independent of weapon).")]
    public float minChaosDamage = 0f;
    [Tooltip("Maximum chaos damage of the skill (independent of weapon).")]
    public float maxChaosDamage = 0f;


}