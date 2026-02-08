using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Scriptable Objects/Material Object")]
public class Material : ScriptableObject
{
    public enum Rarity { Basic, Rare, Legendary }
    
    [SerializeField] public string materialName;
    [SerializeField] public Sprite icon;
    [SerializeField] public Rarity rarity;
    [SerializeField] public int defaultCount;
}
