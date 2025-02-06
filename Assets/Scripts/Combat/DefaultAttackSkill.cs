using UnityEngine;

public class DefaultAttackSkill : MonoBehaviour
{
    [Header("Skill Properties")]
    [SerializeField] private float baseCooldown = 1f;  // Base attack skill cooldown (once per second)
    [SerializeField] private float manaCost = 0f;        // Default mana cost is 0

    [Header("References")]
    [SerializeField] private WeaponFirer weaponFirer;    // Reference to the WeaponFirer that fires the projectiles
    [SerializeField] private CharacterStats playerStats;    // Reference to the PlayerStats for mana and attack speed bonus

    // Internal timer to manage cooldown.
    private float currentCooldown = 0f;

    private void Update()
    {
        // Cooldown countdown.
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }

        // Example: Fire on left mouse click.
        if (Input.GetMouseButtonDown(0))
        {
            TryActivateSkill();
        }
    }

    /// <summary>
    /// Attempts to activate the default attack skill.
    /// </summary>
    public void TryActivateSkill()
    {
        // Check if skill is off cooldown.
        if (currentCooldown > 0f)
        {
            Debug.Log("Default attack is on cooldown.");
            return;
        }

        // Check if the player has enough mana.
        if (manaCost > 0f && playerStats.CurrentMana < manaCost)
        {
            Debug.Log("Not enough mana to use Default Attack.");
            return;
        }

        // Calculate the effective attack rate.
        // We assume that weaponFirer.equippedWeapon.BaseAttackSpeed is defined as the number of attacks per second.
        // And playerStats.AttackSpeedBonus is a decimal (e.g., 0.2 for 20% increased attack speed).
        float effectiveAttackRate = weaponFirer.equippedWeapon.BaseAttackSpeed * (1f + playerStats.AttackSpeedBonus);
        // The effective cooldown is the inverse of the effective attack rate.
        float effectiveCooldown = 1f / effectiveAttackRate;

        // Set the cooldown.
        currentCooldown = effectiveCooldown;

        // Subtract mana cost, if any.
        if (manaCost > 0f)
        {
            playerStats.CurrentMana -= manaCost;
        }

        // Fire the weapon using the WeaponFirer.
        weaponFirer.FireWeapon();
    }
}
