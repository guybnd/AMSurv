using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // For TextMeshPro

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private bool _isDragging = false;
    private Vector2 _savedPosition;

    public Container CurrentContainer { get; set; } // The container holding this item.
    private Transform _originalParent;

    private Container _hoveredContainer; // The container currently being hovered while dragging.

    [Header("Item Details")]
    public Item ItemData; // Reference to the ScriptableObject representing the item.
    public int CurrentStack = 1; // The current stack amount for this item.

    private Image _itemImage; // The UI image displaying the item's sprite.
    private TMP_Text _stackText; // The UI text displaying the stack count.

    private void Start()
    {

        _rectTransform = GetComponent<RectTransform>();
        _itemImage = GetComponent<Image>(); // Automatically fetch the Image component.
        _stackText = GetComponentInChildren<TMP_Text>(); // Find the TMP_Text in the child object.
            if (_stackText == null)
    {
        Debug.LogError($"TMP_Text component not found on {name} or its children!");
    }   

        if (ItemData != null)
        {
            InitializeItem(ItemData);
        }
    }

public void InitializeItem(Item item)
{
    ItemData = item;
    CurrentStack = Mathf.Clamp(item.CurrentStack, 1, item.MaxStack);

    // Update the item's image and reset its color.
    if (_itemImage != null && item.ItemImage != null)
    {
        _itemImage.sprite = item.ItemImage;
        _itemImage.color = Color.white; // Reset to white to ensure no tinting.
        _itemImage.enabled = true; // Ensure the image is visible.
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
                _stackText.gameObject.SetActive(true); // Show the stack count.
            }
            else
            {
                _stackText.gameObject.SetActive(false); // Hide the stack count if <= 1.
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;

        if (CurrentContainer != null)
        {
            _savedPosition = _rectTransform.anchoredPosition;

            // Remove highlight from the current container as dragging starts.
            CurrentContainer.Highlight(false);

            CurrentContainer.RemoveItem(); // Temporarily remove from the container.
        }

        _originalParent = transform.parent;
        transform.SetParent(transform.root, true); // Move to root canvas for dragging.
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;

        var container = DetectContainer();
        if (container != null)
        {
            if (container.GetCurrentItem() != null && container.GetCurrentItem().GetComponent<Draggable>().ItemData == ItemData && ItemData.IsStackable)
            {
                // Stack the items if they are stackable and the same type.
                StackItems(container.GetCurrentItem().GetComponent<Draggable>());
            }
            else
            {
                container.PutInside(_rectTransform, CurrentContainer);
                CurrentContainer = container;
            }
        }
        else
        {
            if (CurrentContainer != null)
            {
                CurrentContainer.PutInside(_rectTransform, null);
            }
            else
            {
                _rectTransform.anchoredPosition = _savedPosition;
            }
        }

        // Clear highlight from any hovered container when dropping.
        if (_hoveredContainer != null)
        {
            _hoveredContainer.Highlight(false);
            _hoveredContainer = null;
        }

        transform.SetParent(_originalParent, true); // Restore original parent.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isDragging && CurrentContainer != null)
        {
            CurrentContainer.Highlight(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isDragging && CurrentContainer != null)
        {
            CurrentContainer.Highlight(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
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
    }

    private void StackItems(Draggable target)
    {
        int availableSpace = target.ItemData.MaxStack - target.CurrentStack;

        if (availableSpace > 0)
        {
            int amountToTransfer = Mathf.Min(CurrentStack, availableSpace);
            target.CurrentStack += amountToTransfer;
            CurrentStack -= amountToTransfer;

            target.UpdateStackText();
            UpdateStackText();

            if (CurrentStack <= 0)
            {
                Destroy(gameObject); // Destroy this item if it's fully stacked.
            }
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
