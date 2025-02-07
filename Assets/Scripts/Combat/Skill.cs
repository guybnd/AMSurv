using UnityEngine;




public enum TargetType // for skills to target specific types of objects
{
    Player,
    Enemy,
    Both // Targets both Players and Enemies
}

/// <summary>
/// Abstract base class for all skills. Provides common properties and methods for skill behavior, usable by any character.
/// Skills now target a direction and are more generic, with configurable Target Types.
/// </summary>
public abstract class Skill : MonoBehaviour
{
    [Header("Skill Base Properties")]
    [Tooltip("Name of the skill (for identification and display).")]
    public string skillName = "Skill";

    [Tooltip("Base cooldown time in seconds.")]
    public float baseCooldownTime = 1f;

    [Header("Targeting")]
    [Tooltip("Determines who this skill can target.")]
    public TargetType targetType = TargetType.Both; // Default to targetting both

    protected float currentCooldownTime = 0f; // Tracks the current cooldown

    /// <summary>
    /// Initializes the skill. Can be used for setup if needed.
    /// </summary>
    public virtual void InitializeSkill()
    {
        currentCooldownTime = 0f; // Skill starts ready
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
    /// Starts the skill's cooldown.
    /// </summary>
    public virtual void BeginCooldown()
    {
        currentCooldownTime = baseCooldownTime;
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