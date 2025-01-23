using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum ContainerType
{
    Inventory,
    Equipment,
    Stash,
    Loot
}

public class Container : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemTooltip tooltip;
    public ContainerType GetContainerType() => _containerType;
    private RectTransform _rectTransform;
    private RectTransform _currentItemTransform;
    private Item _currentItem;
    private Color _originalColor;


    public int ID { get; set; }

    [SerializeField] public ContainerType _containerType = ContainerType.Inventory;

    [SerializeField] public ItemType _acceptedItemType = ItemType.BagItem;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            _originalColor = image.color;
        }

        if (tooltip == null)
        {
            tooltip = FindFirstObjectByType<ItemTooltip>();
        }
    }

    public bool CanAcceptItem(Item item)
    {
        if (item == null) return false;

        switch (_containerType)
        {
            case ContainerType.Inventory:
            case ContainerType.Stash:
            case ContainerType.Loot:
                return true; // These containers accept any item

            case ContainerType.Equipment:
                // Check if the item type matches the accepted type
                return item.Type == _acceptedItemType;

            default:
                return false;
        }
    }

public bool PutInside(RectTransform rect, Container previousContainer)
    {
        var draggable = rect.GetComponent<Draggable>();
        if (draggable == null || draggable.ItemData == null)
        {
            Debug.Log($"Container: Invalid item being placed");
            return false;
        }

        if (!CanAcceptItem(draggable.ItemData))
        {
            Debug.Log($"Container: Cannot accept item of type {draggable.ItemData.Type}");
            return false;
        }

        if (_currentItemTransform != null)
        {
            var currentDraggable = _currentItemTransform.GetComponent<Draggable>();
            if (currentDraggable != null && !previousContainer.CanAcceptItem(currentDraggable.ItemData))
            {
                Debug.Log($"Container: Cannot swap items - destination container cannot accept current item");
                return false;
            }
            SwapItems(rect, previousContainer);
        }
        else
        {
            _currentItemTransform = rect;
            rect.position = _rectTransform.position;
            _currentItem = draggable.ItemData;

            if (previousContainer != null)
            {
                previousContainer.RemoveItem();
            }
        }

        return true;
    }

    public void SetCurrentItem(RectTransform itemTransform, Item itemData)
    {
        _currentItemTransform = itemTransform;
        _currentItem = itemData;
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

        var newDraggable = newItem.GetComponent<Draggable>();

        _currentItemTransform = newItem;
        _currentItem = newDraggable.ItemData;
        newItem.position = _rectTransform.position;

        previousContainer._currentItemTransform = oldItemTransform;
        previousContainer._currentItem = oldItemData;
        oldItemTransform.position = previousContainer._rectTransform.position;

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