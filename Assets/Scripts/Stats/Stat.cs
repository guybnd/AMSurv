using System.Collections.Generic;
using System.Linq;
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

    public void AddModifier(float modifier, bool isMultiplicative = false)
    {
        if (isMultiplicative) multiplicativeModifiers.Add(modifier);
        else additiveModifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier, bool isMultiplicative = false)
    {
        if (isMultiplicative) multiplicativeModifiers.Remove(modifier);
        else multiplicativeModifiers.Remove(modifier);
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