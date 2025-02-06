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
    [SerializeField] public float AttackSpeedBonus { get; set; } = 0f;
    [SerializeField] public float CurrentMana { get; set; } = 100f;

    private void Awake()
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
}
