using System;
using System.Collections;
using System.Collections.Generic;
using Ragnar.Util;
using UnityEngine;

public class PlayerInventory : Singleton<PlayerInventory>, IItemsStash
{


    public event Action<Item> OnItemTaken;
    public event Action<Item> OnItemAdded;
    public int capacity;
    public int totalWeight => stash.totalWeight;
    public int remainingWeight => stash.capacity - stash.totalWeight;

    ItemsStash stash;


    void Awake()
    {
        current = this;
        stash = new ItemsStash(capacity);
    }

    public bool Put(Item item)
    {
        var result = stash.Put(item);
        if (result) OnItemAdded?.Invoke(item);
        return result;
    }

    public int GetCapacity()
    {
        return stash.capacity;
    }


    public bool Fits(Item item)
    {
        return Fits(item, 1);
    }

    public bool Fits(Item item, int amount)
    {
        return stash.Fits(item, amount);
    }


    public bool Has(Item item)
    {
        return stash.Has(item);
    }

    public bool Take(Item item)
    {

        var result = stash.Take(item);
        if (result) OnItemTaken?.Invoke(item);
        return result;
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return stash.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return stash.GetEnumerator();
    }
}
