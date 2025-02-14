using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public string StatName;
    public float Value;
    public bool IsMultiplicative;
}
[System.Serializable]
public class Stat
{
    public float BaseValue;

    private List<float> additiveModifiers = new List<float>();
    private List<float> multiplicativeModifiers = new List<float>();

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
    }

    public void AddModifier(string statName, float modifier, bool isMultiplicative = false)
    {
        if (isMultiplicative) multiplicativeModifiers.Add(modifier);
        else additiveModifiers.Add(modifier);
        float newValue = GetValue();
        Debug.Log($"Stat: AddModifier - Stat {statName}, Added {modifier} to BaseValue {BaseValue}. Total Additive Modifiers: {additiveModifiers.Count}, Total Multiplicative Modifiers: {multiplicativeModifiers.Count}, New Value: {newValue}");
    }

    public void RemoveModifier(string statName, float modifier, bool isMultiplicative = false)
    {
        if (isMultiplicative)
        {
            if (multiplicativeModifiers.Contains(modifier))
            {
                multiplicativeModifiers.Remove(modifier);
                float newValue = GetValue();
                Debug.Log($"Stat: RemoveModifier -  Stat {statName}, Removed {modifier} from BaseValue {BaseValue}. Total Multiplicative Modifiers: {multiplicativeModifiers.Count}, New Value: {newValue}");
            }
            else
            {
                Debug.LogWarning($"Stat: RemoveModifier - Attempted to remove multiplicative modifier {modifier}, but it was not found.");
            }
        }
        else
        {
            if (additiveModifiers.Contains(modifier))
            {
                additiveModifiers.Remove(modifier);
                float newValue = GetValue();
                Debug.Log($"Stat: RemoveModifier - Stat {statName}, Removed {modifier} from BaseValue {BaseValue}. Total Additive Modifiers: {additiveModifiers.Count}, New Value: {newValue}");
            }
            else
            {
                Debug.LogWarning($"Stat: RemoveModifier - Attempted to remove additive modifier {modifier}, but it was not found.");
            }
        }
    }

    public float GetValue()
    {
        float finalValue = BaseValue;
        finalValue += additiveModifiers.Sum();
        finalValue *= 1 + multiplicativeModifiers.Sum();
        return finalValue;
    }

    /// <summary>
    /// Sets the base value of the stat directly.
    /// </summary>
    /// <param name="newValue">The new base value to set.</param>
    public void SetValue(float newValue) // ADD THIS METHOD
    {
        BaseValue = newValue;
    }
}