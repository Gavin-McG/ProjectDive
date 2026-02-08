using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialCost
{
    [Serializable]
    public struct CostEntry
    {
        [SerializeField] public Material material;
        [SerializeField] public int count;
    }
    
    [SerializeField] public List<CostEntry> materials;
}