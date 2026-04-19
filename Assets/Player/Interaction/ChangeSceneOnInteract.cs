using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnInteract : InteractTriggerBehavior
{
    [SerializeField] string targetScene;
    
    public override void TriggerInteraction(GameObject interactor)
    {

        // Play transition sound
        FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/Transition", Camera.main.transform.position);

        SceneManager.LoadScene(targetScene);
    }
}
