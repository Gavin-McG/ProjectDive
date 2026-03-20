using UnityEngine;

public abstract class TriggerInteraction : InteractBehavior
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<InteractionController>()?.QueueInteraction(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    { 
        other.GetComponent<InteractionController>()?.DequeueInteraction(this);
    }
}