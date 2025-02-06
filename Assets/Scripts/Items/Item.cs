using System.Collections.Generic;
using UnityEngine;




public enum ItemType
{
    Weapon,
    Offhand,
    Helmet,
    Gloves,
    Boots,
    Chest,
    Ring,
    Amulet,
    Belt,
    BagItem // Items that cannot be equipped
}

public enum  ItemRarity
{
    Normal,
    Magic,
    Rare,
    Unique
}


[CreateAssetMenu(fileName = "NewItem", menuName = "ARPG/Item")]
public class Item : ScriptableObject
{
    public string ItemName; // Name of the item
    public ItemType Type = ItemType.BagItem; // The type of the item (e.g., Weapon, Helmet)
    public Sprite ItemImage; // The sprite to display in the inventory UI
    public ItemRarity Rarity = ItemRarity.Normal;
        public bool IsStackable; // Determines if the item can stack
    public int MaxStack = 1; // Maximum number of items in a stack
    public int CurrentStack = 1; // Current number of items in the stack
    public List<StatModifier> StatModifiers = new List<StatModifier>();
    public int ID;
    public WeaponType WeaponType { get; set; }

    public void ApplyTo(CharacterStats characterStats)
    {
        foreach (var modifier in StatModifiers)
        {
            var stat = characterStats.GetStat(modifier.StatName);
            if (stat != null)
            {
                stat.AddModifier(modifier.Value, modifier.IsMultiplicative);
            }
        }
    }

    public void RemoveFrom(CharacterStats characterStats)
    {
        foreach (var modifier in StatModifiers)
        {
            var stat = characterStats.GetStat(modifier.StatName);
            if (stat != null)
            {
                stat.RemoveModifier(modifier.Value, modifier.IsMultiplicative);
            }
        }
    }
}
