using System;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot
{
    Weapon,
    Offhand,
    Helmet,
    Gloves,
    Boots,
    Chest,
    RingLeft,
    RingRight,
    Amulet,
    Belt
}

public class Inventory : MonoBehaviour
{
    private CharacterStats characterStats;

    private Dictionary<EquipmentSlot, Item> equippedItems = new Dictionary<EquipmentSlot, Item>();
    private List<Item> inventoryBag = new List<Item>();

    // Events
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats component not found on the GameObject!");
        }

        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            equippedItems[slot] = null;
        }
    }

    public void EquipItem(Item item, EquipmentSlot slot)
    {
        if (item == null)
        {
            Debug.LogError("Cannot equip a null item!");
            return;
        }

        if (!IsItemValidForSlot(item, slot))
        {
            Debug.LogError($"Cannot equip {item.ItemName}: Item type {item.Type} does not match slot {slot}.");
            return;
        }

        if (equippedItems[slot] != null)
        {
            UnequipItem(slot);
        }

        equippedItems[slot] = item;
        item.ApplyTo(characterStats);

        Debug.Log($"Equipped {item.ItemName} to {slot}");
        OnEquipmentChanged?.Invoke();
    }


    public bool TryEquipItem(Item item)
{
    foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
    {
        if (IsItemValidForSlot(item, slot))
        {
            EquipItem(item, slot); // Equip the item
            RemoveFromBag(item); // Remove it from the bag
            return true; // Successful equip
        }
    }
    Debug.LogWarning($"No valid slot found for {item.ItemName}.");
    return false; // No valid slot
}

    public void UnequipItem(EquipmentSlot slot)
    {
        if (equippedItems[slot] == null)
        {
            Debug.LogWarning($"No item equipped in {slot} slot to unequip!");
            return;
        }

        equippedItems[slot].RemoveFrom(characterStats);
        Debug.Log($"Unequipped {equippedItems[slot].ItemName} from {slot}");
        equippedItems[slot] = null;

        OnEquipmentChanged?.Invoke();
    }

    public void AddToBag(Item item, int quantity = 1)
    {
        if (item.IsStackable)
        {
            foreach (var existingItem in inventoryBag)
            {
                if (existingItem.ItemName == item.ItemName && existingItem.CurrentStack < existingItem.MaxStack)
                {
                    int spaceLeft = existingItem.MaxStack - existingItem.CurrentStack;
                    int amountToAdd = Mathf.Min(spaceLeft, quantity);

                    existingItem.CurrentStack += amountToAdd;
                    quantity -= amountToAdd;

                    Debug.Log($"Added {amountToAdd} {item.ItemName} to an existing stack. Remaining quantity: {quantity}");

                    if (quantity <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return;
                    }
                }
            }
        }

        while (quantity > 0)
        {
            Item newItem = Instantiate(item);
            int amountToAdd = Mathf.Min(item.MaxStack, quantity);

            newItem.CurrentStack = amountToAdd;
            inventoryBag.Add(newItem);

            quantity -= amountToAdd;

            Debug.Log($"Added {amountToAdd} {item.ItemName} to a new slot. Remaining quantity: {quantity}");
        }

        OnInventoryChanged?.Invoke();
    }

    public void RemoveFromBag(Item item)
    {
        if (inventoryBag.Remove(item))
        {
            Debug.Log($"{item.ItemName} removed from the inventory bag.");
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"{item.ItemName} was not found in the inventory bag.");
        }
    }

    public List<Item> GetBagContents()
    {
        return inventoryBag;
    }

    public int GetItemIndex(Item item)
{
    return inventoryBag.IndexOf(item);
}

    public Item GetEquippedItem(EquipmentSlot slot)
    {
        if (equippedItems.ContainsKey(slot))
        {
            Debug.Log($"Item in {slot}: {equippedItems[slot]?.ItemName ?? "None"}");
            return equippedItems[slot];
        }
        Debug.LogError($"Equipment slot {slot} not found!");
        return null;
    }

    public void SwapItems(int indexA, int indexB)
{
    if (indexA >= 0 && indexA < inventoryBag.Count && indexB >= 0 && indexB < inventoryBag.Count)
    {
        Item temp = inventoryBag[indexA];
        inventoryBag[indexA] = inventoryBag[indexB];
        inventoryBag[indexB] = temp;

        Debug.Log($"Swapped items at index {indexA} and {indexB}.");
        OnInventoryChanged?.Invoke(); // Trigger UI update
    }
    else
    {
        Debug.LogError("Invalid indices for SwapItems.");
    }
}

    private bool IsItemValidForSlot(Item item, EquipmentSlot slot)
    {
        return (item.Type, slot) switch
        {
            (ItemType.Weapon, EquipmentSlot.Weapon) => true,
            (ItemType.Offhand, EquipmentSlot.Offhand) => true,
            (ItemType.Helmet, EquipmentSlot.Helmet) => true,
            (ItemType.Gloves, EquipmentSlot.Gloves) => true,
            (ItemType.Boots, EquipmentSlot.Boots) => true,
            (ItemType.Chest, EquipmentSlot.Chest) => true,
            (ItemType.Ring, EquipmentSlot.RingLeft or EquipmentSlot.RingRight) => true,
            (ItemType.Amulet, EquipmentSlot.Amulet) => true,
            (ItemType.Belt, EquipmentSlot.Belt) => true,
            _ => false,
        };
    }



        public void ShowEquippedItems()
    {
        foreach (var kvp in equippedItems)
        {
            Debug.Log($"{kvp.Key}: {(kvp.Value != null ? kvp.Value.ItemName : "None")}");
        }
    }
}
