using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    public float damage = 10f;
    public Vector3 knockback = new Vector3(0, 0, 0);

    public LayerMask layerMask; // This is the layer mask that determines which objects the hitbox checks.

    private void OnTriggerEnter_final(Collider other)
    {
        

        if (layerMask == (layerMask | (1 << other.gameObject.layer))){
            Hurtbox h = other.GetComponent<Hurtbox>();

            if (h != null){
                h.playerHealth.health -= damage;
                // TODO: Applies hit animation to the player.
            }
        }

    }

    //Test Version
    private void OnTriggerEnter(Collider other){

        if (layerMask == (layerMask | (1 << other.gameObject.layer))){
        
            Hurtbox h = other.GetComponent<Hurtbox>();

            if (h != null){

                //Destroy(other.gameObject);
                Debug.Log("Hit, health: " + h.playerHealth.health + " name: " + other.gameObject.name);
                h.playerHealth.health = 0;
                other.gameObject.SetActive(false);
                
            }
        
        }

    }

}
