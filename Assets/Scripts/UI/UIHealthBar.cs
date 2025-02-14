using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour {
    // Reference to the CharacterStats of the owner (player/enemy)
    public CharacterStats characterStats;
    // UI Image for health bar fill
    public Image healthFill;
    // Optional UI Image for mana bar fill (hide or leave unassigned for enemies)
    public Image manaFill;
    // World offset for positioning the UI element above the entity
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    private float maxHealth;

    void Start() {
        // Cache the initial health value as maxHealth
        if(characterStats != null && characterStats.Stats.ContainsKey("Life")){
            maxHealth = characterStats.Stats["Life"].GetValue();
        } else {
            maxHealth = 100f;
        }
    }

    void Update() {
        if(characterStats != null) {
            // Update health fill amount
            float currentHealth = characterStats.GetStat("Life").GetValue();
            if(healthFill != null) {
                healthFill.fillAmount = currentHealth / maxHealth;
            }
            // Update mana fill amount if the image is assigned
            if(manaFill != null) {
                manaFill.fillAmount = characterStats.CurrentMana / characterStats.MaxMana;
            }
            // Update the UI position to follow the entity
            transform.position = Camera.main.WorldToScreenPoint(characterStats.transform.position + offset);
        }
    }
}
