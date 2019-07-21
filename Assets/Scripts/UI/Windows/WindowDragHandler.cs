using UnityEngine;
using UnityEngine.EventSystems;

//Draggable Code: https://dev.to/matthewodle/simple-ui-element-dragging-script-in-unity-c-450p
[RequireComponent(typeof(BasicWindow))]
public class WindowDragHandler : EventTrigger
{
    private bool dragging;
    private Vector2 clickOffset;

    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x - clickOffset.x, Input.mousePosition.y - clickOffset.y);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        clickOffset = new Vector2(Input.mousePosition.x - transform.position.x, Input.mousePosition.y - transform.position.y);
        transform.SetAsLastSibling();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        clickOffset = Vector2.zero;
    }

}
