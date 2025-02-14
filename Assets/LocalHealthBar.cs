using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LocalHealthBar : MonoBehaviour
{

    public Slider healthSlider;
    public Slider easeSlider;

    public float maxHealth = 100f;
    public float CurrentHealth;

    private float lerpSpeed = 0.05f;

    // Reference to the player's CharacterStats component
    public CharacterStats entityStats;

    public bool IsPlayer = true;

    private GameObject[] _childObjects;
    private bool _hasTakenDamage = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the CharacterStats component on the player GameObject
        if (this.IsPlayer == true)
        {
            entityStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
            Debug.Log ("PlayerHealthBar: CharacterStats component found on the Player.");
        }
        else if (entityStats == null)
        {
            entityStats = GetComponentInParent<CharacterStats>();
        }
        if (entityStats == null)
        {
            Debug.LogError("PlayerHealthBar: CharacterStats component not found on the Player.  Make sure the Player GameObject has the tag 'Player' and a CharacterStats component.");
            enabled = false; // Disable this script if CharacterStats is not found
            return;
        }
        
        //Get all child objects
        _childObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _childObjects[i] = transform.GetChild(i).gameObject;
        }

        //Disable child objects if not player
        if (!IsPlayer)
        {
            foreach (var child in _childObjects)
            {
                child.SetActive(false);
            }
        }

        maxHealth = entityStats.GetStat("Life").BaseValue;
        CurrentHealth = entityStats.GetStat("Life").GetValue();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = CurrentHealth;
        easeSlider.maxValue = maxHealth;
        easeSlider.value = CurrentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (entityStats != null)
        {
            CurrentHealth = entityStats.GetStat("Life").GetValue();
            healthSlider.value = CurrentHealth;
        }
        if (healthSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, CurrentHealth, lerpSpeed);
        }
    }

    public void OnDamageTaken()
    {
        if (!IsPlayer && !_hasTakenDamage)
        {
            foreach (var child in _childObjects)
            {
                child.SetActive(true);
            }
            _hasTakenDamage = true;
        }
    }
}
