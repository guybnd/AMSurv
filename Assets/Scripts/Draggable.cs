using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    private RectTransform _rectTransform;
    private Vector2 _savedPosition;

    public Container CurrentContainer { get; set; } // The current container holding this item.

    private Image _image;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;

        if (CurrentContainer != null)
        {
            _savedPosition = _rectTransform.anchoredPosition;

            // Temporarily remove the item from its current container.
            CurrentContainer.RemoveItem();
        }

        // Disable raycast target on the item's image so it doesn't block raycasts.
        if (_image != null)
        {
            _image.raycastTarget = false;
        }
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
            // If no valid container is found, return to the original container.
            if (CurrentContainer != null)
            {
                CurrentContainer.PutInside(_rectTransform, null);
            }
            else
            {
                _rectTransform.anchoredPosition = _savedPosition;
            }
        }

        // Re-enable raycast target on the item's image.
        if (_image != null)
        {
            _image.raycastTarget = true;
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _savedPosition = _rectTransform.anchoredPosition;

        // Cache the Image component.
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (_isDragging)
        {
            _rectTransform.position = Input.mousePosition;
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
