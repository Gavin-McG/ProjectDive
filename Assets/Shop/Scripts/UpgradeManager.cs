using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WolverineSoft.SaveSystem;

public class UpgradeManager : MonoBehaviour, ISaveData<UpgradeManager.SaveData>
{
    [SerializeField] private List<ShopItem> ownedItems;

    public void RegisterOwned(ShopItem item)
    {
        ownedItems.Add(item);
    }

    public bool HasUpgrade(ShopItem item)
    {
        return ownedItems.Contains(item);
    }
    
    
    //-----------Save System-------------------
    
    [Serializable]
    public class SaveData
    {
        public List<string> ownedItems;
    }

    public string Identifier => "upgrades";

    public void RestoreToDefault()
    {
        ownedItems.Clear();
    }

    public SaveData GetSaveData()
    {
        return new SaveData() {
            ownedItems = ownedItems.Select(item => item.itemName).ToList()
        };
    }

    public void RestoreFromSaveData(SaveData data)
    {
        Dictionary<string, ShopItem> shopItems = Resources.FindObjectsOfTypeAll<ShopItem>()
            .ToDictionary(
                item => item.itemName,
                item => item
            );

        foreach (string ownedItem in data.ownedItems)
        {
            if (shopItems.TryGetValue(ownedItem, out ShopItem shopItem))
            {
                RegisterOwned(shopItem);
                shopItem.ApplyEffect();
            }
            else
            {
                Debug.LogWarning("Could not find upgrade '" + ownedItem + "'");
            }
        }
        
    }
}
