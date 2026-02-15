using System;
using UnityEngine;

public class ItemCollectible : MonoBehaviour
{
    [SerializeField] private ItemSet itemSet;
    
    private ItemCollector collector;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            collector = other.GetComponent<ItemCollector>();
            collector?.QueueCollectible(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enabled && other.CompareTag("Player"))
            other.GetComponent<ItemCollector>()?.DequeueCollectible(this);
    }

    public ItemSet CollectItems()
    {
        collector.DequeueCollectible(this);
        enabled = false;
        return itemSet;
    }
}
