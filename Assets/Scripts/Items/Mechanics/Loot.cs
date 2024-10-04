using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Loot : MonoBehaviour
{


    public Item item;
    public int amount = 1;

    private new SpriteRenderer renderer;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        tag = "Loot";
        renderer.sprite = item.lootIcon;

    }
    public IEnumerable<Item> Pickup(int amount)
    {
        var pickAmount = Mathf.Min(amount, this.amount);
        this.amount -= pickAmount;
        if (this.amount == 0) Destroy(gameObject);
        return GetItems(pickAmount);
    }

    public IEnumerable<Item> Pickup()
    {
        return Pickup(amount);
    }

    IEnumerable<Item> GetItems(int amount)
    {
        while (amount > 0)
        {
            yield return item;
            amount--;
        }
    }

}
