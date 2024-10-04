using System.Collections;
using System.Collections.Generic;
public class ItemsStash : IItemsStash
{
    public int capacity
    {
        get; protected set;
    }
    public int totalWeight
    {
        get; protected set;
    }

    List<Item> items = new List<Item>();

    public ItemsStash(int capacity)
    {
        this.capacity = capacity;
    }

    public ItemsStash(int capacity, IEnumerable<Item> items)
    {
        this.capacity = capacity;
        this.items.AddRange(items);
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }
    }

    public bool Put(Item item)
    {
        if (!Fits(item)) return false;
        items.Add(item);
        totalWeight += item.weight;
        return true;
    }


    public bool Fits(Item item)
    {
        return Fits(item, 1);
    }

    public bool Fits(Item item, int amount)
    {
        return (totalWeight + (item.weight * amount)) <= capacity;
    }


    public bool Has(Item item)
    {
        var index = items.IndexOf(item);
        return index > 0;
    }

    public bool Take(Item item)
    {
        var index = items.IndexOf(item);
        if (index < 0) return false;
        totalWeight -= item.weight;
        return items[index];
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
}
