using System;
using System.Collections;
using AYellowpaper.SerializedCollections.KeysGenerators;
using UnityEngine;

[KeyListGenerator("Populate Items", typeof(ItemSO), false)]
public class ItemKeyGenerator : KeyListGenerator
{
    public override IEnumerable GetKeys(Type type)
    {
        return Resources.LoadAll<ItemSO>("");
    }
}
