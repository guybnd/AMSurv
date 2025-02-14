using UnityEngine;
using System;

public class DamageReceiver : MonoBehaviour
{
    [Tooltip("CharacterStats component attached to this GameObject.")]
    [SerializeField] private CharacterStats characterStats;

    private PlayerController pc;

    public event EventHandler OnCharacterDeath;
    public event EventHandler<float> OnDamageTaken;

    private UIHealthBarManager _uiHealthBarManager;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
       
        if (characterStats == null)
        {
            characterStats = GetComponent<CharacterStats>();
            if (characterStats == null)
            {
                Debug.LogError("DamageReceiver: CharacterStats component not found. Attach CharacterStats to this GameObject.");
                enabled = false;
            }
        }
    }

    private UIHealthBarManager GetHealthBarManager()
    {
        if (_uiHealthBarManager == null)
        {
            Debug.Log("Finding UIHealthBarManager in the scene.");
            _uiHealthBarManager = FindObjectOfType<UIHealthBarManager>();
            if (_uiHealthBarManager == null)
            {
                Debug.LogError("UIHealthBarManager not found in the scene.");
            }
        }
        return _uiHealthBarManager;
    }

    public void TakeDamage(float damageAmount)
    {
        // Check if this GameObject is the player and is currently dodging (invulnerable)
 
        if (pc != null && pc.IsDodging)
        {
            Debug.Log($"{gameObject.name} is dodging � no damage applied.");
            return;
        }

        // Apply damage (no mitigation for now)
        float mitigatedDamage = damageAmount;
        Stat lifeStat = characterStats.GetStat("Life");
        if (lifeStat != null)
        {
            float currentLife = lifeStat.GetValue();
            float newLife = currentLife - mitigatedDamage;
            if (newLife < 0) newLife = 0;
            lifeStat.SetValue(newLife);
            OnDamageTaken?.Invoke(this, mitigatedDamage);
            Debug.Log($"{gameObject.name} took {mitigatedDamage:F2} damage. New Life: {newLife:F2}");
            if (newLife <= 0)
            {
                Debug.Log($"{gameObject.name} has died!");
                OnCharacterDeath?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Register health bar if life is not full.
                UIHealthBarManager healthBarManager = GetHealthBarManager();
                if (healthBarManager != null) //&& newLife < lifeStat.BaseValue)
                {
                    healthBarManager.RegisterHealthBar(characterStats);
                    Debug.Log($"{gameObject.name} health bar registered.");
                }
            }
        }
        else
        {
            Debug.LogError("DamageReceiver: 'Life' stat not found in CharacterStats. Damage cannot be applied.");
        }
    }
}
