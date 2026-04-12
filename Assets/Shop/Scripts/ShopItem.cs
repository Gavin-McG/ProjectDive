using System.Collections.Generic;
using UnityEngine;

public abstract class ShopItem : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;
    [SerializeField] public ItemSO costItem;
    [SerializeField] public int costAmount;
    
    [TextArea(1,10)]
    [SerializeField] public string prompt;

    public bool Buy()
    {
        // Get the ItemManager
        ItemManager itemManager = Managers.Get<ItemManager>();
        
        //Check if player can afford the item
        ItemSet costSet = new ItemSet() {
            items = new List<ItemSet.ItemEntry>() { 
                new() {
                    count = costAmount,
                    itemSo = costItem,
                }
            }
        };
        if (!itemManager.ChargeItems(costSet)) return false;
        
        //Inform UpgradeManager of owned upgrade
        UpgradeManager upgradeManager = Managers.Get<UpgradeManager>();
        upgradeManager.RegisterOwned(this);
        
        //Apply effect and return true
        ApplyEffect();
        return true;
    }

    public abstract void ApplyEffect();
}
