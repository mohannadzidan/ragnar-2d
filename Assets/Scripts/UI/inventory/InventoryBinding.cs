using System.Collections.Generic;
using Ragnar.Util;
using TMPro;
using UnityEngine;


public class InventoryBinding : Singleton<InventoryBinding>
{

    // Start is called before the first frame update
    public GameObject inventoryItemPrefab;

    List<InventorySlot> slots;

    PlayerInventory inventory => PlayerInventory.current;
    Player player;
    TMP_Text capacityText;

    bool syncEnabled = true;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        slots = new(transform.GetComponentsInChildren<InventorySlot>());
        player = GameObject.Find("Player").GetComponent<Player>();
        capacityText = transform.Find("Capacity").GetComponent<TMP_Text>();
        foreach (var item in inventory)
        {
            OnItemAdded(item);
        }
        inventory.OnItemAdded += OnItemAdded;
        inventory.OnItemTaken += OnItemTaken;
        UpdateCapacityText();
    }

    void OnItemAdded(Item item)
    {
        if (!syncEnabled) return;
        InventorySlot slot;
        if (item.stackable && (slot = slots.Find(slot => slot.content && slot.content.data == item)))
        {
            (slot.content as InventoryItem).Stack(1);
        }
        else
        {
            slot = slots.Find(slot => slot.content == null);
            slot.SetContent(CreateInventoryItem(item));
        }
        UpdateCapacityText();
        UpdateCapacityText();
        UpdateCapacityText();
    }
    void OnItemTaken(Item item)
    {
        if (!syncEnabled) return;
        var slot = slots.Find(slot => slot.content?.data == item);
        var content = (InventoryItem)slot.content;
        content.Take(1);
        UpdateCapacityText();
    }

    public void RequestDropItem(InventoryItem item)
    {
        return ;
        syncEnabled = false;
        var taken = 0;
        for (var i = 0; i < item.amount; i++)
        {
            if (inventory.Take(item.data))
                taken++;
            else
                break;
        }
        item.Take(taken);
        GameManager.current.CreateLoot(player.transform.position, item.data, taken);
        UpdateCapacityText();
        syncEnabled = true;
    }

    public void RequestUseItem(InventoryItem item)
    {
        syncEnabled = false;
        if (player.Use(item.data))
        {
            inventory.Take(item.data);
            item.Take(1);
        }
        UpdateCapacityText();
        syncEnabled = true;
    }

    InventoryItem CreateInventoryItem(Item item)
    {
        var inventoryItem = Instantiate(inventoryItemPrefab, transform).GetComponent<InventoryItem>();
        inventoryItem.data = item;
        // inventoryItem.OnUseRequested += OnItemUse;
        // inventoryItem.OnDropRequested += OnItemDrop;
        return inventoryItem;
    }

    void UpdateCapacityText()
    {
        Debug.Log($"inventory: {inventory.totalWeight}/{inventory.capacity}");
        capacityText.text = $"{inventory.totalWeight}/{inventory.capacity}";
    }

}
