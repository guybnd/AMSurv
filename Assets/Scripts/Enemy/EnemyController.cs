using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic; // Required for List
using System; // Required for EventArgs

public class EnemyController : MonoBehaviour // Make it a base class (can be inherited)
{
    [Header("Enemy Components")]
    [Tooltip("Reference to the EnemyStats component.")]
    [SerializeField] protected EnemyStats enemyStats; // Changed to protected for inheritance
    [Tooltip("Target (typically the player).")]
    [SerializeField] protected Transform target; // Changed to protected for inheritance

    [Header("Skills")]
    [Tooltip("List of skills this enemy can use. (Loaded from EnemyDefaults via EnemyStats)")]
    [SerializeField] public List<Skill> skills = new List<Skill>(); // Public for EnemyStats to add skills, but you can adjust access as needed
    // Skills are now loaded and initialized by EnemyStats from EnemyDefaults

    [Header("Attack Event")]
    [Tooltip("Event triggered when the enemy performs an attack.")]
    public UnityEvent OnAttack;

    protected float attackTimer = 0f; // Changed to protected for inheritance
    protected enum State { Idle, Chase, Attack } // Changed to protected for inheritance
    protected State currentState = State.Idle; // Changed to protected for inheritance


    protected virtual void Awake() // Make virtual for derived classes to override
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

        // Skills are now initialized in EnemyStats after loading from EnemyDefaults
        // InitializeSkills(); // REMOVE - Skills are initialized in EnemyStats now

        DamageReceiver damageReceiver = GetComponent<DamageReceiver>(); // Get DamageReceiver component
        if (damageReceiver != null)
        {
            damageReceiver.OnCharacterDeath += HandleCharacterDeath; // Use '+=' for standard C# events
            Debug.Log($"{gameObject.name} (EnemyController) subscribed to OnCharacterDeath event from DamageReceiver."); // Debug log for subscription
        }
        else
        {
            Debug.LogError("DamageReceiver is null on " + gameObject.name + " EnemyController. Cannot subscribe to OnCharacterDeath event.");
        }
    }

    public void InitializeSkills() // Public method to initialize skills, called from EnemyStats
    {
        // Initialize Skills - VERY IMPORTANT
        foreach (Skill skill in skills)
        {
            skill.InitializeSkill(); // Initialize each skill
        }
    }


    protected virtual void Update() // Make virtual for derived classes to override
    {
        // Retrieve all behavior parameters from the enemy's stats.
        // Ensure these stats ("DetectionRange", "AttackRange", "AttackCooldown", "Speed") are defined in EnemyDefaults
        float detectionRange = enemyStats.GetStat("DetectionRange").GetValue();
        float attackRange = enemyStats.GetStat("AttackRange").GetValue();
        float attackCooldown = enemyStats.GetStat("AttackCooldown").GetValue();
        float speed = enemyStats.GetStat("MovementSpeed").GetValue();

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


    protected virtual void IdleBehavior() // Make virtual for derived classes to override
    {
        // Debug.Log($"{enemyStats.EnemyName} is idle.");
        // (Optional) Add idle animations or effects here.
    }

    protected virtual void ChaseBehavior(float speed) // Make virtual for derived classes to override
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        //Debug.Log($"{enemyStats.EnemyName} is chasing at speed {speed}.");
    }

    protected virtual void AttackBehavior() // Make virtual for derived classes to override
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
            attackTimer = enemyStats.GetStat("AttackCooldown").GetValue(); // Reset attack timer in EnemyController
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
    protected virtual Skill ChooseSkill() // Make virtual for derived classes to override
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

    private void HandleCharacterDeath(object sender, System.EventArgs e) // Event handler method, matches event signature
    {
        Debug.Log($"{gameObject.name} (EnemyController) HandleCharacterDeath() method called in response to OnCharacterDeath event."); // Log when HandleCharacterDeath is called

        // --- Death Logic ---

        // --- Experience Award (Now to Player's Stats) ---
        if (enemyStats != null && target != null) // Ensure enemyStats and target are not null
        {
            CharacterStats playerStats = target.GetComponent<CharacterStats>(); // Get CharacterStats from the target (Player)
            if (playerStats != null)
            {
                int experienceToGive = enemyStats.enemyDefaults.ExperienceGiven;
                playerStats.AddExperience(experienceToGive); // Award experience to player's stats
                Debug.Log($"{gameObject.name} - Awarding {experienceToGive} experience to player's CharacterStats."); // Log experience award
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - Target (Player) does not have CharacterStats component. Cannot award experience.");
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} - EnemyStats or Target is null, cannot award experience.");
        }

        // --- Death Effects (Example - you can expand this) ---
        Debug.Log($"{gameObject.name} - Playing death effects (example - override in derived classes).");
        // You would add code here to play death animations, particle effects, sounds, etc.

        // --- Destroy Enemy GameObject ---
        Debug.Log($"{gameObject.name} (EnemyController) - Destroying GameObject.");
        Destroy(gameObject); // Or consider object pooling for better performance
        Debug.Log($"{gameObject.name} - GameObject Destroyed. Death sequence complete.");
    }
}