using System.Collections.Generic;

public interface IItemsStash : IEnumerable<Item>
{
    public bool Put(Item item);

    public bool Fits(Item item);
    public bool Fits(Item item, int amount);

    public bool Has(Item item);

    public bool Take(Item item);
}
