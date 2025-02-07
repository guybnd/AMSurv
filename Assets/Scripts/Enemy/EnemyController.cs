using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic; // Required for List

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Components")]
    [Tooltip("Reference to the EnemyStats component.")]
    [SerializeField] private EnemyStats enemyStats;
    [Tooltip("Target (typically the player).")]
    [SerializeField] private Transform target;

    [Header("Skills")]
    [Tooltip("List of skills this enemy can use.")]
    [SerializeField] private List<Skill> skills = new List<Skill>(); // List of skills

    [Header("Attack Event")]
    [Tooltip("Event triggered when the enemy performs an attack.")]
    public UnityEvent OnAttack;


    private float attackTimer = 0f;
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    private void Awake()
    {
        // Ensure enemyStats is assigned.
        if (enemyStats == null)
        {
            enemyStats = GetComponent<EnemyStats>();
            if (enemyStats == null)
            {
                Debug.LogError("EnemyController requires an EnemyStats component on the same GameObject.");
            }
        }

        // Auto-assign target if not manually set.
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("EnemyController could not find a GameObject tagged 'Player' for targeting.");
            }
        }

        // Initialize Skills - VERY IMPORTANT
        foreach (Skill skill in skills)
        {
            skill.InitializeSkill(); // Initialize each skill (no CharacterStats passed now)
        }
    }

    private void Update()
    {
        // Retrieve all behavior parameters from the enemy's stats.
        // Ensure these stats ("DetectionRange", "AttackRange", "AttackCooldown", "Speed") have been set
        // via your EnemyDefaults asset and applied to CharacterStats.
        float detectionRange = enemyStats.characterStats.GetStat("DetectionRange").GetValue();
        float attackRange = enemyStats.characterStats.GetStat("AttackRange").GetValue();
        float attackCooldown = enemyStats.characterStats.GetStat("AttackCooldown").GetValue();
        float speed = enemyStats.characterStats.GetStat("MovementSpeed").GetValue();

        // Decrement the attack cooldown timer.
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Update Cooldowns for all skills
        foreach (Skill skill in skills)
        {
            skill.UpdateSkillCooldown();
        }


        // Determine distance to target.
        float distance = (target != null) ? Vector2.Distance(transform.position, target.position) : Mathf.Infinity;

        // Determine state based on distance.
        if (distance <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (distance <= detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Idle;
        }

        // Execute behavior based on the current state.
        switch (currentState)
        {
            case State.Idle:
                IdleBehavior();
                break;
            case State.Chase:
                ChaseBehavior(speed);
                break;
            case State.Attack:
                AttackBehavior(); // Modified to handle skill selection
                break;
        }
    }


    private void IdleBehavior()
    {
       // Debug.Log($"{enemyStats.EnemyName} is idle.");
        // (Optional) Add idle animations or effects here.
    }

    private void ChaseBehavior(float speed)
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        //Debug.Log($"{enemyStats.EnemyName} is chasing at speed {speed}.");
    }

    private void AttackBehavior() // Modified AttackBehavior
    {
        if (attackTimer > 0f)
        {
           // Debug.Log($"{enemyStats.EnemyName} is preparing an attack. Cooldown remaining: {attackTimer:F2} sec");
            return;
        }

        // 1. Skill Selection Logic:
        Skill skillToUse = ChooseSkill(); // Implement skill selection logic

        if (skillToUse != null)
        {
            Debug.Log($"{enemyStats.EnemyName} is using skill: {skillToUse.skillName}");
            // 2. Calculate Direction to Target:
            Vector2 attackDirection = Vector2.zero; // Default direction if no target
            if (target != null)
            {
                attackDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
            }

            skillToUse.ActivateSkill(attackDirection); // Activate skill, passing direction
            OnAttack?.Invoke(); // You can still trigger OnAttack event if needed for animations/sounds
            attackTimer = enemyStats.characterStats.GetStat("AttackCooldown").GetValue(); // Reset attack timer in EnemyController
        }
        else
        {
            Debug.Log($"{enemyStats.EnemyName} wants to attack, but no skill is ready.");
            // Optionally, handle case where no skill is ready (e.g., fall back to basic attack, wait, etc.)
        }
    }


    /// <summary>
    /// Logic to choose which skill the enemy should use for an attack.
    /// </summary>
    /// <returns>The Skill to use, or null if no skill is available.</returns>
    private Skill ChooseSkill()
    {
        // Simple skill selection: Use the first skill that is off cooldown
        foreach (Skill skill in skills)
        {
            if (skill.IsSkillReady())
            {
                return skill;
            }
        }
        return null; // No skill is ready
    }
}