using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item Object")]
public class ItemSO : ScriptableObject
{
    public enum Rarity { Basic, Rare, Legendary }

    [SerializeField] private string displayName;
    [SerializeField] public Sprite icon;
    [SerializeField] public Rarity rarity;
    [SerializeField] public int defaultCount;
    
    // Return Display Name, or Format SO name if empty
    public string DisplayName => displayName.Length == 0 ? ToPretty(name) : displayName;
    
    private static string ToPretty(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(
            input,
            @"(?<!^)(?:
            (?<=[a-z])(?=[A-Z]) |        # fooBar
            (?<=[A-Z])(?=[A-Z][a-z])     # FOOBar
        )",
            " ",
            RegexOptions.IgnorePatternWhitespace
        );
    }
}
