using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ShopUI : MonoBehaviour
{
    [SerializeField] private float dialogueTextRate = 25;
    
    
    
    private VisualElement shopContents;

    //Elements for opening/closing transitions
    private VisualElement backgroundContainer;
    private VisualElement horaceContainer;
    private VisualElement shopContainer;
    private Coroutine transitionCoroutine;
    
    //Elements needed for Shop Dialogue
    private VisualElement dialogueContainer;
    private Label dialogueText;
    private VisualElement dialogueButtonContainer;
    private Button dialogueYesButton;
    private Button dialogueNoButton;
    private Coroutine dialoguePromptCoroutine;
    
    //Shop Entries
    struct ShopOption
    {
        public VisualElement root;
        public Image itemImage;
        public Image costImage;
        public Label costText;
    }
    List<ShopOption> shopOptions;
    
    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();
        shopContents = document.rootVisualElement;
        
        //Collect main shop elements
        backgroundContainer = document.rootVisualElement.Q<VisualElement>("background-container");
        horaceContainer = document.rootVisualElement.Q<VisualElement>("horace-container");
        shopContainer = document.rootVisualElement.Q<VisualElement>("shop-container");
        
        //Collect dialogue elements
        dialogueContainer = document.rootVisualElement.Q<VisualElement>("dialogue-container");
        dialogueText = document.rootVisualElement.Q<Label>("dialogue-text");
        dialogueButtonContainer = document.rootVisualElement.Q<VisualElement>("dialogue-button-container");
        dialogueYesButton = document.rootVisualElement.Q<Button>("dialogue-yes-button");
        dialogueNoButton = document.rootVisualElement.Q<Button>("dialogue-no-button");

        //Collect Shop Entries
        shopOptions = shopContainer.Query("ShopItem").ToList()
            .Select(element =>
            {
                return new ShopOption()
                {
                    root = element,
                    itemImage = element.Q<Image>("item-image"),
                    costImage = element.Q<Image>("cost-image"),
                    costText = element.Q<Label>("cost-text"),
                };
            }).ToList();
    }

    [ContextMenu("Open Shop")]
    public void OpenShop()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(OpenShopRoutine());
    }

    [ContextMenu("Close Shop")]
    public void CloseShop()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(CloseShopRoutine());
    }

    public void DebugDialoguePrompt(string text)
    {
        if (dialoguePromptCoroutine != null)
        {
            StopCoroutine(dialoguePromptCoroutine);
        }
        dialoguePromptCoroutine = StartCoroutine(BeginDialoguePromptRoutine(text));
    }

    IEnumerator OpenShopRoutine()
    {
        backgroundContainer.AddToClassList("center");
        backgroundContainer.RemoveFromClassList("top");
        
        yield return new WaitForSeconds(0.6f);
        
        horaceContainer.AddToClassList("center");
        horaceContainer.RemoveFromClassList("left");
        
        shopContainer.AddToClassList("center");
        shopContainer.RemoveFromClassList("right");
    }
    
    IEnumerator CloseShopRoutine()
    {
        horaceContainer.AddToClassList("left");
        horaceContainer.RemoveFromClassList("center");
        
        shopContainer.AddToClassList("right");
        shopContainer.RemoveFromClassList("center");
        
        yield return new WaitForSeconds(0.6f);

        backgroundContainer.AddToClassList("top");
        backgroundContainer.RemoveFromClassList("center");
    }

    IEnumerator BeginDialoguePromptRoutine(string prompt)
    {
        //Enable dialogue visibility
        dialogueText.visible = true;
        dialogueButtonContainer.visible = false;

        //Update text
        yield return DisplayDialogueTextRoutine(prompt, dialogueTextRate);
        
        //Enable buttons
        dialogueButtonContainer.visible = true;
    }

    IEnumerator DisplayDialogueTextRoutine(string text, float textRate)
    {
        //Loop until the text is fully filled
        int currentTextLength = 0;
        float totalTimePasses = 0;
        while (currentTextLength < text.Length)
        {
            totalTimePasses += Time.deltaTime;
            
            //Update new text length, clamping to total text length
            currentTextLength = (int)Mathf.Floor(totalTimePasses / textRate);
            currentTextLength = Mathf.Clamp(currentTextLength, 0, text.Length);
            
            dialogueText.text = text.Substring(currentTextLength);
                
            yield return null;
        }
    }
}
