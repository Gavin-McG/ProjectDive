using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WolverineSoft.SaveSystem;
using AYellowpaper.SerializedCollections;

public class ItemManager : MonoBehaviour, ISaveData<ItemManager.SaveData>
{
    public static ItemManager Instance;
    public string Identifier => "MaterialManager";
    
    [SerializeField] private SerializedDictionary<ItemSO, int> itemCounts = new();

    private readonly HashSet<ItemSO> items = new();
    private readonly Dictionary<string, ItemSO> itemsByName = new();
    
    private void Awake()
    {
        //Generic Instance check
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        //Load in items in Awake (Don't attempt to use items until Start)
        LoadItems();
    }

    // Check whether a value was previously registered
    private bool HasItem(ItemSO item) => items.Contains(item);

    // Load in all the items found in Resources
    private void LoadItems()
    {
        // Load items from resources
        var newItems = Resources.LoadAll<ItemSO>("").ToList();
        foreach (ItemSO item in newItems)
        {
            AddItem(item);
        }

        // Ensure all specified counts are added to other lists
        foreach (var pair in itemCounts)
        {
            if (!HasItem(pair.Key)) AddItem(pair.Key);
        }
    }

    // Register an item and assign it to its default count - returns count
    private int AddItem(ItemSO itemSo)
    {
        if (HasItem(itemSo))
        {
            Debug.LogWarning($"ItemManager: Attempting to add registered Item {itemSo.name}");
            return itemCounts[itemSo];
        }
        
        // Set item count to default if one not already specified
        if (!itemCounts.ContainsKey(itemSo))
        {
            itemCounts[itemSo] = itemSo.defaultCount;
        }
        
        // Add to other lists
        items.Add(itemSo);
        itemsByName[itemSo.name] = itemSo;
        return itemCounts[itemSo];
    }

    // Get the count owned of a specific item - Add the item if unregistered
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

    // Set the count of an Item to a specified value - Add the item if unregistered
    public void SetItemCount(ItemSO itemSo, int count)
    {
        if (itemSo == null)
        {
            Debug.LogError("ItemManager: item is null");
        }
        
        //Add the item if not registered
        if (!HasItem(itemSo))
        {
            AddItem(itemSo);
        }
        
        
        itemCounts[itemSo] = count;
    }

    // Increment the count of an item by a specified value - Add the 
    public void AddItemCount(ItemSO itemSo, int count)
    {
        if (itemSo == null)
        {
            Debug.LogError("ItemManager: item is null");
        }

        //Add the item if not registered
        if (!HasItem(itemSo))
        {
            AddItem(itemSo);
        }
        
        int newCount = Mathf.Max(0,itemCounts[itemSo] + count);
        itemCounts[itemSo] = newCount;
    }

    public bool CanAfford(ItemCost cost)
    {
        foreach (var entry in cost.items)
        {
            if (GetItemCount(entry.itemSo) < entry.count)
                return false;
        }
        
        return true;
    }

    // Attempt to charge the current item counts by a specified amount - returns whether charge was succefsful
    public bool ChargeItems(ItemCost cost, bool ignoreAfford = false)
    {
        if (!ignoreAfford && !CanAfford(cost)) return false;

        foreach (var entry in cost.items)
        {
            AddItemCount(entry.itemSo, -entry.count);
        }
        return true;
    }
    
    //-------------------------------------------------
    //          ISaveData Implementation
    //-------------------------------------------------
    
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

    [ContextMenu("Reset Item Counts")]
    public void RestoreToDefault()
    {
        LoadItems();
        foreach (ItemSO item in items)
        {
            SetItemCount(item, item.defaultCount);
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
                itemName = item.name,
                count = itemCounts[item]
            });
        }
        
        return saveData;
    }
}
