using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] InputActionReference collectAction;
    [SerializeField] GameObject indicator;

    private List<ItemCollectible> queuedItems = new List<ItemCollectible>();
    
    public void QueueCollectible(ItemCollectible c)
    {
        queuedItems.Add(c);
        indicator?.SetActive(true);
    }

    public void DequeueCollectible(ItemCollectible c)
    {
        queuedItems.Remove(c);
        
        if (queuedItems.Count == 0)
            indicator?.SetActive(false);
    }

    private void OnEnable()
    {
        collectAction.action.performed += CollectItems;
    }

    private void OnDisable()
    {
        collectAction.action.performed -= CollectItems;
    }

    private void CollectItems(InputAction.CallbackContext context)
    {
        foreach (ItemCollectible collectible in queuedItems)
        {
            foreach (ItemSet.ItemEntry entry in collectible.CollectItems().items)
            {
                ItemManager.Instance.AddItemCount(entry.itemSo, entry.count);
            }
        }
    }
}
