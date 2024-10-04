using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Slot<T> : MonoBehaviour, IDropHandler
{
    public Draggable<T> content { get; private set; }

    public virtual void OnDrop(PointerEventData eventData)
    {
        // Get the draggable object
        GameObject droppedObject = eventData.pointerDrag;
        Draggable<T> draggable = droppedObject.GetComponent<Draggable<T>>();
        if (!draggable || !droppedObject) return;
        if (!content && WillAllow(draggable))
        {
            SetContent(draggable);
        }
    }

    public void SetContent(Draggable<T> newContent)
    {
        // Debug.Log($"SetContent current={content?.ToString() ?? "null"}  current.slot={content?.slot?.ToString() ?? "null"} new={newContent?.ToString() ?? "null"} new.slot={newContent?.slot?.ToString() ?? "null"}  sameContent={content == newContent} sameSlot={content?.slot == newContent?.slot}");
        if (content == newContent) return;

        if (content)
        {
            var oldContent = content;
            content = newContent;
            // Debug.Log("SetContent oldContent.SetSlot(null)");
            oldContent.SetSlot(null);
        }
        content = newContent;
        if (newContent != null && newContent.slot != this)
        {
            // Debug.Log("SetContent newContent.SetSlot(null)");
            newContent.SetSlot(this);
        }

        if (content)
        {
            OnData(content.data);
        }
        else
        {
            OnClear();
        }
    }

    protected abstract void OnData(T draggable);

    protected abstract void OnClear();

    protected virtual bool WillAllow(Draggable<T> draggable)
    {
        return true;
    }
    protected virtual bool WillStack(Draggable<T> draggable)
    {
        return false;
    }
}
