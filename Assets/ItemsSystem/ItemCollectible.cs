using System;
using UnityEngine;

public class ItemCollectible : InteractTriggerBehavior
{
    [SerializeField] private ItemSet itemSet;
    
    public override void TriggerInteraction(GameObject interactor)
    {
        foreach (ItemSet.ItemEntry entry in itemSet.items)
        {
            ItemPopups.Instance.AddItem(entry, transform.position);
        }

        Destroy(gameObject);
    }
}