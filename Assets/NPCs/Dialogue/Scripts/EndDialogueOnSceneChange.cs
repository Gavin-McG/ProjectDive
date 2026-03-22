using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using WolverineSoft.DialogueSystem;

[RequireComponent(typeof(DialogueManager))]
public class EndDialogueOnSceneChange : MonoBehaviour
{
    private DialogueManager dialogueManager;
    
    private void OnEnable()
    {
        dialogueManager = GetComponent<DialogueManager>();
        SceneManager.activeSceneChanged += DisableDialogue;
    }

    void DisableDialogue(Scene scene1, Scene scene2)
    {
        dialogueManager.EndDialogue();
    }
}
