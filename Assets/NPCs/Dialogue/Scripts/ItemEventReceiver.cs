using System;
using UnityEngine;
using WolverineSoft.DialogueSystem;

public class ItemEventReceiver : MonoBehaviour
{
    [SerializeField] DSEventItems itemEvent;

    private void OnEnable()
    {
        itemEvent.AddListener(CollectItems);
    }

    private void OnDisable()
    {
        itemEvent.RemoveListener(CollectItems);
    }

    void CollectItems(ItemSet items)
    {
        foreach (var entry in items.items)
        {
            ItemPopups.Instance.AddItem(entry, Vector3.zero);
        }
    }
}
