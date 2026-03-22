using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int tankCount = 2;
    [SerializeField] private float health = 1;
    
    [SerializeField] private float removeRate = 0.2f;

    private OxygenTankDisplay display;

    private void OnEnable()
    {
        GameObject displayGO = GameObject.Find("OxygenTankDisplay");
        display = displayGO?.GetComponent<OxygenTankDisplay>();
    }

    private void Start()
    {
        display?.SetTankCount(tankCount);
        health = tankCount;
    }

    private void Update()
    {
        health -= removeRate * Time.deltaTime;
        display?.SetOxygenAmount(health);
    }
}
