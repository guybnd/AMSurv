using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Dictionary<string, Stat> Stats = new Dictionary<string, Stat>();
    
    private float defaultStatValue = 0; // Default base value for new stats
    


    
    void Awake()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        Stats.Add("Strength", new Stat(10));
        Stats.Add("Dexterity", new Stat(10));
        Stats.Add("Intelligence", new Stat(10));
        Stats.Add("Life", new Stat(100));

    }


    public Stat GetStat(string statName)
    {
        if (!Stats.ContainsKey(statName))
        {
            Debug.LogWarning($"Stat {statName} does not exist. Adding it dynamically with a default value of {defaultStatValue}.");
            Stats.Add(statName, new Stat(defaultStatValue)); // Use the configurable default value
        }

        return Stats[statName];
    }
    public void AddStat(string statName, float baseValue)
    {
        if (!Stats.ContainsKey(statName))
            Stats.Add(statName, new Stat(baseValue));
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
