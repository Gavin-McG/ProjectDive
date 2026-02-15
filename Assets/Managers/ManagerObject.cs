using UnityEngine;

[CreateAssetMenu(fileName = "ManagerObject", menuName = "Scriptable Objects/Manager Object", order = 1)]
public class ManagerObject : ScriptableObject
{
    [SerializeField] public GameObject prefab;
}