using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject indicator;

    private readonly List<InteractBehavior> queuedInteractions = new();

    public void QueueInteraction(InteractBehavior interaction)
    {
        if (!queuedInteractions.Contains(interaction))
            queuedInteractions.Add(interaction);
    }

    public void DequeueInteraction(InteractBehavior interaction)
    {
        queuedInteractions.Remove(interaction);
    }

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }

    private void Update()
    {
        int enabledCount = queuedInteractions.Aggregate(0, (count, interaction) => interaction.enabled ? count + 1 : count);
        indicator.SetActive(enabledCount > 0);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // Copy to avoid modification during iteration
        var temp = queuedInteractions.ToList();

        foreach (var interaction in temp)
        {
            if (interaction.enabled)
                interaction.TriggerInteraction(gameObject);
        }
    }
}