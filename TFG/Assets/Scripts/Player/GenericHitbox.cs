using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericHitbox : MonoBehaviour
{
    // Start is called before the first frame update

    public float damage = 10f;
    public Vector3 knockback = new Vector3(0, 0, 0);

    public LayerMask layerMask; // This is the layer mask that determines which objects the hitbox checks.

    private int inventoryIndex = 0; // Index of the inventory from which it applies the effects, so effects aren't applied twice.

    private InventoryController inventory; // Referencia al inventario

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
    }

    private void OnTriggerEnter(Collider other){

        if (layerMask == (layerMask | (1 << other.gameObject.layer))){
        
            Hurtbox h = other.GetComponent<Hurtbox>();

            if (h != null){

                //Destroy(other.gameObject);
                Debug.Log("Hit, health: " + h.health.currentHealth + " name: " + other.gameObject.name);
                h.health.currentHealth -= damage;
                if(h.health.currentHealth <= 0){
                    other.gameObject.SetActive(false);
                }

                inventory.ApplyEffects(other.gameObject, inventoryIndex, 0); // Apply the effects from the active effects inventory

                
            }
        
        }

    }
}
