using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;

    // Pre-assigned UI elements for equippable slots
    public Button helmetSlot;
    public Button chestSlot;
    public Button glovesSlot;
    public Button bootsSlot;
    public Button weaponSlot;
    public Button offhandSlot;
    public Button ringLeftSlot;
    public Button ringRightSlot;
    public Button amuletSlot;
    public Button beltSlot;

    // Bag UI
    public GameObject bagSlotsParent;
    public GameObject bagSlotPrefab;
    public int totalBagSlots = 60; // Total number of slots in the grid

    private List<GameObject> bagSlotButtons = new List<GameObject>();

    public GameObject tooltip; // Tooltip GameObject
    public TextMeshProUGUI tooltipText; // Text component for tooltip details

    void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned to InventoryUI!");
            return;
        }

        // Subscribe to inventory events
        inventory.OnInventoryChanged += RefreshBagSlots;
        inventory.OnEquipmentChanged += RefreshEquippableSlots;

        // Initial setup
        GenerateBagSlots();
        RefreshEquippableSlots();
        RefreshBagSlots();

        tooltip.SetActive(false); // Hide tooltip initially
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RefreshBagSlots;
            inventory.OnEquipmentChanged -= RefreshEquippableSlots;
        }
    }

  private void GenerateBagSlots()
{
    for (int i = 0; i < totalBagSlots; i++)
    {
        GameObject slot = Instantiate(bagSlotPrefab, bagSlotsParent.transform);

        // Name the slot for easier identification during debugging
        slot.name = $"InventorySlot_{i}";

        bagSlotButtons.Add(slot);

        // Assign InventoryUI reference to SlotDropHandler
        SlotDropHandler dropHandler = slot.GetComponent<SlotDropHandler>();
        if (dropHandler != null)
        {
            dropHandler.inventoryUI = this;
        }
        else
        {
            Debug.LogError($"GenerateBagSlots: Slot {slot.name} is missing a SlotDropHandler component.");
        }
    }

    Debug.Log($"Generated {bagSlotButtons.Count} bag slots.");
}

    public void ShowTooltip(Item item, Vector2 position)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position;
        tooltipText.text = GetItemDetails(item);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    private string GetItemDetails(Item item)
    {
        string details = $"{item.ItemName}\n";
        foreach (var modifier in item.StatModifiers)
        {
            details += $"{modifier.StatName}: {modifier.Value}\n";
        }
        return details.Trim();
    }

    public void HandleItemDrop(Item draggedItem, SlotDropHandler targetSlot)
    {
        if (draggedItem == null || targetSlot == null)
        {
            Debug.LogError("Invalid drag-and-drop operation: draggedItem or targetSlot is null.");
            return;
        }

        int targetIndex = bagSlotButtons.IndexOf(targetSlot.gameObject);
        int draggedIndex = inventory.GetItemIndex(draggedItem);

        Debug.Log($"Dragged Item: {draggedItem?.ItemName}, Dragged Index: {draggedIndex}, Target Index: {targetIndex}");

        if (targetIndex >= 0 && draggedIndex >= 0 && targetIndex < inventory.GetBagContents().Count)
        {
            inventory.SwapItems(draggedIndex, targetIndex);
            RefreshBagSlots();
        }
        else
        {
            Debug.LogError($"Invalid indices for SwapItems. Dragged Index: {draggedIndex}, Target Index: {targetIndex}");
        }
    }

    public void HighlightSlot(SlotDropHandler slot, bool highlight)
    {
        Image slotImage = slot.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = highlight ? new Color(0.8f, 0.8f, 1f, 1f) : Color.white; // Highlight or reset
        }
    }

    public void RefreshEquippableSlots()
    {
        UpdateSlot(helmetSlot, EquipmentSlot.Helmet);
        UpdateSlot(chestSlot, EquipmentSlot.Chest);
        UpdateSlot(glovesSlot, EquipmentSlot.Gloves);
        UpdateSlot(bootsSlot, EquipmentSlot.Boots);
        UpdateSlot(weaponSlot, EquipmentSlot.Weapon);
        UpdateSlot(offhandSlot, EquipmentSlot.Offhand);
        UpdateSlot(ringLeftSlot, EquipmentSlot.RingLeft);
        UpdateSlot(ringRightSlot, EquipmentSlot.RingRight);
        UpdateSlot(amuletSlot, EquipmentSlot.Amulet);
        UpdateSlot(beltSlot, EquipmentSlot.Belt);
    }

    private void UpdateSlot(Button slotButton, EquipmentSlot slot)
    {
        if (slotButton == null) return;

        TextMeshProUGUI buttonText = slotButton.GetComponentInChildren<TextMeshProUGUI>();
        Image slotImage = slotButton.GetComponentInChildren<Image>();

        Item equippedItem = inventory.GetEquippedItem(slot);
        if (equippedItem != null)
        {
            buttonText.text = equippedItem.ItemName;
            slotImage.sprite = equippedItem.ItemImage;
            slotImage.enabled = true;
        }
        else
        {
            buttonText.text = "Empty";
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
    }

public void RefreshBagSlots()
{
    List<Item> bagContents = inventory.GetBagContents();

    for (int i = 0; i < bagSlotButtons.Count; i++)
    {
        GameObject slot = bagSlotButtons[i];
        if (slot == null)
        {
            Debug.LogError($"RefreshBagSlots: Slot at index {i} is null.");
            continue;
        }

        Transform itemImageTransform = slot.transform.Find("ItemImage");
        if (itemImageTransform == null)
        {
            Debug.LogError($"RefreshBagSlots: Slot {slot.name} (Index: {i}) is missing an ItemImage child.");
            continue;
        }

        Image itemImage = itemImageTransform.GetComponent<Image>();
        if (itemImage == null)
        {
            Debug.LogError($"RefreshBagSlots: ItemImage in slot {slot.name} (Index: {i}) is missing an Image component.");
            continue;
        }

        TextMeshProUGUI stackText = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (stackText == null)
        {
            Debug.LogError($"RefreshBagSlots: StackText is missing in slot {slot.name} (Index: {i}).");
            continue;
        }

        if (i < bagContents.Count && bagContents[i] != null)
        {
            Item item = bagContents[i];
            itemImage.sprite = item.ItemImage;
            itemImage.enabled = true;
            stackText.text = item.IsStackable ? $"x{item.CurrentStack}" : "";

            DraggableItem draggableItem = itemImageTransform.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.item = item;
            }
            else
            {
                Debug.LogError($"RefreshBagSlots: DraggableItem is missing on ItemImage in slot {slot.name} (Index: {i}).");
            }
        }
        else
        {
            // Empty slot
            itemImage.sprite = null;
            itemImage.enabled = false;
            stackText.text = "";

            DraggableItem draggableItem = itemImageTransform.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.item = null;
            }
        }
    }
}


    public float GetCanvasScale()
{
    Canvas canvas = GetComponentInParent<Canvas>();
    return canvas != null ? canvas.scaleFactor : 1f;
}
}
