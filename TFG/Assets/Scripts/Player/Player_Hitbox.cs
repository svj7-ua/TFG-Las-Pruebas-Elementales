using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Hitbox : MonoBehaviour
{

    public float damage = 10f;
    public Vector3 knockback = new Vector3(0, 0, 0);

    public LayerMask layerMask; // This is the layer mask that determines which objects the hitbox checks.

    private int inventoryIndex = 0; // Index of the inventory from which it applies the effects, so effects aren't applied twice.

    private InventoryController inventory; // Referencia al inventario

    public EnumDamageTypes damageType = EnumDamageTypes.None; // Type of damage that the hitbox does

    [SerializeField] private EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee; // The type of spells the effect will trigger
    private void Start()
    {
        // Busca el inventario en el Player
        inventory = FindObjectOfType<InventoryController>();
        if (inventory == null)
        {
            Debug.LogError("No se encontró InventoryController en la escena.");
        }
    }

    public void AugmentDamage(float amount){
        damage += amount; // Increase the damage of the hitbox
    }

    public void SetInventoryIndex(int index){
        inventoryIndex = index; // Set the inventory index to apply the effects from the active effects inventory
        if(gameObject.GetComponent<Collider>() != null){
            gameObject.GetComponent<Collider>().enabled = true; // Set the collider
        }
    }

    public void SetSpellCardType(EnumSpellCardTypes type){
        spellCardType = type; // Set the spell card type to apply the effects from the active effects inventory
    }

    private float calculateDamage(float baseDamage, Hurtbox hurtbox){
        float finalDamage = baseDamage; // Start with the base damage
        switch (damageType){
            case EnumDamageTypes.Fire:
                if (hurtbox.isInmuneToFire) finalDamage = 0.0f;
                if (hurtbox.isResistantToFire) finalDamage = baseDamage / 2.0f; // Fire damage is halved if the target is resistant to fire
                break;
            case EnumDamageTypes.Lightning:
                if (hurtbox.isInmuneToLightning) finalDamage = 0.0f;
                if (hurtbox.isResistantToLightning) finalDamage = baseDamage / 2.0f; // Electric damage is halved if the target is resistant to electricity
                break;

            case EnumDamageTypes.Poison:
                if (hurtbox.isInmuneToPoison) finalDamage = 0.0f;
                if (hurtbox.isResistantToPoison) finalDamage = baseDamage / 2.0f; // Poison damage is halved if the target is resistant to poison
                break;
            case EnumDamageTypes.Wind:
                if (hurtbox.isInmuneToWind) finalDamage = 0.0f;
                if (hurtbox.isResistantToWind) finalDamage = baseDamage / 2.0f; // Wind damage is halved if the target is resistant to wind
                break;
            // More cases can be added here for different damage types
            default:
                return baseDamage; // Default case, return base damage
        }

        return finalDamage;

    }

    //Test Version
    private void OnTriggerEnter(Collider other){

        if (layerMask == (layerMask | (1 << other.gameObject.layer))){
        
            Hurtbox h = other.GetComponent<Hurtbox>();

            if (h != null){

                //Destroy(other.gameObject);
                Debug.Log("Hit, health: " + h.health.currentHealth + " name: " + other.gameObject.name);
                Debug.Log("SpellCard Type: " + spellCardType.ToString() + " Damage Type: " + damageType.ToString());
                float finalDamage = h.calculateDamage(damage, damageType); // Calculate the final damage based on the damage type and target's resistances
                Debug.Log("Final damage: " + finalDamage + " to " + other.gameObject.name + " by " + gameObject.name);

                h.EnemyHit(); // Call the enemy hit function to play the hit animation

                Debug.Log("Applying effects from inventory index: " + inventoryIndex + " to " + other.gameObject.name);
                if(finalDamage > 0){
                    inventory.ApplyEffects(other.gameObject, inventoryIndex, spellCardType); // Apply the effects from the active effects inventory
                } else {
                    Debug.Log("No damage applied to " + other.gameObject.name + ", no effects will be applied.");
                }

                
            }
        
        }

    }

}
