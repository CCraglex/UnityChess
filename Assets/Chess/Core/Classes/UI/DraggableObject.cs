
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool ObjEnabled = false;
    private Vector2 mousePos;
    private Camera camera2D;
    protected RectTransform rect;
    protected bool wasDragAction;

    public virtual void Init() { }

    private void Awake()
    {
        camera2D = Camera.main;
        rect = transform as RectTransform;
        Enable();
        Init();
    }

    public void Enable()
        => ObjEnabled = true;
    public void Disable()
        => ObjEnabled = false;

    
    public void Select()
    {       
        print("Selected!");
        OnSelectExtras();
    }

    //Must be impelmented for onpointerup to work
    public void OnPointerDown(PointerEventData eventData) {}

    public void OnPointerUp(PointerEventData eventData)
    {

        if(!ObjEnabled)
            return;

        if (wasDragAction)
        {
            wasDragAction = false;
            return;
        }

        Select();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!ObjEnabled)
            return;

        wasDragAction = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!ObjEnabled)
            return;

        mousePos = Mouse.current.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform,mousePos,camera2D,out Vector2 localPoint);
        rect.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!ObjEnabled)
            return;

        wasDragAction = false;
        OnEndDragExtras();
    }

    public virtual void OnEndDragExtras(){ }
    public virtual void OnSelectExtras() { }
}