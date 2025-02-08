using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Components")]
    [Tooltip("Reference to the EnemyStats component.")]
    [SerializeField] protected EnemyStats enemyStats;
    [Tooltip("Target (typically the player).")]
    [SerializeField] protected Transform target;

    [Header("Skills")]
    [Tooltip("List of skills this enemy can use. (Loaded from EnemyDefaults via EnemyStats)")]
    [SerializeField] public List<Skill> skills = new List<Skill>();

    [Header("Attack Event")]
    [Tooltip("Event triggered when the enemy performs an attack.")]
    public UnityEvent OnAttack;

    protected float attackTimer = 0f;
    protected float idleTimer = 0f;
    protected float timeToIdle = 3f;

    protected enum State { Idle, Awake, Chase, Attack, Die }
    protected State currentState = State.Idle;

    private bool hasAggravated = false; // Tracks if the awake animation has been triggered.
    private Animator animator;          // Reference to the Animator component.

    // Timer variables for the Awake state.
    private float awakeTimer = 0f;
    [Tooltip("Maximum time (in seconds) to wait in the Awake state before forcing a transition.")]
    [SerializeField] private float maxAwakeDuration = 2f; // Adjust to match your awake animation length.

    // --- New variables for Attack Animation control ---
    private bool isAttackAnimationPlaying = false;
    private float attackAnimationTimer = 0f;
    [Tooltip("Maximum time (in seconds) to wait in the Attack state before forcing a transition.")]
    [SerializeField] private float maxAttackAnimationDuration = 1f; // Adjust to match your attack animation length.

    protected virtual void Awake()
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

        // Subscribe to the character death event.
        DamageReceiver damageReceiver = GetComponent<DamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.OnCharacterDeath += HandleCharacterDeath;
            Debug.Log($"{gameObject.name} (EnemyController) subscribed to OnCharacterDeath event from DamageReceiver.");
        }
        else
        {
            Debug.LogError("DamageReceiver is null on " + gameObject.name + " EnemyController. Cannot subscribe to OnCharacterDeath event.");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing on " + gameObject.name + " EnemyController.");
        }
    }

    public void InitializeSkills()
    {
        // Initialize each skill.
        foreach (Skill skill in skills)
        {
            skill.InitializeSkill();
        }
    }

    protected virtual void Update()
    {
        // Update attack cooldown timer.
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Update skill cooldowns.
        foreach (Skill skill in skills)
        {
            skill.UpdateSkillCooldown();
        }

        // --- BLOCK MOVEMENT WHILE ATTACKING ---
        if (isAttackAnimationPlaying)
        {
            attackAnimationTimer += Time.deltaTime;
            if (attackAnimationTimer >= maxAttackAnimationDuration)
            {
                //Debug.Log($"<color=orange>{enemyStats.EnemyName}:</color> Max attack duration exceeded, forcing finish.");
                OnAttackAnimationFinished();
            }
            else
            {
                //Debug.Log($"<color=yellow>{enemyStats.EnemyName}:</color> Attack animation playing; skipping movement update (timer: {attackAnimationTimer:F2}).");
                return;
            }
        }

        // Retrieve parameters from enemyStats.
        float detectionRange = enemyStats.GetStat("DetectionRange").GetValue();
        float attackRange = enemyStats.GetStat("AttackRange").GetValue();
        float speed = enemyStats.GetStat("MovementSpeed").GetValue();
        timeToIdle = enemyStats.GetStat("TimeToIdle").GetValue();

        // --- STATE SELECTION ---
        // Only update the state based on distance if not in a transitional (Awake) or Die state.
        if (currentState != State.Awake && currentState != State.Die)
        {
            if (target != null)
            {
                float distance = Vector2.Distance(transform.position, target.position);
                if (distance <= attackRange)
                {
                    currentState = State.Attack;
                    idleTimer = 0f;
                }
                else if (distance <= detectionRange)
                {
                    currentState = State.Chase;
                    idleTimer = 0f;
                }
                else
                {
                    currentState = State.Idle;
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= timeToIdle)
                    {
                        currentState = State.Idle;
                        idleTimer = 0f;
                    }
                }
            }
            else
            {
                currentState = State.Idle;
            }
        }

        // --- ANIMATION STATE CONTROL ---
        if (animator != null)
        {
            switch (currentState)
            {
                case State.Idle:
                    animator.SetInteger("EnemyState", 0);
                    break;
                case State.Awake:
                    animator.SetInteger("EnemyState", 1);
                    break;
                case State.Chase:
                    animator.SetInteger("EnemyState", 2);
                    break;
                case State.Attack:
                    animator.SetInteger("EnemyState", 3);
                    break;
                case State.Die:
                    animator.SetInteger("EnemyState", 4);
                    break;
            }
        }

        // --- EXECUTE BEHAVIOR ---
        switch (currentState)
        {
            case State.Idle:
                IdleBehavior();
                break;
            case State.Awake:
            case State.Chase:
                ChaseBehavior(speed);
                break;
            case State.Attack:
                AttackBehavior();
                break;
            case State.Die:
                DieBehavior();
                break;
        }
    }

    protected virtual void IdleBehavior()
    {
        // Reset aggravated flag and awake timer when idle.
        hasAggravated = false;
        awakeTimer = 0f;
    }

    /// <summary>
    /// Handles chasing behavior.
    /// If the enemy hasn't played its awake animation, it triggers it and enters the Awake state.
    /// While in Awake, it waits for the Animation Event or until maxAwakeDuration is reached.
    /// </summary>
    /// <param name="speed">Movement speed for chasing.</param>
    protected virtual void ChaseBehavior(float speed)
    {
        if (target == null || currentState == State.Die)
            return;

        // Trigger the awake animation once when entering Chase.
        if (!hasAggravated && animator != null)
        {
            animator.SetTrigger("IsAggravated");
            currentState = State.Awake; // Lock in Awake state.
            hasAggravated = true;
            awakeTimer = 0f;
            Debug.Log($"<color=green>{enemyStats.EnemyName}:</color> Triggered awake animation.");
            return; // Wait for animation or fallback.
        }

        // While in Awake state, wait for the animation event.
        if (currentState == State.Awake)
        {
            awakeTimer += Time.deltaTime;
            if (awakeTimer >= maxAwakeDuration)
            {
                Debug.Log($"<color=orange>{enemyStats.EnemyName}:</color> Max awake duration exceeded, forcing transition to Chase.");
                OnAwakeAnimationFinished(); // Fallback transition.
            }
            else
            {
                //Debug.Log($"<color=yellow>{enemyStats.EnemyName}:</color> Waiting for awake animation to finish (timer: {awakeTimer:F2}).");
            }
            return;
        }

        // If we reach here, state is Chase; proceed with movement.
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        //Debug.Log($"<color=cyan>{enemyStats.EnemyName}:</color> Chasing at speed {speed}.");
    }

    /// <summary>
    /// Should be called by an Animation Event at the end of the awake animation,
    /// or by the fallback timer if the event doesn't fire.
    /// </summary>
    public void OnAwakeAnimationFinished()
    {
        Debug.Log($"<color=purple>{enemyStats.EnemyName}:</color> Awake animation finished. Transitioning to Chase.");
        currentState = State.Chase;
        awakeTimer = 0f;
    }

    /// <summary>
    /// Handles the enemy's attack behavior.
    /// Triggers the attack animation and sets the attack flag so that movement is blocked.
    /// </summary>
    protected virtual void AttackBehavior()
    {
        // Do not attack if on cooldown, during awake transition, or if dying.
        if (attackTimer > 0f || currentState == State.Awake || currentState == State.Die)
            return;

        // Skill selection.
        Skill skillToUse = ChooseSkill();
        if (skillToUse != null && animator != null)
        {
            Debug.Log($"{enemyStats.EnemyName} is using skill: {skillToUse.skillName}");
            animator.SetTrigger("StartAttack");
            // Set the attack flag and reset the timer.
            isAttackAnimationPlaying = true;
            attackAnimationTimer = 0f;

            Vector2 attackDirection = Vector2.zero;
            if (target != null)
                attackDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;

            skillToUse.ActivateSkill(attackDirection);
            OnAttack?.Invoke();
            attackTimer = enemyStats.GetStat("AttackCooldown").GetValue();
        }
        else
        {
            Debug.Log($"{enemyStats.EnemyName} wants to attack, but no skill is ready.");
        }
    }

    /// <summary>
    /// This method should be called by an Animation Event at the end of the attack animation,
    /// or by the fallback timer if the event doesn't fire.
    /// </summary>
    public void OnAttackAnimationFinished()
    {
        Debug.Log($"<color=purple>{enemyStats.EnemyName}:</color> Attack animation finished.");
        isAttackAnimationPlaying = false;
        attackAnimationTimer = 0f;
    }

    protected virtual void DieBehavior()
    {
        // Death animation and additional effects can be handled here.
    }

    /// <summary>
    /// Returns the first ready skill, or null if none are available.
    /// </summary>
    protected virtual Skill ChooseSkill()
    {
        foreach (Skill skill in skills)
        {
            if (skill.IsSkillReady())
                return skill;
        }
        return null;
    }

    private void HandleCharacterDeath(object sender, EventArgs e)
    {
        Debug.Log($"{gameObject.name} (EnemyController) HandleCharacterDeath() called in response to OnCharacterDeath event.");
        currentState = State.Die;
        if (animator != null)
        {
            animator.SetTrigger("OnDeath");
            animator.SetInteger("EnemyState", 4);
        }

        // Award experience to player.
        if (enemyStats != null && target != null)
        {
            CharacterStats playerStats = target.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                int experienceToGive = enemyStats.enemyDefaults.ExperienceGiven;
                playerStats.AddExperience(experienceToGive);
                Debug.Log($"{gameObject.name} - Awarding {experienceToGive} experience to player's CharacterStats.");
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

        Debug.Log($"{gameObject.name} - Playing death effects.");
        Debug.Log($"{gameObject.name} (EnemyController) - Destroying GameObject.");
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        Debug.Log($"{gameObject.name} - GameObject Destroyed. Death sequence complete.");
    }
}
