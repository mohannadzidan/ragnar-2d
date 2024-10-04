using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Items/LootTable", order = 1)]
public class LootTable : ScriptableObject
{
    [Serializable]
    public struct DropRate
    {
        public Item item;
        public float rate;
    }

    public DropRate[] rates;
}