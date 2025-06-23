using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAOE_Hitbox : MonoBehaviour
{
    private InventoryController inventory; // Referencia al inventario

    public LayerMask layerMask; // This is the layer mask that determines which objects the hitbox checks.

    [SerializeField] private float dashEffectsCooldown = 10.0f; // Cooldown for the dash effects
    private float lastDashEffectsTime = -Mathf.Infinity; // Last time the dash effects were applied

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Dash AOE Hitbox created");
        inventory = FindObjectOfType<InventoryController>();
        if (inventory == null)
        {
            Debug.LogError("No se encontró InventoryController en la escena.");
        }

    }

    void OnTriggerEnter(Collider other)
    {
        // Dash AOE needs to not be in cooldown to apply effects
        // Check if the object is in the layer mask and if the cooldown has passed
        if ((Time.time - lastDashEffectsTime > dashEffectsCooldown) && layerMask == (layerMask | (1 << other.gameObject.layer))){
            Debug.Log("Enter hitbox: " + other.gameObject.name + " Timer: " + (Time.time - lastDashEffectsTime) + " Cooldown: " + dashEffectsCooldown);
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();

            if (hurtbox != null)
            {
                    lastDashEffectsTime = Time.time; // Update the last time the dash effects were applied
                    Debug.Log("DASH AOE Hitbox hit " + other.gameObject.name);
                    // DOES SOMETHING
                    inventory.ApplyEffects(other.gameObject, 0, EnumSpellCardTypes.Dash_AOE);
            }

        }
        
    }

}
