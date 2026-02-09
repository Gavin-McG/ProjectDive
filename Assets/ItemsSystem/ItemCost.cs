using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ItemCost
{
    [Serializable]
    public struct CostEntry
    {
        [FormerlySerializedAs("item")] [SerializeField] public ItemSO itemSo;
        [SerializeField] public int count;
    }
    
    [SerializeField] public List<CostEntry> items;
}