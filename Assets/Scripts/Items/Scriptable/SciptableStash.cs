using UnityEngine;

[CreateAssetMenu(fileName = "Stash", menuName = "Items/Stash", order = 1)]
public class ScriptableStash : ScriptableObject
{
    public Item[] items;
}