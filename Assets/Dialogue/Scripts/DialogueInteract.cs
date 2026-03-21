using System;
using UnityEngine;
using WolverineSoft.DialogueSystem;

public class DialogueInteract : InteractTriggerBehavior
{
    [SerializeField] DialogueAsset asset;
    [SerializeField] private string startPoint;
    
    DialogueManager dialogueManager;

    private void OnEnable()
    {
        dialogueManager = Managers.Get<DialogueManager>();
    }

    public override void TriggerInteraction(GameObject interactor)
    {
        dialogueManager?.BeginDialogue(asset, startPoint);
    }
}
