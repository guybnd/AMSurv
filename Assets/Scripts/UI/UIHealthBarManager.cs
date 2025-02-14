using System.Collections.Generic;
using UnityEngine;

public class UIHealthBarManager : MonoBehaviour {
    public static UIHealthBarManager Instance { get; private set; }
    public GameObject healthBarPrefab; // Assign your UIHealthBar prefab in the Inspector.
    public Canvas uiCanvas; // Reference to the Canvas.

    private Dictionary<CharacterStats, UIHealthBar> activeHealthBars = new Dictionary<CharacterStats, UIHealthBar>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void RegisterHealthBar(CharacterStats stats) {
        Debug.Log("Registering health bar for " + stats.name);
        if (stats == null) return;
        if (activeHealthBars.ContainsKey(stats)) return;

        GameObject hbGO = Instantiate(healthBarPrefab, uiCanvas.transform);
        UIHealthBar hb = hbGO.GetComponent<UIHealthBar>();

        if (hb != null) {
            hb.characterStats = stats;

            // Determine initial visibility *before* UIHealthBar's Start/Update runs
            Stat lifeStat = stats.GetStat("Life");
            if (lifeStat != null)
            {
                float currentHealth = lifeStat.GetValue();
                hbGO.SetActive(currentHealth < lifeStat.BaseValue); // Set initial visibility
            }
            else
            {
                hbGO.SetActive(false); // Hide if no life stat
                Debug.LogError("Life stat not found on " + stats.name);
            }

            activeHealthBars.Add(stats, hb);
        }
    }

    public void UnregisterHealthBar(CharacterStats stats) {
        if (stats == null) return;
        if (activeHealthBars.ContainsKey(stats)) {
            Destroy(activeHealthBars[stats].gameObject);
            activeHealthBars.Remove(stats);
        }
    }
}
