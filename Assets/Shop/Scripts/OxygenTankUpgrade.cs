using UnityEngine;

[CreateAssetMenu(fileName = "OxygenTankUpgrade", menuName = "Shop/Upgrades/OxygenTankUpgrade")]
public class OxygenTankUpgrade : ShopItem
{
    [Header("Upgrade Info")]
    [SerializeField] private float oxygenMultiplier;

    protected override void ApplyEffect()
    {
        Debug.Log("Oxygen Tank Upgraded by " + oxygenMultiplier + " Times!");
    }
}
