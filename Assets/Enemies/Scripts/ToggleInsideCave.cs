using System;
using UnityEngine;

public class ToggleInsideCave : MonoBehaviour
{
    EnemyLeatherbackManager leatherbackManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leatherbackManager = transform.parent.Find("Enemy").GetComponent<EnemyLeatherbackManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            leatherbackManager.TogglePlayerInsideCave(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            leatherbackManager.TogglePlayerInsideCave(false);
        }
    }
}
