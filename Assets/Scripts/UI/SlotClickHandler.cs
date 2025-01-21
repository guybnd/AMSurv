using UnityEngine;
using UnityEngine.EventSystems;

public class SlotClickHandler : MonoBehaviour, IPointerClickHandler
{
    public InventoryUI inventoryUI;
    public DraggableItem draggableItem;

    private void Start()
    {
        // Debug.Log($"SlotClickHandler attached to {gameObject.name}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Left-click recognized on slot: {gameObject.name}");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"Right-click recognized on slot: {gameObject.name}");
        }
        else
        {
            Debug.Log($"Other click recognized on slot: {gameObject.name}");
        }
    }
}
