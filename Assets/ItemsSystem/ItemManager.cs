using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WolverineSoft.SaveSystem;

public class ItemManager : MonoBehaviour, ISaveData<ItemManager.SaveData>
{
    public static ItemManager Instance;
    public string Identifier => "MaterialManager";

    private List<ItemSO> items = new();
    private readonly Dictionary<ItemSO, int> itemCounts = new();
    private readonly Dictionary<string, ItemSO> itemsByName = new();

    [Serializable]
    public class SaveData
    {
        [Serializable]
        public struct ItemEntry
        {
            [SerializeField] public string itemName;
            [SerializeField] public int count;
        }
        
        [SerializeField] public List<ItemEntry> entries = new();
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        LoadItems();
    }

    private void LoadItems()
    {
        // Load items and default to 0 count
        var newItems = Resources.LoadAll<ItemSO>("").ToList();
        foreach (ItemSO item in newItems)
        {
            AddItem(item);
        }
    }

    private int AddItem(ItemSO itemSo)
    {
        items.Add(itemSo);
        itemCounts[itemSo] = itemSo.defaultCount;
        itemsByName[itemSo.itemName] = itemSo;
        return itemCounts[itemSo];
    }

    public int GetItemCount(ItemSO itemSo)
    {
        if (itemSo == null)
        {
            Debug.LogError("ItemManager: item is null");
        }

        if (itemCounts.TryGetValue(itemSo, out int count))
        {
            return count;
        }

        return AddItem(itemSo);
    }

    public void SetItemCount(ItemSO itemSo, int count)
    {
        if (itemSo == null)
        {
            Debug.LogError("ItemManager: item is null");
        }
        
        itemCounts[itemSo] = count;
    }

    public void AddItemCount(ItemSO itemSo, int count)
    {
        if (itemSo == null)
        {
            Debug.LogError("ItemManager: item is null");
        }

        if (itemCounts.TryGetValue(itemSo, out int currentCount))
        {
            int newCount = Mathf.Max(0,currentCount + count);
            itemCounts[itemSo] = newCount;
        }

        Debug.LogWarning("ItemManager: item count is null");
        itemCounts[itemSo] = count;
        itemsByName[itemSo.name] = itemSo;
        items.Add(itemSo);
    }

    public bool ChargeItems(ItemCost cost)
    {
        foreach (var entry in cost.items)
        {
            if (GetItemCount(entry.itemSo) < entry.count)
                return false;
        }

        foreach (var entry in cost.items)
        {
            AddItemCount(entry.itemSo, -entry.count);
        }
        return true;
    }
    
    
    // ISaveData Implementation

    public void RestoreToDefault()
    {
        foreach (ItemSO item in items)
        {
            itemCounts[item] = item.defaultCount;
        }
    }

    public void RestoreFromSaveData(SaveData data)
    {
        foreach (var entry in data.entries)
        {
            if (itemsByName.TryGetValue(entry.itemName, out var item))
            {
                SetItemCount(item, entry.count);
            }
            else
            {
                Debug.LogWarning($"ItemManager: item {entry.itemName} not found");
            }
        }
    }

    public SaveData GetSaveData()
    {
        SaveData saveData = new SaveData();
        
        foreach (ItemSO item in items)
        {
            saveData.entries.Add(new SaveData.ItemEntry()
            {
                itemName = item.itemName,
                count = itemCounts[item]
            });
        }
        
        return saveData;
    }
}
