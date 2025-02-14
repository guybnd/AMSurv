using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{

    public Slider healthSlider;
    public Slider easeSlider;

    public float maxHealth = 100f;
    public float CurrentHealth;

    private float lerpSpeed = 0.05f;

    // Reference to the player's CharacterStats component
    public CharacterStats playerStats;

    public bool isPlayer = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the CharacterStats component on the player GameObject
        if (isPlayer == true)
        {
                        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        }



        if (playerStats == null)
        {
            
            Debug.LogError("PlayerHealthBar: CharacterStats component not found on the Player.  Make sure the Player GameObject has the tag 'Player' and a CharacterStats component.");
            enabled = false; // Disable this script if CharacterStats is not found
            return;
        }

        maxHealth = playerStats.GetStat("Life").BaseValue;
        CurrentHealth = playerStats.GetStat("Life").GetValue();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = CurrentHealth;
        easeSlider.maxValue = maxHealth;
        easeSlider.value = CurrentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats != null)
        {
            CurrentHealth = playerStats.GetStat("Life").GetValue();
            healthSlider.value = CurrentHealth;
        }
        if (healthSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, CurrentHealth, lerpSpeed);
        }

    }


}
