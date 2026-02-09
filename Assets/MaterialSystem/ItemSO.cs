using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item Object")]
public class ItemSO : ScriptableObject
{
    public enum Rarity { Basic, Rare, Legendary }
    
    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;
    [SerializeField] public Rarity rarity;
    [SerializeField] public int defaultCount;
}
