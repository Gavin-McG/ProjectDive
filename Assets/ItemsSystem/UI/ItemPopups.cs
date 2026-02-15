using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ItemPopups : MonoBehaviour
{
    private static readonly string OpenClassName = "popupOpen";
    public static ItemPopups Instance;
    
    [Header("Popup Settings")]
    [SerializeField] private float popupStayDuration = 1f;
    //[SerializeField] private AudioClip collectSound;
    //[SerializeField] private float volume;
    [Header("UI Assets")]
    [SerializeField] private VisualTreeAsset flyingItem;
    [Header("Animation")]
    [SerializeField] private float flyingDuration;
    [SerializeField] private AnimationCurve flyingPositionCurve;
    [SerializeField] private AnimationCurve flyingSizeCurve;
    
    private UIDocument uiDocument;
    private VisualElement flyingItemLayer;

    private List<VisualElement> popups;
    private List<ItemSO> popupItems;
    private List<int> popupCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        flyingItemLayer = uiDocument.rootVisualElement.Q("FlyingItems");
        popups = uiDocument.rootVisualElement.Query("Popup").ToList();

        popupItems = new List<ItemSO>(popups.Count);
        popupCount = new List<int>(popups.Count);

        for (int i = 0; i < popups.Count; i++)
        {
            popupItems.Add(null);
            popupCount.Add(0);
        }

    }

    /// <summary>
    /// Finds an existing popup for an item or opens a new one if allowed.
    /// </summary>
    private int FindPopup(ItemSO item, bool findOpen = true)
    {
        // Look for matching item first
        for (int i = 0; i < popups.Count; i++)
        {
            if (popupItems[i] == item)
            {
                AssignPopup(i, item);
                return i;
            }
        }

        if (!findOpen)
            return -1;

        // Look for open slot
        for (int i = 0; i < popups.Count; i++)
        {
            if (popupItems[i] == null)
            {
                AssignPopup(i, item);
                return i;
            }
        }

        return -1;
    }

    private void AssignPopup(int index, ItemSO item)
    {
        popupItems[index] = item;
        popups[index].AddToClassList(OpenClassName);

        popups[index].Q("Image").style.backgroundImage = new StyleBackground(item.icon);
        popups[index].Q<Label>("Text").text = ItemManager.Instance.GetItemCount(item).ToString();
    }

    /// <summary>
    /// Called when an item is collected in the world.
    /// </summary>
    public void AddItem(ItemSet.ItemEntry entry, Vector3 worldPos)
    {
        int index = FindPopup(entry.itemSo);
        if (index == -1)
        {
            ItemManager.Instance.AddItemCount(entry.itemSo, entry.count);
            return;
        }

        popupCount[index]++;

        VisualElement popup = popups[index];

        // Convert world → panel space
        Vector2 source = RuntimePanelUtils.CameraTransformWorldToPanel(flyingItemLayer.panel, worldPos, Camera.main);
        Vector2 destination = popup.parent.worldBound.center;

        // Create flying item element from VisualTreeAsset
        VisualElement flying = flyingItem.Instantiate();
        flying.style.position = Position.Absolute;
        flying.style.left = source.x;
        flying.style.top = source.y;

        popups[index].Q("Image").style.backgroundImage = new StyleBackground(entry.itemSo.icon);

        flyingItemLayer.Add(flying);

        // Animate to popup
        StartCoroutine(AnimateFlyingItem(
            flying,
            source,
            destination,
            flyingDuration,
            entry
        ));
    }

    /// <summary>
    /// Called when a flying item reaches its popup.
    /// </summary>
    private void FinishItem(ItemSet.ItemEntry entry)
    {
        int popupNum = FindPopup(entry.itemSo, findOpen: false);
        if (popupNum == -1)
            return;
        
        popups[popupNum].Q<Label>("Text").text = ItemManager.Instance.AddItemCount(entry.itemSo, entry.count).ToString();

        /*if (collectSound)
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position, volume);
        */

        // Invoke the FMOD event as a one-shot
        FMODUnity.RuntimeManager.PlayOneShot("event:/Item Collect", Camera.main.transform.position);

        popupCount[popupNum]--;
        if (popupCount[popupNum] == 0)
        {
            StartCoroutine(FreePopupRoutine(popupNum, popupStayDuration));
        }
    }
    
    private IEnumerator AnimateFlyingItem(
        VisualElement element,
        Vector2 start,
        Vector2 destination,
        float duration,
        ItemSet.ItemEntry entry
    ) {
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // Apply easing
            float positionT = flyingPositionCurve.Evaluate(t);
            float size = flyingSizeCurve.Evaluate(t);

            Vector2 pos = Vector2.Lerp(start, destination, positionT);

            element.style.left = pos.x;
            element.style.top = pos.y;
            element.style.scale = new StyleScale(Vector3.one * size);

            yield return null;
        }

        // Snap to final position (important for precision)
        element.style.left = destination.x;
        element.style.top = destination.y;

        element.RemoveFromHierarchy();
        FinishItem(entry);
    }

    private IEnumerator FreePopupRoutine(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (popupCount[index] == 0)
        {
            // Close popup if still not waiting
            popups[index].RemoveFromClassList(OpenClassName);
            yield return new WaitForSeconds(0.7f);
            
            //free popup if still not waiting
            if (popupCount[index] == 0)
            {
                popupItems[index] = null;
            }
        }
    }
}
