using System.Collections.Generic;
using Ragnar.Util;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{


    // [Header("Player")]
    // public Player player;
    // public PlayerInventory playerInventory;
    [Header("Prefabs")]
    public GameObject lootPrefab;
    [Header("Cursors")]
    public Sprite defaultCursor;
    public Sprite pickupCursor;
    public Sprite attackCursor;

    private Toaster toaster;

    void Awake()
    {
        SetCursor(CursorType.Default);
        toaster = FindObjectOfType<Toaster>();
    }


    public void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Default:
                ResetCursor();
                break;
            case CursorType.Pickup:
                Cursor.SetCursor(pickupCursor?.texture ?? defaultCursor?.texture, Vector2.zero, CursorMode.Auto);

                break;
            case CursorType.Attack:
                Cursor.SetCursor(attackCursor?.texture ?? defaultCursor?.texture, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
    public void ResetCursor()
    {
        Cursor.SetCursor(defaultCursor?.texture, Vector2.zero, CursorMode.Auto);
    }


    public List<Loot> CreateLoot(Vector3 position, Item item, int amount)
    {

        var list = new List<Loot>(amount);
        while (amount > 0)
        {
            var go = Instantiate(lootPrefab, position, Quaternion.identity);
            var loot = go.GetComponent<Loot>();
            loot.item = item;
            loot.amount = 1;
            list.Add(loot);
            if (item.stackable)
            {
                loot.amount = amount;
                break;
            }
            else
            {
                amount--;
            }
        }
        return list;

    }
}