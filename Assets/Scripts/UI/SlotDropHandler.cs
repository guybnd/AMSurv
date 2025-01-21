using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDropHandler : MonoBehaviour, IDropHandler
{
    public InventoryUI inventoryUI;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggedItem != null)
        {
            Debug.Log($"Item {draggedItem.item.ItemName} dropped on {name}");
            inventoryUI.HandleItemDrop(draggedItem.item, this); // Delegate drop handling to InventoryUI
        }
    }
}
