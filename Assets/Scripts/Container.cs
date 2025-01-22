using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private RectTransform _currentItem;
    private Color _originalColor;

    public int ID { get; set; } // Container ID.

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Save the original color of the container's background (assuming it has an Image component).
        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            _originalColor = image.color;
        }
    }

    public void PutInside(RectTransform rect, Container previousContainer)
    {
        if (_currentItem != null)
        {
            // If this container already has an item, swap the items.
            SwapItems(rect, previousContainer);
        }
        else
        {
            // If this container is empty, place the item inside.
            _currentItem = rect;
            rect.SetParent(transform); // Make the item a child of this container.
            rect.position = _rectTransform.position;

            if (previousContainer != null)
            {
                previousContainer.RemoveItem();
            }
        }
    }

    public void RemoveItem()
    {
        _currentItem = null;
    }

    public RectTransform GetCurrentItem()
    {
        return _currentItem;
    }

    private void SwapItems(RectTransform newItem, Container previousContainer)
    {
        var oldItem = _currentItem;

        // Place the new item in this container.
        _currentItem = newItem;
        newItem.SetParent(transform); // Make the new item a child of this container.
        newItem.position = _rectTransform.position;

        // Move the old item back to the previous container.
        previousContainer._currentItem = oldItem;
        oldItem.SetParent(previousContainer.transform); // Make the old item a child of the previous container.
        oldItem.position = previousContainer._rectTransform.position;

        // Update the Draggable references for both items.
        oldItem.GetComponent<Draggable>().CurrentContainer = previousContainer;
        newItem.GetComponent<Draggable>().CurrentContainer = this;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
    }

    private void Highlight(bool enable)
    {
        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = enable ? Color.yellow : _originalColor;
        }
    }
}
