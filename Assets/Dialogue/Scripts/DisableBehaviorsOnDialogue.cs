using System;
using UnityEngine;
using System.Collections.Generic;
using WolverineSoft.DialogueSystem;

public class DisableBehaviorsOnDialogue : MonoBehaviour
{
    [SerializeField] List<Behaviour> behaviors = new List<Behaviour>();
    
    private DialogueManager dialogueManager;

    private void OnEnable()
    {
        dialogueManager = Managers.Get<DialogueManager>();
        dialogueManager.StartedDialogue.AddListener(DisableBehaviors);
        dialogueManager.EndedDialogue.AddListener(EnableBehaviors);
    }

    private void OnDisable()
    {
        dialogueManager.StartedDialogue.RemoveListener(DisableBehaviors);
        dialogueManager.EndedDialogue.RemoveListener(EnableBehaviors);
    }

    private void DisableBehaviors()
    {
        foreach (Behaviour behavior in behaviors)
        {
            behavior.enabled = false;
        }
    }

    private void EnableBehaviors()
    {
        foreach (Behaviour behavior in behaviors)
        {
            behavior.enabled = true;
        }
    }
}
