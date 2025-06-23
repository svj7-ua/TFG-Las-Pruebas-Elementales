using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingArea_Effect : MonoBehaviour
{

    public float healingAmount = 1.0f; // Amount of health to heal
    
    public LayerMask layerMask; // Layer mask to determine which objects can be healed

    public float healingCooldown = 0.5f; // Cooldown time between healing ticks
    private float lastHealTime = 0.0f; // Time of the last healing tick

    private void OnTriggerStay(Collider other)
    {

        // Check if the object is on the correct layer and if it has a Hurtbox component
       
        if (layerMask == (layerMask | (1 << other.gameObject.layer)) && other.GetComponent<Hurtbox>() != null)
        {
            // Check if enough time has passed since the last healing tick
            if (Time.time - lastHealTime >= healingCooldown)
            {
                // Heal the object
                Hurtbox hurtbox = other.GetComponent<Hurtbox>();
                hurtbox.health.Heal(healingAmount); // Heal the hurtbox by the specified amount
                Debug.Log("Healed " + other.gameObject.name + " for " + healingAmount + " health. Current health: " + hurtbox.health.currentHealth);
                lastHealTime = Time.time; // Update the last heal time
            }
        }
    }

}
