using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InventoryItem : Draggable<Item>, IPointerClickHandler
{

    public InventorySlotType slotType;
    int _amount = 1;

    public int amount
    {
        get => _amount;
        private set
        {
            _amount = value;
            if (amountElement && value == 1)
            {
                amountElement.gameObject.SetActive(false);
            }
            else if (amountElement)
            {
                amountElement.gameObject.SetActive(true);
                amountElement.text = value.ToString();
            }
        }
    }

    TMP_Text amountElement;
    new void Start()
    {
        base.Start();
        amountElement = transform.GetComponentInChildren<TMP_Text>();
        amount = _amount;
    }

    public void Stack(InventoryItem other)
    {
        other.SetSlot(null);
        amount += other.amount;
        Destroy(other.gameObject);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryBinding.current.RequestUseItem(this);
        }
    }

    public void Use()
    {
        if (amount == 1)
        {
            Destroy(gameObject);
        }
        else
        {
            amount--;
        }
    }

    private Slot<Item> dragStartSlot = null;
    private InventoryItem splitItem = null;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        dragStartSlot = slot;
        if (Input.GetKey(KeyCode.LeftControl) && amount > 1)
        {
            var go = Instantiate(gameObject);
            var half = amount / 2;
            var remaining = amount - half;
            amount = half;
            splitItem = go.GetComponent<InventoryItem>();
            splitItem.amount = remaining;
            splitItem.data = data;
            var slot = this.slot;
            SetSlot(null);
            splitItem.SetSlot(slot);

        }
    }


    public void Take(int amount)
    {
        this.amount -= amount;
        if (this.amount <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void Stack(int amount)
    {

        this.amount += amount;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            transform.SetParent(slot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            InventoryBinding.current.RequestDropItem(this);
        }
        else if (splitItem && !slot)
        {
            var slot = splitItem.slot;
            amount += splitItem.amount;
            SetSlot(slot);
            Destroy(splitItem);
        }
        else if (!slot && dragStartSlot)
        {
            SetSlot(dragStartSlot);
        }
        splitItem = null;
        dragStartSlot = null;
        base.OnEndDrag(eventData);
    }
}
