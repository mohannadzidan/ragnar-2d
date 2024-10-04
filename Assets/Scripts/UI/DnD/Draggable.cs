using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Draggable<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public T data;
    private Vector3 startPosition;
    private Image image;
    public Slot<T> slot { get; private set; }

    private Slot<T> startedWithSlot = null;

    protected void Start()
    {

        image = GetComponent<Image>();
        image.raycastTarget = true;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // Save the starting position in case we need to return the object back
        startPosition = transform.position;

        // Save the parent to reattach the object later if dropped outside any slot
        startedWithSlot = slot;

        // Optional: Set the draggable object to appear on top of others by moving it to a higher layer in the hierarchy
        transform.SetParent(transform.root);
        image.raycastTarget = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // Move the object along with the mouse pointer
        transform.position = Input.mousePosition;

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        if (startedWithSlot == slot)
        {
            transform.position = startPosition;
        }
        startedWithSlot = null;
    }

    public void SetSlot(Slot<T> newSlot)
    {
        // Debug.Log($"SetSlot current={slot?.ToString() ?? "null"}  current.content={slot?.content?.ToString() ?? "null"} new={newSlot?.ToString() ?? "null"} new.content={newSlot?.content?.ToString() ?? "null"} sameContent={slot?.content == newSlot?.content} sameSlot={slot == newSlot}");
        if (slot == newSlot) return;

        if (slot != null)
        {
            var oldSlot = slot;
            slot = newSlot;
            // Debug.Log("SetSlot newSlot.SetContent(null)");
            oldSlot.SetContent(null);
        }
        slot = newSlot;
        if (newSlot != null && newSlot.content != this)
        {
            // Debug.Log("SetSlot newSlot.SetContent(this)");
            newSlot.SetContent(this);
        }
        if (slot)
        {
            transform.SetParent(slot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }

}
