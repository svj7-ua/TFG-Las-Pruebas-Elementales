using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    public float poisonAmmount = 2.0f; // Health loss per tick
    
    public LayerMask layerMask; // Layer mask to determine which objects can be healed

    public float poisonTickTime = 0.2f; // Cooldown time between ticks

    public float poisonDuration = 2.0f; // Duration of the poison effect

    public void AugmentDuration(float duration)
    {
        // Increase the duration of the poison effect
        poisonDuration += duration;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is on the correct layer and if it has a Hurtbox component
        if (layerMask == (layerMask | (1 << other.gameObject.layer)) && other.GetComponent<Hurtbox>() != null)
        {
            // Apply poison effect to the object
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();

            hurtbox.PoisonTarget(poisonAmmount, poisonDuration, poisonTickTime); // Apply poison effect to the object
            Debug.Log("Poisoned " + other.gameObject.name + " for " + poisonAmmount + " health. Current health: " + hurtbox.health.currentHealth);

        }
    }

}
