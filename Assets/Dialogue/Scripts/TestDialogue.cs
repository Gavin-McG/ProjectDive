using System;
using UnityEngine;
using WolverineSoft.DialogueSystem;

public class TestDialogue : MonoBehaviour
{
    [SerializeField] DialogueAsset _dialogueAsset;
    [SerializeField] private bool Begin;
    
    private bool started = false;
    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = Managers.Get<DialogueManager>();
    }

    private void Update()
    {
        if (Begin && !started)
        {
            started = true;
            dialogueManager.BeginDialogue(_dialogueAsset);
        }

        if (!Begin && started)
        {
            started = false;
        }
    }
}