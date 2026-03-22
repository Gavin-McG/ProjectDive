using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int tankCount = 2;
    [SerializeField] private float health = 1;
    
    [SerializeField] private float removeRate = 0.2f;
    [SerializeField] SceneAsset deathScene;

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
        
        if (health <= 0)
            SceneManager.LoadScene(deathScene.name);
    }
}
