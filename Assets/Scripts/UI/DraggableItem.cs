using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item; // The item this UI element represents
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent; // Store the original parent slot
    private InventoryUI inventoryUI;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryUI = Object.FindAnyObjectByType<InventoryUI>();

        if (canvasGroup == null)
            Debug.LogError($"CanvasGroup is missing on {gameObject.name}. Please add one.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Begin drag for item: {item?.ItemName}");

        originalParent = transform.parent; // Store the current parent (slot)
        transform.SetParent(inventoryUI.transform, true); // Temporarily reparent to inventory UI

        canvasGroup.blocksRaycasts = false; // Allow detection of drop areas
        canvasGroup.alpha = 0.8f; // Make the item semi-transparent during drag

        inventoryUI.HideTooltip(); // Hide tooltip during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / inventoryUI.GetCanvasScale(); // Move item with drag
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End drag for item: {item?.ItemName}");

        canvasGroup.blocksRaycasts = true; // Re-enable raycast blocking
        canvasGroup.alpha = 1f; // Reset transparency

        // Check if dropped on a valid slot
        SlotDropHandler dropHandler = eventData.pointerEnter?.GetComponent<SlotDropHandler>();
        if (dropHandler != null)
        {
            Debug.Log($"Dropped on slot: {dropHandler.name}");
            inventoryUI.HandleItemDrop(item, dropHandler);
        }
        else
        {
            Debug.LogWarning("Dropped outside valid slots.");
            // Return item to original slot if no valid drop occurred
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
