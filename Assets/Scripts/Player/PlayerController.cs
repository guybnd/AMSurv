using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [Tooltip("Reference to the CharacterStats component for the player.")]
    [SerializeField] private CharacterStats playerStats; // Assign your player's CharacterStats here.

    [Header("Skills")]
    [Tooltip("List of skills the player can use.")]
    [SerializeField] private List<Skill> skills = new List<Skill>(); // Assign player skills here.

    private ProjectileSkill projectileSkill; // Reference to the ProjectileSkill (assuming you'll use this for left click)


    private void Awake()
    {
        // Initialize Skills
        foreach (Skill skill in skills)
        {
            skill.InitializeSkill(); // Initialize each skill
        }

        // Get a reference to the ProjectileSkill (assuming you have one in the skills list)
        projectileSkill = GetComponentInChildren<ProjectileSkill>(); // Or find it in the skills list if needed.
        if (projectileSkill == null)
        {
            Debug.LogError("PlayerController: ProjectileSkill not found in children. Make sure you have a ProjectileSkill attached to the player or its children.");
        }
    }


    private void Update()
    {
        // Update skill cooldowns
        foreach (Skill skill in skills)
        {
            skill.UpdateSkillCooldown();
        }


        HandleInput();
    }


    private void HandleInput()
    {
        // Example: Left mouse click to fire projectile skill
        if (Input.GetMouseButtonDown(0))
        {
            if (projectileSkill != null)
            {
                TryActivateProjectileSkill();
            }
        }
        // You can add input handling for other skills here (e.g., number keys for skill selection)
    }


    private void TryActivateProjectileSkill()
    {
        // Get mouse position to determine firing direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure Z is 0 for 2D
        Vector2 fireDirection = ((Vector2)mousePos - (Vector2)transform.position).normalized;


        if (projectileSkill.IsSkillReady())
        {
            Debug.Log("Player activating Projectile Skill");
            projectileSkill.ActivateSkill(fireDirection); // Activate projectile skill with direction
        }
        else
        {
            Debug.Log($"Projectile Skill on cooldown. Remaining: {projectileSkill.currentCooldownTime:F2} sec");
        }
    }
}