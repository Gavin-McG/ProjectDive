using FMODUnity;
using UnityEngine;
using WolverineSoft.DialogueSystem;

public class DisableOnShopOpen : MonoBehaviour
{
    // Open shop event reference
    [SerializeField] private DSEvent openShopEvent;

    // FMOD Island Ambiance Event instance
    FMODUnity.StudioEventEmitter IslandAmbianceEvent;

    private void OnEnable()
    {
        openShopEvent.AddListener(DisableAmbiance);
        IslandAmbianceEvent = GetComponent<StudioEventEmitter>();
    }

    private void OnDisable()
    {
        openShopEvent.RemoveListener(DisableAmbiance);
    }

    private void DisableAmbiance()
    {

        IslandAmbianceEvent.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }

}
