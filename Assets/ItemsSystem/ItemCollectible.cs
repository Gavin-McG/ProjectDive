using System;
using UnityEngine;

public class ItemCollectible : MonoBehaviour
{
    [SerializeField] private ItemSet itemSet;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<ItemCollector>()?.QueueCollectible(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<ItemCollector>()?.DequeueCollectible(this);
    }

    public ItemSet CollectItems()
    {
        return itemSet;
    }
}
