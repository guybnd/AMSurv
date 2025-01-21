using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDropHandler : MonoBehaviour, IDropHandler
{
    public InventoryUI inventoryUI;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggedItem != null)
        {
            inventoryUI.HandleItemDrop(draggedItem.item, this);
        }
    }
}
