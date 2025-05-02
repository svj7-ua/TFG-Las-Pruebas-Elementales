using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFireArea : MonoBehaviour
{
    public float fireAmmount = 2.0f; // Amount of health to loose per tick
    
    public LayerMask layerMask; // Layer mask to determine which objects can be healed

    public float fireTickTime = 0.4f; // Cooldown time between ticks

    public float fireDuration = 2.0f; // Duration of the fire effect

    // private InventoryController inventory;

    private Vector3 default_size = new Vector3(1.2f, 0.66f, 1.2f); // Size of the area of effect

    [SerializeField]
    public Vector3 knockback = Vector3.zero; 

    // private void Start()
    // {
    //     // Busca el inventario en el Player
    //     inventory = FindObjectOfType<InventoryController>();
    //     if (inventory == null)
    //     {
    //         Debug.LogError("No se encontró InventoryController en la escena.");
    //     }
    // }

    public void changeSize (Vector3 _newSize){
        // Adds the new size to the current size of the object
        Vector3 currentSize = transform.localScale;
        Vector3 newSize = new Vector3(currentSize.x + _newSize.x, currentSize.y + _newSize.y, currentSize.z + _newSize.z);
        transform.localScale = newSize;
    }

    public void setDefaultSize(){
        // Sets the default size of the object
        transform.localScale = default_size;
    }

    public void AddFireDuration(float addedDuration){
        // Adds 0,4 seconds to the fire duration, which will add an additional tick of damage
        fireDuration += addedDuration;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(layerMask == (layerMask | (1 << other.gameObject.layer)) && other.GetComponent<Hurtbox>() != null){
            // Apply fire effect to the object
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();

            hurtbox.SetOnFire(fireAmmount, fireDuration, fireTickTime); // Set the object on fire

            if(hurtbox.health.currentHealth <= 0){
                    other.gameObject.SetActive(false);
            }
            
        }

    }

}
