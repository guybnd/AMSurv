using UnityEngine;
using System; // Required for events

/// <summary>
/// Component to handle receiving damage and applying it to CharacterStats.
/// This is a generic damage receiver that can be used for any entity that can take damage.
/// </summary>
public class DamageReceiver : MonoBehaviour
{
    [Tooltip("CharacterStats component attached to this GameObject.")]
    [SerializeField] private CharacterStats characterStats; // Drag the CharacterStats component here in Inspector

    // --- Death Event ---
    public event EventHandler OnCharacterDeath; // Event to be invoked when character dies

    private void Awake()
    {
        if (characterStats == null)
        {
            characterStats = GetComponent<CharacterStats>();
            if (characterStats == null)
            {
                Debug.LogError("DamageReceiver: CharacterStats component not found on this GameObject. Please attach CharacterStats to the same GameObject.");
                enabled = false; // Disable this DamageReceiver if CharacterStats is missing
            }
        }
    }

    /// <summary>
    /// Applies damage to the character.
    /// </summary>
    /// <param name="damageAmount">The total damage amount to be applied (after damage type calculations).</param>
    public void TakeDamage(float damageAmount)
    {
        if (characterStats == null)
        {
            Debug.LogWarning("DamageReceiver: TakeDamage called but CharacterStats is missing. Damage cannot be applied.");
            return;
        }

        // --- Apply Damage Mitigation (Armor, Resistance, Evasion - To be implemented later) ---
        float mitigatedDamage = damageAmount; // For now, no mitigation is applied. Damage is direct.

        // --- Reduce Life Stat ---
        Stat lifeStat = characterStats.GetStat("Life"); // Assuming your Life stat is named "Life" in CharacterStats
        if (lifeStat != null)
        {
            float currentLife = lifeStat.GetValue();
            float newLife = currentLife - mitigatedDamage;

            if (newLife < 0)
            {
                newLife = 0; // Ensure life doesn't go below zero
            }

            lifeStat.SetValue(newLife); // Update the Life stat with the new value

            Debug.Log($"{gameObject.name} took {mitigatedDamage:F2} damage. New Life: {newLife:F2}");

            // --- Handle Death ---
            if (newLife <= 0)
            {
                Debug.Log($"{gameObject.name} has died!");
                OnCharacterDeath?.Invoke(this, EventArgs.Empty); // Invoke the death event
            }
        }
        else
        {
            Debug.LogError("DamageReceiver: 'Life' stat not found in CharacterStats. Damage cannot be applied.");
        }
    }
}