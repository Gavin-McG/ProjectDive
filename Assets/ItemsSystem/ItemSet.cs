using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ItemSet
{
    [Serializable]
    public struct ItemEntry
    {
        [FormerlySerializedAs("item")] [SerializeField] public ItemSO itemSo;
        [SerializeField] public int count;
    }
    
    [SerializeField] public List<ItemEntry> items;
}