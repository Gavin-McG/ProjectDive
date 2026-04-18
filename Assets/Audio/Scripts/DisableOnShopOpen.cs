using FMODUnity;
using UnityEngine;
using WolverineSoft.DialogueSystem;

public class DisableOnShopOpen : MonoBehaviour
{
    // Open shop event reference
    [SerializeField] private DSEvent openShopEvent;

    // FMOD Island Ambiance Event instance
    FMODUnity.StudioEventEmitter island_ambiance_event;

    private void OnEnable()
    {
        openShopEvent.AddListener(DisableAmbiance);
        island_ambiance_event = GetComponent<StudioEventEmitter>();
    }

    private void OnDisable()
    {
        openShopEvent.RemoveListener(DisableAmbiance);
    }

    private void DisableAmbiance()
    {

        island_ambiance_event.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }

}
