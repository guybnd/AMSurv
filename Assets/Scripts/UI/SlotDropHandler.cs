using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDropHandler : MonoBehaviour, IDropHandler
{
    public InventoryUI inventoryUI;

public void OnDrop(PointerEventData eventData)
{
    if (inventoryUI == null)
    {
        Debug.LogError($"SlotDropHandler: InventoryUI is not assigned on {name}");
        return;
    }

    if (eventData.pointerDrag == null)
    {
        Debug.LogError($"SlotDropHandler: PointerDrag is null for slot {name}");
        return;
    }

    DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
    if (draggedItem == null)
    {
        Debug.LogError($"SlotDropHandler: PointerDrag does not have a DraggableItem component for slot {name}");
        return;
    }

    if (draggedItem.item == null)
    {
        Debug.LogError($"SlotDropHandler: DraggedItem does not have an item assigned for slot {name}");
        return;
    }

    Debug.Log($"Item {draggedItem.item.ItemName} dropped on {name}");
    inventoryUI.HandleItemDrop(draggedItem.item, this);
}
}