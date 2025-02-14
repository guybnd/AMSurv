using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,
                        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    private bool _isDragging = false;
    private Vector2 _savedPosition;

    public Container CurrentContainer { get; set; }
    private Transform _originalParent;
    private Container _hoveredContainer;

    [Header("Item Details")]
    public Item ItemData;
    public int CurrentStack = 1;

    private Image _itemImage;
    private TMP_Text _stackText;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _itemImage = GetComponent<Image>();
        _stackText = GetComponentInChildren<TMP_Text>(true);

        if (_stackText == null)
        {
            Debug.LogError($"TMP_Text component not found on {name} or its children!");
        }

        if (ItemData != null)
        {
            InitializeItem(ItemData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (CurrentContainer.GetContainerType() == ContainerType.Equipment)
            {
                // Send the item back to inventory
                var inventoryContainers = Object.FindObjectsByType<Container>(FindObjectsSortMode.None);
                foreach (var container in inventoryContainers)
                {
                    if (container.GetContainerType() == ContainerType.Inventory && container.GetCurrentItemTransform() == null)
                    {
                        bool success = container.PutInside(_rectTransform, CurrentContainer);
                        if (success)
                        {
                            CurrentContainer.RemoveItem();
                            CurrentContainer = container;
                        }
                        return;
                    }
                }

                Debug.LogWarning("No empty inventory slots available to send the item back.");
            }
            else if (CurrentContainer.GetContainerType() == ContainerType.Inventory && ItemData.Type != ItemType.BagItem)
            {
                // Try to equip the item from the inventory
                var equipmentContainers = Object.FindObjectsByType<Container>(FindObjectsSortMode.None);
                foreach (var container in equipmentContainers)
                {
                    if (container.GetContainerType() == ContainerType.Equipment && container.CanAcceptItem(ItemData) && container.GetCurrentItemTransform() == null)
                    {
                        bool success = container.PutInside(_rectTransform, CurrentContainer);
                        if (success)
                        {
                            CurrentContainer.RemoveItem();
                            CurrentContainer = container;
                        }
                        return;
                    }
                }

                Debug.LogWarning("No available equipment slots for this item.");
            }
        }
    }

    private void TryEquipItem()
    {
        if (ItemData.Type == ItemType.BagItem) return;

        var containers = Object.FindObjectsByType<Container>(FindObjectsSortMode.None);
        foreach (var container in containers)
        {
            // Skip inventory/bag containers
            if (container.GetContainerType() == ContainerType.Inventory) continue;

            if (container.CanAcceptItem(ItemData))
            {
                bool success = container.PutInside(_rectTransform, CurrentContainer);
                if (success)
                {
                    CurrentContainer = container;
                    return;
                }
            }
        }
    }

    public void InitializeItem(Item item)
    {
        ItemData = item;
        CurrentStack = Mathf.Clamp(item.CurrentStack, 1, item.MaxStack);

        if (_itemImage != null && item.ItemImage != null)
        {
            _itemImage.sprite = item.ItemImage;
            _itemImage.color = Color.white;
            _itemImage.enabled = true;
        }

        UpdateStackText();
    }

    public void UpdateStackText()
    {
        if (_stackText != null)
        {
            if (ItemData.IsStackable && CurrentStack > 1)
            {
                _stackText.text = CurrentStack.ToString();
                _stackText.gameObject.SetActive(true);
            }
            else
            {
                _stackText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = false; // Reset dragging state on pointer down

        if (CurrentContainer != null)
        {
            _savedPosition = _rectTransform.anchoredPosition;
            CurrentContainer.Highlight(false);
        }

        _originalParent = transform.parent;
        transform.SetParent(transform.root, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            // If not dragging, reset position and exit
            transform.SetParent(_originalParent, true);
            return;
        }

        _isDragging = false;

        // Detect the container under the pointer
        var container = DetectContainer();
        if (container == null)
        {
            Debug.LogWarning("No valid container detected. Returning item to its original position.");
            ReturnToOriginalPosition();
            transform.SetParent(_originalParent, true);
            return;
        }

        // Check if the container can accept the item
        if (!container.CanAcceptItem(ItemData) || !container.PutInside(_rectTransform, CurrentContainer))
        {
            Debug.LogWarning("Container cannot accept item. Returning item to its original position.");
            ReturnToOriginalPosition();
        }
        else
        {
            // Update the item's current container
            if (CurrentContainer != null)
            {
                CurrentContainer.RemoveItem();
            }
            CurrentContainer = container;
        }

        transform.SetParent(_originalParent, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isDragging && CurrentContainer != null)
        {
            CurrentContainer.Highlight(true);
            CurrentContainer.ShowTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isDragging && CurrentContainer != null)
        {
            CurrentContainer.Highlight(false);
            CurrentContainer.HideTooltip();
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true; // Mark as dragging
        _rectTransform.position = Input.mousePosition;

        var container = DetectContainer();
        if (container != _hoveredContainer)
        {
            if (_hoveredContainer != null)
            {
                _hoveredContainer.Highlight(false);
            }

            if (container != null)
            {
                container.Highlight(true);
            }

            _hoveredContainer = container;
        }
    }

    private bool StackItems(Draggable target)
    {
        int availableSpace = target.ItemData.MaxStack - target.CurrentStack;

        if (availableSpace > 0)
        {
            int amountToTransfer = Mathf.Min(CurrentStack, availableSpace);
            target.CurrentStack += amountToTransfer;
            CurrentStack -= amountToTransfer;

            target.UpdateStackText();
            UpdateStackText();

            return CurrentStack <= 0;
        }

        return false;
    }

    private void ReturnToOriginalPosition()
    {
        _rectTransform.anchoredPosition = _savedPosition;

        if (CurrentContainer != null)
        {
            CurrentContainer.SetCurrentItem(_rectTransform, ItemData);
        }
    }

    private Container DetectContainer()
    {
        var pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (var result in raycastResults)
        {
            var container = result.gameObject.GetComponent<Container>();
            if (container != null)
            {
                return container;
            }
        }

        return null;
    }
}