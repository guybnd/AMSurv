using UnityEngine;

/// <summary>
/// A concrete skill class for melee attacks. Extends the base Skill class with melee-specific logic.
/// Now targets a direction and is more generic, and respects Target Types.
/// </summary>
public class MeleeAttackSkill : Skill
{
    [Header("Melee Attack Settings")]
    [Tooltip("Range of the melee attack in direction.")]
    public float meleeRange = 1.5f;

    [Tooltip("Base damage of the melee attack.")]
    public float baseDamage = 10f; // Base damage defined directly in the skill


    /// <inheritdoc />
    public override void ActivateSkill(Vector2 direction)
    {
        if (!IsSkillReady()) return; // Skill is on cooldown

        Debug.Log($"{skillName} activated in direction: {direction}, Target Type: {targetType}!");

        // 1. Perform Directional Melee Attack Logic:
        // Cast a ray or overlap in the given direction

        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + direction.normalized * meleeRange; // Attack in direction

        RaycastHit2D[] hitInfo = Physics2D.LinecastAll(startPosition, endPosition);

        foreach (RaycastHit2D hit in hitInfo)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject) // Hit something that is not the skill's owner
            {
                // Check Target Type before applying damage
                if (IsCorrectTargetType(hit.collider.gameObject))
                {
                    CharacterStats targetStats = hit.collider.GetComponent<CharacterStats>();
                    if (targetStats != null)
                    {
                        float damageToDeal = baseDamage; // Damage is now directly from skill, can be modified based on caster stats later if needed
                        Stat targetLife = targetStats.GetStat("Life");
                        targetLife.BaseValue -= damageToDeal;
                        Debug.Log($"{hit.collider.gameObject.name} took {damageToDeal} melee damage from {skillName}. Remaining Life: {targetLife.BaseValue}");
                    }
                    // You could add other effects or logic based on what is hit here (e.g., knockback, visual effects)
                }
            }
        }


        // 2. Begin Skill Cooldown
        BeginCooldown();
    }


    /// <summary>
    /// Checks if the given GameObject is of the correct TargetType for this skill.
    /// </summary>
    /// <param name="targetGameObject">The GameObject to check.</param>
    /// <returns>True if the target is of the correct type, false otherwise.</returns>
    private bool IsCorrectTargetType(GameObject targetGameObject)
    {
        if (targetType == TargetType.All) return true; // Skill targets both

        if (targetType == TargetType.Player && targetGameObject.CompareTag("Player")) return true; // Skill targets Player and hit is Player

        if (targetType == TargetType.Enemy && targetGameObject.CompareTag("Enemy")) return true; // Skill targets Enemy and hit is Enemy

        return false; // Target type does not match
    }


    private void OnDrawGizmosSelected() // For visualizing melee range in the editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Vector2.right * meleeRange); // Default direction visualization
    }
}