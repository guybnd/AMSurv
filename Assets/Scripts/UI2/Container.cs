using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemTooltip tooltip; // Reference to the tooltip component

    private RectTransform _rectTransform;
    private RectTransform _currentItemTransform;
    private Item _currentItem; // New field to store the actual Item data
    private Color _originalColor;

    public enum ContainerType
    {
        Inventory,
        Equipment,
        Stash,
        Loot
    }
    public enum EquipmentSlotType
    {
        Weapon,
        Offhand,
        Helmet,
        Gloves,
        Boots,
        Chest,
        LeftRing,
        RightRing,
        Amulet,
        Belt,
        None
    }

    public int ID { get; set; }

    [SerializeField] private ContainerType _containerType = ContainerType.Inventory;
    [SerializeField] private EquipmentSlotType _equipmentSlotType = EquipmentSlotType.None;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            _originalColor = image.color;
        }

        // If tooltip reference is not set, try to find it in the scene
        if (tooltip == null)
        {
            if (tooltip == null)
            {
                tooltip = FindFirstObjectByType<ItemTooltip>();
            }
            tooltip = FindFirstObjectByType<ItemTooltip>();
        }
    }

    // New method to set both the transform and item data
    public void SetCurrentItem(RectTransform itemTransform, Item itemData)
    {
        _currentItemTransform = itemTransform;
        _currentItem = itemData;
    }

    public void PutInside(RectTransform rect, Container previousContainer)
    {
        if (_currentItemTransform != null)
        {
            // If this container already has an item, swap the items
            SwapItems(rect, previousContainer);
        }
        else
        {
            // If this container is empty, place the item inside
            _currentItemTransform = rect;
            rect.position = _rectTransform.position;

            // Get the item data from the Draggable component
            var draggable = rect.GetComponent<Draggable>();
            if (draggable != null)
            {
                _currentItem = draggable.ItemData;
            }

            if (previousContainer != null)
            {
                previousContainer.RemoveItem();
            }
        }
    }

    public void RemoveItem()
    {
        _currentItemTransform = null;
        _currentItem = null;
    }

    public RectTransform GetCurrentItemTransform()
    {
        return _currentItemTransform;
    }

    public Item GetCurrentItem()
    {
        return _currentItem;
    }

    private void SwapItems(RectTransform newItem, Container previousContainer)
    {
        var oldItemTransform = _currentItemTransform;
        var oldItemData = _currentItem;

        // Get the new item's data from its Draggable component
        var newDraggable = newItem.GetComponent<Draggable>();

        // Place the new item in this container
        _currentItemTransform = newItem;
        _currentItem = newDraggable.ItemData;
        newItem.position = _rectTransform.position;

        // Move the old item back to the previous container
        previousContainer._currentItemTransform = oldItemTransform;
        previousContainer._currentItem = oldItemData;
        oldItemTransform.position = previousContainer._rectTransform.position;

        // Update the Draggable references
        var oldDraggable = oldItemTransform.GetComponent<Draggable>();
        if (oldDraggable != null)
        {
            oldDraggable.CurrentContainer = previousContainer;
        }
        if (newDraggable != null)
        {
            newDraggable.CurrentContainer = this;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(true);
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
        HideTooltip();
    }

    public void ShowTooltip()
    {
        Debug.Log($"Container: ShowTooltip - Tooltip Component: {(tooltip != null ? "exists" : "null")} - Current Item: {(_currentItem != null ? _currentItem.ItemName : "null")}");
        if (_currentItem != null && tooltip != null)
        {
            tooltip.ShowTooltip(_currentItem, Input.mousePosition);
        }
    }

    public void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }

    public void Highlight(bool enable)
    {
        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = enable ? Color.yellow : _originalColor;
        }
    }
}