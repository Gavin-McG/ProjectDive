using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OxygenTankDisplay : MonoBehaviour
{
    [Header("UXML References")]
    [SerializeField] private VisualTreeAsset tankTemplate;

    [Header("Colors")]
    [SerializeField] private Color tankColor = Color.white;
    [SerializeField] private Color emptyColor = Color.gray;
    [SerializeField] private Color warningColor = Color.red;

    [Header("Warning Animation")]
    [SerializeField] private AnimationCurve warningCurve;
    [SerializeField] private float warningSpeed = 2f;

    private UIDocument document;
    private VisualElement tankHolder;

    private class TankUI
    {
        public VisualElement root;
        public VisualElement oxygenBar;
        public Image outline;
        public Image sprite;
    }

    private List<TankUI> tanks = new List<TankUI>();
    private float oxygenAmount = 0f;
    private float warningTime = 0f;

    void Awake()
    {
        document = GetComponent<UIDocument>();
        tankHolder = document.rootVisualElement.Q("TankHolder");
    }

    void Update()
    {
        AnimateWarningTank();
    }

    public void SetTankCount(int count)
    {
        tankHolder.Clear();
        tanks.Clear();

        for (int i = 0; i < count; i++)
        {
            VisualElement tankRoot = tankTemplate.Instantiate();
            tankHolder.Add(tankRoot);

            TankUI tank = new TankUI();
            tank.root = tankRoot;
            tank.oxygenBar = tankRoot.Q("OxygenBar");
            tank.outline = tankRoot.Q<Image>("TankOutline");
            tank.sprite = tankRoot.Q<Image>("TankSprite");

            tanks.Add(tank);
        }

        UpdateTankVisuals();
    }

    public void SetOxygenAmount(float amount)
    {
        oxygenAmount = Mathf.Clamp(amount, 0, tanks.Count);
        UpdateTankVisuals();
    }

    private void UpdateTankVisuals()
    {
        for (int i = 0; i < tanks.Count; i++)
        {
            float tankFill = Mathf.Clamp01(oxygenAmount - i);

            // Set oxygen bar height (0% - 100%)
            Length length = Length.Percent(tankFill * 100f);
            tanks[i].oxygenBar.style.height = length;
            tanks[i].oxygenBar.style.minHeight = length;
            tanks[i].oxygenBar.style.maxHeight = length;

            // Color tank based on fill
            Color color = Color.Lerp(emptyColor, tankColor, tankFill);
            tanks[i].sprite.tintColor = color;
            tanks[i].outline.tintColor = color;
        }
    }

    private void AnimateWarningTank()
    {
        if (tanks.Count == 0)
            return;
        
        // Only animate if last tank is below 50%
        if (oxygenAmount < 0.5f && oxygenAmount > 0f)
        {
            warningTime += Time.deltaTime * warningSpeed;
            float t = warningCurve.Evaluate(warningTime % 1f);
            Color warningTint = Color.Lerp(tankColor, warningColor, t);

            tanks[0].sprite.tintColor = warningTint;
            tanks[0].outline.tintColor = warningTint;
        }
    }
}