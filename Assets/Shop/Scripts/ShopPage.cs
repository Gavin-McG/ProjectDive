using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopPage", menuName = "Shop/Shop Page")]
public class ShopPage : ScriptableObject
{
    [SerializeField] public List<ShopItem> shopItems;

    private void OnValidate()
    {
        //Set item count to 6, expanding or shrinking list to fit
        if (shopItems.Count < 6)
        {
            int itemsToAdd = 6 - shopItems.Count;
            for (int i = 0; i < itemsToAdd; i++)
            {
                shopItems.Add(null);
            }
        }
        else if (shopItems.Count > 6)
        {
            shopItems.RemoveRange(6, shopItems.Count - 6);
        }
    }
}
