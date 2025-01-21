using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public Item testItem;
    public EquipmentSlot testSlot;

    [ContextMenu("Equip Test Item")]
    public void EquipTestItem()
    {
        if (testItem == null)
        {
            Debug.LogError("No test item assigned!");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("No Inventory component assigned!");
            return;
        }

        inventory.EquipItem(testItem, testSlot);
    }

    [ContextMenu("Unequip From Slot")]
    public void UnequipFromSlot()
    {
        if (inventory == null)
        {
            Debug.LogError("No Inventory component assigned!");
            return;
        }

        inventory.UnequipItem(testSlot);
    }

    [ContextMenu("Add to Bag")]
    public void AddToBag()
    {
        if (testItem == null)
        {
            Debug.LogError("No test item assigned!");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("No Inventory component assigned!");
            return;
        }

        inventory.AddToBag(testItem);
    }

    [ContextMenu("Show Equipped Items")]
    public void ShowEquippedItems()
    {
        if (inventory == null)
        {
            Debug.LogError("No Inventory component assigned!");
            return;
        }

        inventory.ShowEquippedItems();
    }

    [ContextMenu("Show Inventory Bag")]
    public void ShowInventoryBag()
    {
        if (inventory == null)
        {
            Debug.LogError("No Inventory component assigned!");
            return;
        }

        Debug.Log("Inventory Bag Contents:");
        foreach (var item in inventory.GetBagContents())
        {
            Debug.Log(item.ItemName);
        }
    }
}
