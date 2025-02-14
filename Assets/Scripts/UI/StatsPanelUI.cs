using UnityEngine;
using TMPro;

public class StatsPanelUI : MonoBehaviour
{
    public TMP_Text category1Text; // For Life, Mana, EnergyShield
    public TMP_Text category2Text; // For Strength, Dexterity, Intelligence
    public TMP_Text category3Text; // For other stats like Damage & others

    private CharacterStats playerStats;

    private void Awake()
    {
        // Assumes the player GameObject is tagged "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<CharacterStats>();
            // Subscribe to stats change event.
            if(playerStats != null)
            {
                playerStats.OnStatsChanged += UpdateStatsPanel;
            }
        }
    }

    private void Start()
    {
        UpdateStatsPanel();
    }

    public void UpdateStatsPanel()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("StatsPanelUI: Player stats not found.");
            return;
        }

        // Category 1: Life / max life, Mana / max mana, EnergyShield/ max energy shield
        float life = playerStats.GetStat("Life")?.GetValue() ?? 0;
        float maxLife = playerStats.GetStat("MaxLife")?.GetValue() ?? 0;
        float mana = playerStats.GetStat("Mana")?.GetValue() ?? 0;
        float maxMana = playerStats.GetStat("MaxMana")?.GetValue() ?? 0;
        float energyShield = playerStats.GetStat("EnergyShield")?.GetValue() ?? 0;
        float maxEnergyShield = playerStats.GetStat("MaxEnergyShield")?.GetValue() ?? 0;
        category1Text.text = $"Life: {life} / {maxLife}\nMana: {mana} / {maxMana}\nEnergyShield: {energyShield} / {maxEnergyShield}";

        // Category 2: Strength, Dexterity, Intelligence
        float strength = playerStats.GetStat("Strength")?.GetValue() ?? 0;
        float dexterity = playerStats.GetStat("Dexterity")?.GetValue() ?? 0;
        float intelligence = playerStats.GetStat("Intelligence")?.GetValue() ?? 0;
        category2Text.text = $"Strength: {strength}\nDexterity: {dexterity}\nIntelligence: {intelligence}";

        // Category 3: Other stats (excluding the ones already listed)
        System.Text.StringBuilder otherStatsBuilder = new System.Text.StringBuilder();
        foreach (var stat in playerStats.GetAllStats())
        {
            if (stat.Key != "Life" && stat.Key != "MaxLife" && stat.Key != "Mana" && stat.Key != "MaxMana" &&
            stat.Key != "EnergyShield" && stat.Key != "MaxEnergyShield" && stat.Key != "Strength" &&
            stat.Key != "Dexterity" && stat.Key != "Intelligence")
            {
            otherStatsBuilder.AppendLine($"{stat.Key}: {stat.Value.GetValue()}");
            }
        }
        category3Text.text = otherStatsBuilder.ToString();
    }
}
