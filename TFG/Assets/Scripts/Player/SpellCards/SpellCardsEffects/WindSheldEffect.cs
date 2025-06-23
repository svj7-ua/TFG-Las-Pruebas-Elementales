using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WindSheldEffect : MonoBehaviour
{

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float shieldDuration = 1.5f; // Time the object is stunned

    private List<Collider> trapped_enemies = new List<Collider>(); // List of colliders in the wind shield
    private float time = 0f; // Time since the wind shield was created

    void Update()
    {
        // Check if the wind shield is still active
        time += Time.deltaTime;
        if (time >= shieldDuration)
        {
            FreeEnemies(); // Free all enemies trapped in the wind shield
            Destroy(gameObject); // Destroy the wind shield
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if(layerMask == (layerMask | (1 << other.gameObject.layer)) && other.GetComponent<Hurtbox>() != null){
            // Apply fire effect to the object
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox.isBoss)
            {
                return; // Do not trap bosses
            }
            other.transform.position = gameObject.transform.position; // Set the position of the object to the position of the wind shield
            trapped_enemies.Add(other); // Add the object to the list of trapped enemies
            hurtbox.TrapTarget(); // Trap the object in the wind shield
            
        }
    }

    public void AugmentDuration(float addedDuration){
        // Adds 0,4 seconds to the wind shield duration
        shieldDuration += addedDuration;
    }

    public float getShieldDuration(){
        // Returns the duration of the wind shield
        return shieldDuration;
    }

    void FreeEnemies(){
        Debug.Log("Freeing enemies trapped in the wind shield: " + trapped_enemies.Count); // Debug message to check if the function is called
        // Free all enemies trapped in the wind shield
        foreach (Collider enemy in trapped_enemies){
            if(enemy != null){
                enemy.GetComponent<Hurtbox>().FreeTarget(); // Free the object from the wind shield
            }
        }
    }

}
