using UnityEngine;

public class MeteorRainEffect : MonoBehaviour
{
    
    float damage = 10f; // Daño del efecto de lluvia de meteoros

    [SerializeField]
    private LayerMask layerMask; // Prefab del efecto de lluvia de meteoros

    [SerializeField]
    private EnumDamageTypes damageType = EnumDamageTypes.Arcane; // Tipo de daño del efecto de lluvia de meteoros

    [SerializeField] private EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee; // The type of spells the effect will trigger

    
    private int inventoryIndex = 0; // Index of the inventory from which it applies the effects, so effects aren't applied twice.

    private InventoryController inventory; // Referencia al inventario

    private float chanceToApplyEffects = 0.2f; // Chance to apply the effects from the active effects inventory

    private void Start()
    {
        // Busca el inventario en el Player
        inventory = FindObjectOfType<InventoryController>();
        if (inventory == null)
        {
            Debug.LogError("No se encontró InventoryController en la escena.");
        }
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

    public void AugmentChance(float addedChance){
        chanceToApplyEffects += addedChance; // Increase the chance to apply the effects from the active effects inventory
    }

    public float GetChanceToApplyEffects(){
        return chanceToApplyEffects; // Get the chance to apply the effects from the active effects inventory
    }


    void OnParticleCollision(GameObject other){

        if (layerMask == (layerMask | (1 << other.layer))){

            Hurtbox h = other.GetComponent<Hurtbox>();

            if(h != null){

                // Apply damage to the target
                Debug.Log("Hit, health: " + h.health.currentHealth + " name: " + other.gameObject.name);
                Debug.Log("SpellCard Type: " + spellCardType.ToString() + " Damage Type: " + damageType.ToString());
                float finalDamage = h.calculateDamage(damage, damageType); // Calculate the final damage based on the damage type and target's resistances
                Debug.Log("Final damage: " + finalDamage + " to " + other.gameObject.name + " by " + gameObject.name);

                h.EnemyHit(); // Call the enemy hit function to play the hit animation

                Debug.Log("Applying effects from inventory index: " + inventoryIndex + " to " + other.gameObject.name);
                if(finalDamage > 0){

                    //Has a chance to apply the effects from the active effects inventory
                    float randomChance = Random.Range(0.0f, 1.0f);
                    if(randomChance <= chanceToApplyEffects){
                        Debug.Log("Applying effects from inventory index: " + inventoryIndex + " to " + other.gameObject.name);
                        inventory.ApplyEffects(other.gameObject, inventoryIndex, spellCardType); // Apply the effects from the active effects inventory
                    }
                } else {
                    Debug.Log("No damage applied to " + other.gameObject.name + ", no effects will be applied.");
                }
            }
            else{
                Debug.Log("No Hurtbox found on: " + other.name);
            }

        }

    }

}