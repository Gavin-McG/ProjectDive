using System;
using UnityEngine;
using WolverineSoft.DialogueSystem;

[RequireComponent(typeof(ShopUI))]
public class OpenShopListener : MonoBehaviour
{
    [SerializeField] private DSEvent openShopEvent;
    
    private ShopUI shopUI;

    private void OnEnable()
    {
        shopUI = GetComponent<ShopUI>();
        openShopEvent.AddListener(OpenShop);
    }

    private void OnDisable()
    {
        openShopEvent.RemoveListener(OpenShop);
    }

    private void OpenShop()
    {
        shopUI.OpenShop();
    }
}
