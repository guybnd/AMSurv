using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [Tooltip("Reference to the CharacterStats component for the player.")]
    [SerializeField] private CharacterStats playerStats;

    [Header("Skills")]
    [Tooltip("List of skills the player can use.")]
    [SerializeField] private List<Skill> skills = new List<Skill>();

    private ProjectileSkill projectileSkill;

    // --- Dodge Roll Fields ---
    private bool isDodging = false;
    public bool IsDodging { get { return isDodging; } } // Exposed for DamageReceiver checks
    private float dodgeTimer = 0f;
    [SerializeField] private float dodgeDuration = 0.5f; // Duration of the dodge (modifiable via stats/equipment)
    [SerializeField] private float dodgeSpeed = 10f;       // Dodge movement speed (modifiable via stats/equipment)
    private Vector2 dodgeDirection;

    // --- Post-Dodge Recovery Fields ---
    private bool isRecovering = false;
    private float recoveryTimer = 0f;
    [SerializeField] private float postDodgeRecoveryDuration = 0.5f; // How long the speed penalty lasts after dodging
    [SerializeField]
    [Range(0f, 1f)]
    private float movementSpeedPenaltyPercentage = 0.5f; // e.g., 0.5 means 50% of normal speed (50% slower)
    private float originalMovementSpeed; // To store the normal movement speed

    // References to other components
    private PlayerMovement pm;      // Handles regular movement (assumed to have a public "movementSpeed" field)
    private Animator animator;      // For triggering the dodge animation
    private PlayerAnimator playerAnimator; // (Optional) if you want integration with PlayerAnimator

    private void Awake()
    {
        // Initialize skills
        foreach (Skill skill in skills)
        {
            skill.InitializeSkill();
        }
        projectileSkill = GetComponentInChildren<ProjectileSkill>();
        if (projectileSkill == null)
        {
            Debug.LogError("PlayerController: ProjectileSkill not found in children. Ensure one exists.");
        }

        // Get references to other components
        pm = GetComponent<PlayerMovement>(); // Your movement script
        animator = GetComponent<Animator>();
        playerAnimator = GetComponent<PlayerAnimator>();

        // Store the original movement speed from the PlayerMovement script
        if (pm != null)
        {
            originalMovementSpeed = pm.movementSpeed;
        }
    }

    private void Update()
    {
        // Update skill cooldowns
        foreach (Skill skill in skills)
        {
            skill.UpdateSkillCooldown();
        }

        // Check for dodge input (Space key) – only if not already dodging
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging)
        {
            StartDodge();
        }

        // If currently dodging, update dodge movement and skip other input
        if (isDodging)
        {
            UpdateDodge();
            return; // Skip other input (e.g., skill activation) while dodging
        }

        // If in post-dodge recovery, update recovery timer and adjust movement speed
        if (isRecovering)
        {
            recoveryTimer += Time.deltaTime;
            if (pm != null)
            {
                // Apply penalty: reduce movement speed by the penalty percentage
                pm.movementSpeed = originalMovementSpeed * movementSpeedPenaltyPercentage;
            }
            if (recoveryTimer >= postDodgeRecoveryDuration)
            {
                // Recovery finished: restore normal movement speed
                isRecovering = false;
                recoveryTimer = 0f;
                if (pm != null)
                {
                    pm.movementSpeed = originalMovementSpeed;
                }
            }
        }

        HandleInput();
    }

    private void StartDodge()
    {
        // Determine dodge direction:
        // Use PlayerMovement's moveDir if the player is moving.
        if (pm != null && pm.moveDir != Vector2.zero)
        {
            dodgeDirection = pm.moveDir.normalized;
        }
        else
        {
            // Otherwise, dodge toward the mouse's world position.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            dodgeDirection = ((Vector2)mousePos - (Vector2)transform.position).normalized;
        }

        isDodging = true;
        dodgeTimer = 0f;

        // Trigger the dodge roll animation – ensure your Animator Controller has a "DodgeRoll" trigger.
        if (animator != null)
        {
            animator.SetTrigger("DodgeRoll");
        }
    }

    private void UpdateDodge()
    {
        dodgeTimer += Time.deltaTime;
        // Move the player along the dodge direction.
        transform.Translate(dodgeDirection * dodgeSpeed * Time.deltaTime, Space.World);

        if (dodgeTimer >= dodgeDuration)
        {
            EndDodge();
        }
    }

    private void EndDodge()
    {
        isDodging = false;
        dodgeTimer = 0f;

        // Begin post-dodge recovery: the player's movement speed will be lowered for a short time.
        isRecovering = true;
        recoveryTimer = 0f;
    }

    private void HandleInput()
    {
        // Example: Left mouse click to fire projectile skill.
        if (Input.GetMouseButtonDown(0))
        {
            if (projectileSkill != null)
            {
                TryActivateProjectileSkill();
            }
        }
        // Additional input handling for other skills can be added here.
    }

    private void TryActivateProjectileSkill()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure we're in 2D.
        Vector2 fireDirection = ((Vector2)mousePos - (Vector2)transform.position).normalized;

        if (projectileSkill.IsSkillReady())
        {
            Debug.Log("Player activating Projectile Skill");
            projectileSkill.ActivateSkill(fireDirection);
        }
        else
        {
            Debug.Log($"Projectile Skill on cooldown. Remaining: {projectileSkill.currentCooldownTime:F2} sec");
        }
    }
}
