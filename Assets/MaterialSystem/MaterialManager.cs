using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WolverineSoft.SaveSystem;

public class MaterialManager : MonoBehaviour, ISaveData<MaterialManager.SaveData>
{
    public static MaterialManager Instance;
    public string Identifier => "MaterialManager";

    private List<Material> materials = new();
    private readonly Dictionary<Material, int> materialCounts = new();
    private readonly Dictionary<string, Material> materialsByName = new();

    [Serializable]
    public class SaveData
    {
        [Serializable]
        public struct MaterialEntry
        {
            [SerializeField] public string materialName;
            [SerializeField] public int count;
        }
        
        [SerializeField] public List<MaterialEntry> entries = new();
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        LoadMaterials();
    }

    private void LoadMaterials()
    {
        // Load materials and default to 0 count
        var newMaterials = Resources.LoadAll<Material>("").ToList();
        foreach (Material material in newMaterials)
        {
            AddMaterial(material);
        }
    }

    private int AddMaterial(Material material)
    {
        materials.Add(material);
        materialCounts[material] = material.defaultCount;
        materialsByName[material.materialName] = material;
        return materialCounts[material];
    }

    public int GetMaterialCount(Material material)
    {
        if (material == null)
        {
            Debug.LogError("MaterialManager: material is null");
        }

        if (materialCounts.TryGetValue(material, out int count))
        {
            return count;
        }

        return AddMaterial(material);
    }

    public void SetMaterialCount(Material material, int count)
    {
        if (material == null)
        {
            Debug.LogError("MaterialManager: material is null");
        }
        
        materialCounts[material] = count;
    }

    public void AddMaterial(Material material, int count)
    {
        if (material == null)
        {
            Debug.LogError("MaterialManager: material is null");
        }

        if (materialCounts.TryGetValue(material, out int currentCount))
        {
            int newCount = Mathf.Max(0,currentCount + count);
            materialCounts[material] = newCount;
        }

        Debug.LogWarning("MaterialManager: material count is null");
        materialCounts[material] = count;
        materialsByName[material.name] = material;
        materials.Add(material);
    }

    public bool ChargeMaterials(MaterialCost cost)
    {
        foreach (var entry in cost.materials)
        {
            if (GetMaterialCount(entry.material) < entry.count)
                return false;
        }

        foreach (var entry in cost.materials)
        {
            AddMaterial(entry.material, entry.count);
        }
        return true;
    }
    
    
    // ISaveData Implementation

    public void RestoreToDefault()
    {
        foreach (Material material in materials)
        {
            materialCounts[material] = material.defaultCount;
        }
    }

    public void RestoreFromSaveData(SaveData data)
    {
        foreach (var entry in data.entries)
        {
            if (materialsByName.TryGetValue(entry.materialName, out var material))
            {
                SetMaterialCount(material, entry.count);
            }
            else
            {
                Debug.LogWarning($"MaterialManager: material {entry.materialName} not found");
            }
        }
    }

    public SaveData GetSaveData()
    {
        SaveData saveData = new SaveData();
        
        foreach (Material material in materials)
        {
            saveData.entries.Add(new SaveData.MaterialEntry()
            {
                materialName = material.materialName,
                count = materialCounts[material]
            });
        }
        
        return saveData;
    }
}
