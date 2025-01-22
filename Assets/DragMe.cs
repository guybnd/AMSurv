using UnityEngine;
using UnityEngine.EventSystems;

public class DragMe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    
    public RectTransform rect;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        Debug.Log("Mouse button pressed down on " + gameObject.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        Debug.Log("Mouse button released on " + gameObject.name);
    }

    private void Update()
    {
        if(_isDragging)
        {
            Debug.Log("Ma");
        }
    }
}
