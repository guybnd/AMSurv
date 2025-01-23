using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        _isDragging = true;

        if (CurrentContainer != null)
        {
            _savedPosition = _rectTransform.anchoredPosition;
            CurrentContainer.Highlight(false);
            CurrentContainer.RemoveItem();
        }

        _originalParent = transform.parent;
        transform.SetParent(transform.root, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;

        var container = DetectContainer();
        if (container != null)
        {
            // Get the RectTransform of the current item in the container
            RectTransform currentItemTransform = container.GetCurrentItemTransform();

            if (currentItemTransform != null)
            {
                // Get the Draggable component from the current item
                Draggable targetDraggable = currentItemTransform.GetComponent<Draggable>();

                if (targetDraggable != null && targetDraggable.ItemData == ItemData && ItemData.IsStackable)
                {
                    // Attempt to stack the items
                    bool fullyStacked = StackItems(targetDraggable);

                    if (!fullyStacked)
                    {
                        ReturnToOriginalPosition();
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    container.PutInside(_rectTransform, CurrentContainer);
                    CurrentContainer = container;
                }
            }
            else
            {
                container.PutInside(_rectTransform, CurrentContainer);
                CurrentContainer = container;
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }

        if (_hoveredContainer != null)
        {
            _hoveredContainer.Highlight(false);
            _hoveredContainer = null;
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