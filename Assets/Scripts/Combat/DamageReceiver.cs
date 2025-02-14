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
    private LocalHealthBar _localHealthBar; // Add this line

    private void Start()
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

        _localHealthBar = GetComponentInChildren<LocalHealthBar>(); // Modified to search in child GameObjects
        if (_localHealthBar == null)
        {
            Debug.LogError($"DamageReceiver: LocalHealthBar component not found on {gameObject.name}. Attach LocalHealthBar to this GameObject.");
        }
    }

    private UIHealthBarManager GetHealthBarManager()
    {
        if (_uiHealthBarManager == null)
        {
            _uiHealthBarManager = FindObjectOfType<UIHealthBarManager>();
            if (_uiHealthBarManager == null)
            {
            }
        }
        return _uiHealthBarManager;
    }

    public void TakeDamage(float damageAmount)
    {
        if (pc != null && pc.IsDodging)
        {
            return;
        }

        float mitigatedDamage = damageAmount;
        Stat lifeStat = characterStats.GetStat("Life");
        if (lifeStat != null)
        {
            float currentLife = lifeStat.GetValue();
            float newLife = currentLife - mitigatedDamage;
            if (newLife < 0) newLife = 0;
            lifeStat.SetValue(newLife);
            OnDamageTaken?.Invoke(this, mitigatedDamage);
            _localHealthBar?.OnDamageTaken(); // Add this line
            if (newLife <= 0)
            {
                OnCharacterDeath?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UIHealthBarManager healthBarManager = GetHealthBarManager();
                if (healthBarManager != null)
                {
                    healthBarManager.RegisterHealthBar(characterStats);
                }
            }
        }
        else
        {
        }
    }
}
