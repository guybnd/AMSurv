using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item; // The item this UI element represents
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventoryUI inventoryUI;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError($"CanvasGroup is missing on {gameObject.name}. Please add one.");
        }

        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Begin drag for item: {item?.ItemName}");

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false; // Allow drop detection
        }

        // Detach for dragging
        originalParent = transform.parent;
        transform.SetParent(inventoryUI.transform, true);
        inventoryUI.HideTooltip();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"Dragging item: {item?.ItemName}");
        rectTransform.anchoredPosition += eventData.delta / inventoryUI.GetCanvasScale();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End drag for item: {item?.ItemName}");

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true; // Re-enable raycast blocking
        }

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
        }

        // Return to original parent if no valid drop occurred
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
