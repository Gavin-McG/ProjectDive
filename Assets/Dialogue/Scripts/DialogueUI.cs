using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using WolverineSoft.DialogueSystem;

[RequireComponent(typeof(UIDocument))]
public class DialogueUI : MonoBehaviour
{
    DialogueManager dialogueManager;
    
    private VisualElement dialogueContents;
    private VisualElement profile;
    private Label nameText;
    private Label text;
    private Button continueButton;
    
    private bool isPanelOpen = false;

    private void OnEnable()
    {
        dialogueManager = Managers.Get<DialogueManager>();
        dialogueManager.StartedDialogue.AddListener(DialogueBegan);
        
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        
        dialogueContents = root.Q("DialogueContent");
        profile = root.Q("Profile");
        nameText = root.Q<Label>("Name");
        text = root.Q<Label>("Text");
        continueButton = root.Q<Button>("ContinueButton");

        continueButton.clicked += ContinuePressed;
    }

    private void OnDisable()
    {
        dialogueManager.StartedDialogue.RemoveListener(DialogueBegan);
        continueButton.clicked -= ContinuePressed;
    }

    private void OpenDialoguePanel()
    {
        dialogueContents.RemoveFromClassList("closed");
        dialogueContents.AddToClassList("open");
        isPanelOpen = true;
        
        continueButton.SetEnabled(false);
    }

    private void CloseDialoguePanel()
    {
        dialogueContents.RemoveFromClassList("open");
        dialogueContents.AddToClassList("closed");
        isPanelOpen = false;
    }

    private void DialogueBegan()
    {
        var info = dialogueManager.AdvanceDialogue(new AdvanceContext());
        UpdateUI(info);
    }

    IEnumerator SetPanelText(string newText, float openDelay, float charDelay)
    {
        text.text = "";
        
        if (!isPanelOpen)
        {
            OpenDialoguePanel();
            yield return new WaitForSeconds(openDelay);
        }

        float time = 0;
        int currentLength = 0;
        while (currentLength < newText.Length)
        {
            //Calculate the number of new characters to add
            time += Time.deltaTime;
            int newChars = (int)Mathf.Floor(time / charDelay);

            //Change the text if necessary
            if (newChars > 0)
            {
                time -= charDelay * newChars;
                currentLength += newChars;
                currentLength = Mathf.Clamp(currentLength, 0, newText.Length);
                
                string subString = newText.Substring(0, currentLength);
                text.text = subString;
            }

            yield return null;
        }
        
        // Allow continue to be pressed once completed
        continueButton.SetEnabled(true);
        continueButton.pickingMode = PickingMode.Position;
    }

    private void ContinuePressed()
    {
        continueButton.SetEnabled(false);
        continueButton.pickingMode = PickingMode.Ignore;
        
        var info = dialogueManager.AdvanceDialogue(new AdvanceContext());
        UpdateUI(info);
    }

    private void UpdateUI(DialogueInfo info)
    {
        //Close dialogue once the end is reached
        if (info == null)
        {
            CloseDialoguePanel();
            return;
        }
        
        StartCoroutine(SetPanelText(info.text, 0.5f, 0.05f));
    }
}
