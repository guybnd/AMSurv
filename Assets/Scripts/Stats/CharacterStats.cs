using System.Collections.Generic;
using UnityEngine;
using System.Linq; // For Sum() on modifier lists if needed

public class CharacterStats : MonoBehaviour
{
    // Dictionary to hold all stats (not shown in Inspector)
    public Dictionary<string, Stat> Stats = new Dictionary<string, Stat>();

    [Header("Default Base Values (modifiable in Inspector)")]
    [SerializeField] private float defaultStrength = 10f;
    [SerializeField] private float defaultDexterity = 10f;
    [SerializeField] private float defaultIntelligence = 10f;
    [SerializeField] private float defaultLife = 100f;
    [SerializeField] public float DamageMultiplier { get; set; } = 1f;
    [SerializeField] public float CritChanceBonus { get; set; } = 0f;
    [SerializeField] public float CritMultiplierBonus { get; set; } = 0f;
    [SerializeField] public float IncreasedAttackSpeed = 0f;
   [SerializeField]  public float IncreasedProjectileSpeed = 0f;
    [SerializeField] public float IncreasedDuration = 0f;
    [SerializeField] public float CurrentMana { get; set; } = 100f;
    [SerializeField] public float MaxMana { get; set; } = 100f;
    [SerializeField] public float ManaRegenRate { get; set; } = 1f;
    [SerializeField] public float CurrentExperience { get; set; } = 0f;

    protected virtual void Awake()
    {
        InitializeStats();
    }

    // Initialize common stats using the provided Stat class.
    private void InitializeStats()
    {
        Stats["Strength"] = new Stat(defaultStrength);
        Stats["Dexterity"] = new Stat(defaultDexterity);
        Stats["Intelligence"] = new Stat(defaultIntelligence);
        Stats["Life"] = new Stat(defaultLife);
        Stats["IncreasedAttackSpeed"] = new Stat(IncreasedAttackSpeed);
        Stats["Experience"] = new Stat(CurrentExperience);
    }

    // Retrieve a stat by name. If missing, add it with a default base value of 0.
    public Stat GetStat(string statName)
    {
        if (!Stats.ContainsKey(statName))
        {
            Debug.LogWarning($"Stat '{statName}' does not exist. Creating it with a default value of 0.");
            Stats[statName] = new Stat(0);
        }
        return Stats[statName];
    }

    // Add a new stat if it doesn't exist.
    public void AddStat(string statName, float baseValue)
    {
        if (!Stats.ContainsKey(statName))
        {
            Stats[statName] = new Stat(baseValue);
        }
    }

    [ContextMenu("Show Stats in Console")]
    private void ShowStatsInConsole()
    {
        Debug.Log("Current Stats:");
        foreach (var stat in Stats)
        {
            Debug.Log($"{stat.Key}: {stat.Value.GetValue()}");
        }
    }

    public void AddExperience(int experienceAmount)
    {
        Stat experienceStat = GetStat("Experience"); // Get the Experience stat
        if (experienceStat != null)
        {
            experienceStat.AddModifier(experienceAmount); // Increase experience value
            Debug.Log($"Player gained {experienceAmount} experience. Total Experience: {experienceStat.GetValue()}");

            // --- Level Up Logic (Example - Add your level up logic here) ---
            Debug.LogWarning("--- Level Up Logic NOT IMPLEMENTED YET in CharacterStats.AddExperience() ---");
            // In a real game, you would check if experienceStat.GetValue() has reached a level-up threshold here,
            // and then trigger level-up actions (increase stats, learn new skills, etc.).
            // --- Level Up Logic ---
        }
        else
        {
            Debug.LogError("Experience stat not found in CharacterStats.AddExperience()!");
        }
    }
}
