using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsPickUp : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; // Player LayerMask

    void OnTriggerEnter(Collider other)
    {

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log($"Gems Picked Up by {other.gameObject.name}!"); // Log message for debugging
            other.GetComponent<InventoryController>().AddGems(1); // Add the item to the player's inventory
            // Destroy the life orb after picking it up (Destroy the parent object, since the LifeOrbPickUp is a child of the LifeOrb)
            Destroy(gameObject);
        }


    }
}
