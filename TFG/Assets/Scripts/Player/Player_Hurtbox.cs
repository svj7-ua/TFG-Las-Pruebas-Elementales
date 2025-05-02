using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script is attached to the hurtbox object, which is a child of the player object.
// The hurtbox object is a trigger collider that is used to detect when the player is hit by an enemy attack. When the hurtbox is triggered by an enemy hitbox, it sends a message to the player object to reduce the player's health.
public class Player_Hurtbox : MonoBehaviour
{

    public Health health;
    public PlayerController_test playerController;

    private void Start(){
        health = GetComponentInParent<Health>();
        //playerController = GetComponentInParent<PlayerController_test>(); // No se necesita para el maniqui
    }

}
