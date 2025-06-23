using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Hitbox : MonoBehaviour
{

    public float damage = 10f;

    public LayerMask layerMask; // This is the layer mask that determines which objects the hitbox checks.

    private int inventoryIndex = 0; // Index of the inventory from which it applies the effects, so effects aren't applied twice.

    private InventoryController inventory; // Referencia al inventario

    public EnumDamageTypes damageType = EnumDamageTypes.None; // Type of damage that the hitbox does

    [SerializeField] private EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee; // The type of spells the effect will trigger

    private bool applyEffects = true; // If true, the hitbox will apply effects from the inventory, if false, it won't apply any effects.

    private RunesModifiers runesModifiers; // Reference to the runesModifierManager to apply runes effects

    [Space]
    [Header("Secondary Effect related variables")]
    [Tooltip("If this hitbox has a secondary effect, set it here.")]

    [SerializeField]
    private bool hasSecondaryEffect = false; // Whether this hitbox has tick damage or not.

    [SerializeField]
    private float secondaryEffectDamage = 0f; // The amount of damage the secondary effect does. (tick damage, mostly)
    [SerializeField]
    private float secondaryEffectDuration = 0f; // The duration of the secondary effect.
    [SerializeField]
    private float secondaryEffectTickInterval = 0f; // The interval between ticks of the secondary effect. (if any)

    [Space]
    [Header("Chance to apply effects")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float chanceToApplyEffects = 1.0f; // Chance to apply effects from the inventory, 1.0 means always applies, 0.5 means 50% chance, etc.

    [Space]
    [Header("Check if this hitbox has to also check for stay collisions")]
    [Tooltip("If this hitbox has to check for stay collisions, set it here.")]
    [SerializeField]
    private bool checkStay = false; // Whether to check for stay collisions or not.
    [SerializeField]
    [Tooltip("The time between stay collisions.")]
    [Range(0.1f, 1f)]
    private float tickCooldown = 0.1f; // The time to check for stay collisions.
    private Dictionary<Collider, float> colliderLastTick = new Dictionary<Collider, float>();

    private void Start()
    {

        inventory = FindObjectOfType<InventoryController>();
        runesModifiers = FindObjectOfType<RunesModifiers>(); // Find the RunesModifiers script in the scene
        if (inventory == null)
        {
            Debug.LogError("No se encontró InventoryController en la escena.");
        }
        if (runesModifiers == null)
        {
            Debug.LogError("No se encontró RunesModifiers en la escena.");
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
    
    public void SetApplyEffects(bool apply){
        applyEffects = apply; // Set if the hitbox will apply effects from the inventory
    }   

    public void AumentSecondaryEffectDamage(float amount)
    {
        secondaryEffectDamage += amount; // Increase the secondary effect damage
    }

    public void AumentSecondaryEffectDuration(float amount)
    {
        secondaryEffectDuration += amount; // Increase the secondary effect duration
    }

    public void AumentSecondaryEffectTickInterval(float amount)
    {
        secondaryEffectTickInterval += amount; // Increase the secondary effect tick interval
    }

    public float GetSecondaryEffectDamage()
    {
        return secondaryEffectDamage; // Return the secondary effect damage
    }
    public float GetSecondaryEffectDuration()
    {
        return secondaryEffectDuration; // Return the secondary effect duration
    }
    public float GetSecondaryEffectTickInterval()
    {
        return secondaryEffectTickInterval; // Return the secondary effect tick interval
    }

    private void OnTriggerExit(Collider other)
    {

        if (checkStay)
        {
            if ((layerMask & (1 << other.gameObject.layer)) != 0)
            {
                
                if (colliderLastTick.ContainsKey(other))
                {
                    colliderLastTick.Remove(other);
                    Debug.Log("Collider exited: " + other.gameObject.name);
                }
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {

        if (checkStay)
        {
            if ((layerMask & (1 << other.gameObject.layer)) != 0)
            {
                float currentTime = Time.time;


                if (currentTime - colliderLastTick[other] >= tickCooldown)
                {
                    Hurtbox h = other.GetComponent<Hurtbox>();
                    if (h == null) { Debug.Log("No Hurtbox found on: " + other.name); return; }

                    colliderLastTick[other] = currentTime; // Update the last tick time for this collider
                    float finalDamage = h.CalculateDamage(damage, damageType); // Calculate the final damage based on the damage type and target's resistances
                    Debug.Log("Hit, health: " + h.health.currentHealth + " name: " + other.gameObject.name);
                    if (finalDamage > 0 && hasSecondaryEffect)
                    {
                        h.ApplySecondaryEffect(damageType, secondaryEffectDuration, secondaryEffectDamage, secondaryEffectTickInterval); // Apply the secondary effect based on the damage type and final damage
                        Debug.Log("Applying secondary effect to " + other.gameObject.name + " with damage: " + secondaryEffectDamage);
                    }

                }

            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if ((layerMask & (1 << other.gameObject.layer)) != 0)
        {

            Hurtbox h = other.GetComponent<Hurtbox>();

            if (h != null)
            {
                if (checkStay && !colliderLastTick.ContainsKey(other))
                {
                    colliderLastTick.Add(other, Time.time); // Add the collider to the dictionary with the current time
                }

                //Destroy(other.gameObject);
                Debug.Log("Hit, health: " + h.health.currentHealth + " name: " + other.gameObject.name);
                Debug.Log("SpellCard Type: " + spellCardType.ToString() + " Damage Type: " + damageType.ToString());
                float finalDamage = h.CalculateDamage(damage, damageType); // Calculate the final damage based on the damage type and target's resistances
                Debug.Log("Final damage: " + finalDamage + " to " + other.gameObject.name + " by " + gameObject.name);

                if (finalDamage > 0)
                {
                    if (runesModifiers.vampirism)
                    {
                        // Checks the vampirism chance
                        if (Random.value <= runesModifiers.vampirismChance)
                        {
                            inventory.gameObject.GetComponent<Health>().Heal(1.0f);
                        }
                    }

                    if (hasSecondaryEffect)
                        h.ApplySecondaryEffect(damageType, secondaryEffectDuration, secondaryEffectDamage, secondaryEffectTickInterval); // Apply the secondary effect based on the damage type and final damage
                }



                h.EnemyHit(); // Call the enemy hit function to play the hit animation

                Debug.Log("Applying effects from inventory index: " + inventoryIndex + " to " + other.gameObject.name);

                if (Random.value > chanceToApplyEffects)
                {
                    Debug.Log("Chance to apply effects failed, no effects will be applied.");
                    return; // If the chance to apply effects fails, return and do not apply effects
                }

                if (applyEffects)
                    inventory.ApplyEffects(other.gameObject, inventoryIndex, spellCardType); // Apply the effects from the active effects inventory

            }

        }

    }

}
