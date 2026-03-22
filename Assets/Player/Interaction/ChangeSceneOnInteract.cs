using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnInteract : InteractTriggerBehavior
{
    [SerializeField] SceneAsset targetScene;
    
    public override void TriggerInteraction(GameObject interactor)
    {
        SceneManager.LoadScene(targetScene.name);
    }
}
