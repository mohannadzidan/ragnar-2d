using UnityEngine;

public class Item : ScriptableObject
{
    public new string name;
    public ItemType type;
    public int weight;

    public Sprite lootIcon;

    public Sprite inventoryIcon;
    public bool stackable;
}