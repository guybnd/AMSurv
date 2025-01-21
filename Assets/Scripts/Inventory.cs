using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private CharacterStats characterStats;

    private Dictionary<EquipmentSlot, Item> equippedItems = new Dictionary<EquipmentSlot, Item>();
    private List<Item> inventoryBag = new List<Item>();

    public int TotalSlots = 60;

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

        // Initialize inventoryBag with null values for empty slots
        for (int i = 0; i < TotalSlots; i++)
        {
            inventoryBag.Add(null);
        }
    }

    public void AddToBag(Item item, int quantity = 1)
    {
        for (int i = 0; i < inventoryBag.Count; i++)
        {
            if (inventoryBag[i] == null)
            {
                inventoryBag[i] = Instantiate(item);
                inventoryBag[i].CurrentStack = quantity;
                OnInventoryChanged?.Invoke();
                return;
            }
        }
        Debug.LogWarning("Inventory is full. Cannot add more items.");
    }

    public void RemoveFromBag(Item item)
    {
        int index = inventoryBag.IndexOf(item);
        if (index >= 0)
        {
            inventoryBag[index] = null;
            OnInventoryChanged?.Invoke();
        }
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= inventoryBag.Count || indexB < 0 || indexB >= inventoryBag.Count)
        {
            Debug.LogError($"Invalid indices for SwapItems: indexA={indexA}, indexB={indexB}, Bag Count={inventoryBag.Count}");
            return;
        }

        Item temp = inventoryBag[indexA];
        inventoryBag[indexA] = inventoryBag[indexB];
        inventoryBag[indexB] = temp;

        OnInventoryChanged?.Invoke();
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
        return equippedItems.ContainsKey(slot) ? equippedItems[slot] : null;
    }

    public void EquipItem(Item item, EquipmentSlot slot)
    {
        if (equippedItems[slot] != null)
        {
            UnequipItem(slot);
        }

        equippedItems[slot] = item;
        item.ApplyTo(characterStats);
        OnEquipmentChanged?.Invoke();
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        equippedItems[slot]?.RemoveFrom(characterStats);
        equippedItems[slot] = null;
        OnEquipmentChanged?.Invoke();
    }

    public void ShowEquippedItems()
{
    foreach (var kvp in equippedItems)
    {
        Debug.Log($"{kvp.Key}: {(kvp.Value != null ? kvp.Value.ItemName : "None")}");
    }
}
}
