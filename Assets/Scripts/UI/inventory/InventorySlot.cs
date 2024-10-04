
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : Slot<Item>
{
    public InventorySlotType slotType;


    protected override void OnClear()
    {
        // throw new NotImplementedException();
    }

    protected override void OnData(Item item)
    {
        // throw new NotImplementedException();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        InventoryItem item = droppedObject.GetComponent<InventoryItem>();
        // Debug.Log($"item={!!item} droppedObject={!!droppedObject} content={!!content} sameCell={content == item} sameData={content?.data == item?.data}");
        if (!item || !droppedObject) return;
        if (content && content != item && content.data == item.data && WillAllow(item) && WillStack(item))
        {
            var existing = (InventoryItem)content;
            existing.Stack(item);
        }
        else if (!content && WillAllow(item))
        {
            SetContent(item);
        }
    }

    protected override bool WillAllow(Draggable<Item> draggable)
    {
        if (draggable.GetType() != typeof(InventoryItem)) return false;
        var item = (InventoryItem)draggable;
        return (slotType & item.slotType) == item.slotType;
    }

    protected override bool WillStack(Draggable<Item> draggable)
    {
        if (draggable.GetType() != typeof(InventoryItem)) return false;
        var item = (InventoryItem)draggable;
        return item.data.stackable;
    }
}
