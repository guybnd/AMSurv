using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private bool _isDragging = false;
    private Vector2 _savedPosition;

    public Container CurrentContainer { get; set; } // The container holding this item.
    private Transform _originalParent;

    private Container _hoveredContainer; // The container currently being hovered while dragging.

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
            container.PutInside(_rectTransform, CurrentContainer);
            CurrentContainer = container;
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
        // Highlight the container of this draggable item (if not dragging).
        if (!_isDragging && CurrentContainer != null)
        {
            CurrentContainer.Highlight(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Remove highlight from the container of this draggable item (if not dragging).
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

            // Detect the container currently being hovered during dragging.
            var container = DetectContainer();
            if (container != _hoveredContainer)
            {
                // Remove highlight from the previously hovered container.
                if (_hoveredContainer != null)
                {
                    _hoveredContainer.Highlight(false);
                }

                // Highlight the new container.
                if (container != null)
                {
                    container.Highlight(true);
                }

                _hoveredContainer = container;
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

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _savedPosition = _rectTransform.anchoredPosition;
    }
}
