using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitManager : MonoBehaviour
{
    [SerializeField] InputActionReference quitAction;

    private void Update()
    {
        if (quitAction.action.IsPressed())
        {
            Debug.Log("Quitting");
            Application.Quit();
        }
    }
}
