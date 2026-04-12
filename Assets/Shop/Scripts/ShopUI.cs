using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UIDocument))]
public class ShopUI : MonoBehaviour
{
    [SerializeField] private float dialogueTextRate = 25;
    [SerializeField] private List<ShopPage> shopPages;

    [Header("Dialogue Messages")]
    [SerializeField] private List<string> startMessages;
    [SerializeField] private string tooExpensiveMessage;
    [SerializeField] private List<string> purchaseMessages;
    [SerializeField] private List<string> goodbyeMessages;
    
    private VisualElement shopContents;

    //Elements for opening/closing transitions
    private VisualElement backgroundContainer;
    private Button closeButton;
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
        public Button itemButton;
    }
    private List<ShopOption> shopOptions;
    private int currentShopPageIndex;
    private ShopItem selectedShopItem;
    
    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();
        shopContents = document.rootVisualElement;
        
        //Collect main shop elements
        backgroundContainer = shopContents.Q<VisualElement>("background-container");
        closeButton = shopContents.Q<Button>("close-button");
        horaceContainer = shopContents.Q<VisualElement>("horace-container");
        shopContainer = shopContents.Q<VisualElement>("shop-container");

        closeButton.clicked += CloseShop;
        
        //Collect dialogue elements
        dialogueContainer = shopContents.Q<VisualElement>("dialogue-container");
        dialogueText = shopContents.Q<Label>("dialogue-text");
        dialogueButtonContainer = shopContents.Q<VisualElement>("dialogue-button-container");
        dialogueYesButton = shopContents.Q<Button>("dialogue-yes-button");
        dialogueNoButton = shopContents.Q<Button>("dialogue-no-button");

        dialogueYesButton.clicked += BuySelectedShopItem;
        dialogueNoButton.clicked += () =>
        {
            DeselectShopItem();
            CloseDialoguePrompt();
        };
        
        //Collect Shop Entries
        shopOptions = shopContainer.Query("ShopItem").ToList()
            .Select((element, i) => {
                //Get option elements
                ShopOption newOption = new() {
                    root = element,
                    itemImage = element.Q<Image>("item-image"),
                    costImage = element.Q<Image>("cost-image"),
                    costText = element.Q<Label>("cost-text"),
                    itemButton = element.Q<Button>("item-button"),
                };
                //Add button functionality
                newOption.itemButton.clicked += () => SelectShopItem(i);
                
                return newOption;
            }).ToList();
    }

    [ContextMenu("Open Shop")]
    public void OpenShop()
    {
        dialogueContainer.visible = false;
        SetShopPage(0);

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(OpenShopRoutine());
    }

    [ContextMenu("Close Shop")]
    public void CloseShop()
    {
        dialogueContainer.visible = false;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(CloseShopRoutine());
    }

    public void SetShopPage(int shopPageIndex)
    {
        currentShopPageIndex = shopPageIndex;
        ShopPage shopPage = shopPages.ElementAt(shopPageIndex);
        for (int i = 0; i < shopOptions.Count; i++)
        {
            ShopItem shopItem = shopPage.shopItems[i];
            ShopOption shopOption = shopOptions[i];

            if (shopItem == null)
            {
                shopOption.root.visible = false;
                continue;
            }

            shopOption.root.visible = true;
            shopOption.itemImage.sprite = shopItem.icon;
            shopOption.costText.text = shopItem.costAmount.ToString();
            shopOption.costImage.sprite = shopItem.costItem.icon;
        }
    }

    public void SelectShopItem(int optionIndex)
    {
        ShopPage shopPage = shopPages.ElementAt(currentShopPageIndex);
        selectedShopItem = shopPage.shopItems[optionIndex];
        
        BeginDialoguePrompt(selectedShopItem.prompt);
    }

    public void DeselectShopItem()
    {
        selectedShopItem = null;
        dialogueButtonContainer.visible = false;
    }

    public void BuySelectedShopItem()
    {
        if (selectedShopItem == null) return;

        if (!selectedShopItem.Buy())
        {
            UpdateDialogueText(tooExpensiveMessage);
            return;
        }
        
        //deselect item
        DeselectShopItem();
        
        //Update to random purchase text
        string text = purchaseMessages.ElementAt(Random.Range(0, purchaseMessages.Count));
        UpdateDialogueText(text);
        
        //TODO remove bought item from display
    }

    public void BeginDialoguePrompt(string text)
    {
        if (dialoguePromptCoroutine != null)
        {
            StopCoroutine(dialoguePromptCoroutine);
        }
        dialoguePromptCoroutine = StartCoroutine(BeginDialoguePromptRoutine(text));
    }

    public void UpdateDialogueText(string text)
    {
        if (dialoguePromptCoroutine != null)
        {
            StopCoroutine(dialoguePromptCoroutine);
        }
        
        dialoguePromptCoroutine = StartCoroutine(DisplayDialogueTextRoutine(text, dialogueTextRate));
    }

    public void CloseDialoguePrompt()
    {
        if (dialoguePromptCoroutine != null)
        {
            StopCoroutine(dialoguePromptCoroutine);
        }
        dialoguePromptCoroutine = null;
        
        dialogueContainer.visible = false;
        dialogueButtonContainer.visible = false;
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
        
        yield return new WaitForSeconds(1.2f);
        
        //Update to random starting text
        string text = startMessages.ElementAt(Random.Range(0, startMessages.Count));
        UpdateDialogueText(text);
    }
    
    IEnumerator CloseShopRoutine()
    {
        //Update to random goodbye text
        string text = goodbyeMessages.ElementAt(Random.Range(0, goodbyeMessages.Count));
        UpdateDialogueText(text);
        
        yield return new WaitForSeconds(1.2f);
        
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
        dialogueContainer.visible = true;
        dialogueButtonContainer.visible = false;

        //Update text
        yield return DisplayDialogueTextRoutine(prompt, dialogueTextRate);
        
        //Enable buttons
        dialogueButtonContainer.visible = true;
    }

    IEnumerator DisplayDialogueTextRoutine(string text, float textRate)
    {
        dialogueContainer.visible = true;
        dialogueButtonContainer.visible = false;
        
        //Loop until the text is fully filled
        int currentTextLength = 0;
        float totalTimePassed = 0;
        while (currentTextLength < text.Length)
        {
            totalTimePassed += Time.deltaTime;
            
            //Update new text length, clamping to total text length
            currentTextLength = (int)Mathf.Floor(totalTimePassed * textRate);
            currentTextLength = Mathf.Clamp(currentTextLength, 0, text.Length);
            
            dialogueText.text = text.Substring(0, currentTextLength);
                
            yield return null;
        }
    }
}
