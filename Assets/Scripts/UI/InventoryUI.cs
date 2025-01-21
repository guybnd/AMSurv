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

    public GameObject tooltip; // Assign the Tooltip prefab in the Inspector
    public TextMeshProUGUI tooltipText; // Text to display item details
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

        // Initial refresh
        GenerateBagSlots();
        RefreshEquippableSlots();
        RefreshBagSlots();


         tooltip.SetActive(false); // Hide the tooltip initially
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

        public float GetCanvasScale()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null ? canvas.scaleFactor : 1f;
    }

    // Generate the fixed number of bag slots
    private void GenerateBagSlots()
    {
        for (int i = 0; i < totalBagSlots; i++)
        {
            GameObject slot = Instantiate(bagSlotPrefab, bagSlotsParent.transform);
            bagSlotButtons.Add(slot);

            // Initialize empty slot visuals
            Image itemImage = slot.transform.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = null;
            itemImage.enabled = false;

            TextMeshProUGUI stackText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (stackText != null)
            {
                stackText.text = "";
            }
        }
    }
        public void ShowTooltip(Item item, Vector2 position)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position; // Position the tooltip near the mouse
        tooltipText.text = GetItemDetails(item); // Populate the tooltip with item stats
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

    // Attach these to bag slot events
    public void OnSlotHover(Item item)
    {
        Vector2 mousePosition = Input.mousePosition;
        ShowTooltip(item, mousePosition);
    }

    public void OnSlotExit()
    {
        HideTooltip();
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

    if (targetIndex >= 0 && draggedIndex >= 0)
    {
        inventory.SwapItems(draggedIndex, targetIndex);
        RefreshBagSlots(); // Update the UI after the swap
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

public void HighlightValidSlots(Item item, bool highlight)
{
    foreach (var button in bagSlotButtons)
    {
        SlotDropHandler slot = button.GetComponent<SlotDropHandler>();
        if (slot != null)
        {
            HighlightSlot(slot, highlight); // Add logic to validate specific slots if needed
        }
    }
}

    


    // Update the visuals for equippable slots
    public void RefreshEquippableSlots()
    {
        Debug.Log("Refreshing equippable slots...");

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
        if (slotButton == null)
        {
            Debug.LogError($"SlotButton for {slot} is not assigned!");
            return;
        }

        TextMeshProUGUI buttonText = slotButton.GetComponentInChildren<TextMeshProUGUI>();
        Image slotImage = slotButton.GetComponentInChildren<Image>();

        if (buttonText == null || slotImage == null)
        {
            Debug.LogError($"Missing components in {slotButton.name}! Ensure it has TextMeshProUGUI and Image.");
            return;
        }

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

    // Refresh the bag slots with current inventory contents
public void RefreshBagSlots()
{
    List<Item> bagContents = inventory.GetBagContents();

    for (int i = 0; i < bagSlotButtons.Count; i++)
    {
        GameObject slot = bagSlotButtons[i];
        Transform itemImageTransform = slot.transform.Find("ItemImage");

        if (i < bagContents.Count)
        {
            // Assign item data to the slot
            Item item = bagContents[i];
            Image itemImage = itemImageTransform.GetComponent<Image>();
            itemImage.sprite = item.ItemImage;
            itemImage.enabled = true;

            TextMeshProUGUI stackText = slot.GetComponentInChildren<TextMeshProUGUI>();
            stackText.text = item.IsStackable ? $"x{item.CurrentStack}" : "";

            DraggableItem draggableItem = itemImageTransform.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.item = i < bagContents.Count ? bagContents[i] : null;
            }
        }
        else
        {
            // Empty slot
            Image itemImage = itemImageTransform.GetComponent<Image>();
            itemImage.sprite = null;
            itemImage.enabled = false;

            TextMeshProUGUI stackText = slot.GetComponentInChildren<TextMeshProUGUI>();
            stackText.text = "";

            DraggableItem draggableItem = itemImageTransform.GetComponent<DraggableItem>();
            draggableItem.item = null; // Clear the item from the draggable component
        }
    }
}



    private void OnBagSlotClicked(Item item)
    {
        Debug.Log($"Clicked on {item.ItemName}");
        // Add logic to equip or use the item
    }



    public void OnSlotRightClick(Item item)
{
    if (inventory.TryEquipItem(item))
    {
        RefreshBagSlots();
        RefreshEquippableSlots();
    }
}
}
