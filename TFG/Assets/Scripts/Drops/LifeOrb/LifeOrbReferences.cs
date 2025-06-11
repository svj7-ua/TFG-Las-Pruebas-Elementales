using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LifeOrbReferences : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;

    [SerializeField]
    public LayerMask playerLayer; // Layer mask to identify the player
    public Health playerHealth; // Reference to the player's Hurtbox component

    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component

    // Start is called before the first frame updatew
    void Awake()
    {
        // Finds the player GameObject in the scene
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("LifeOrbReferences: Player GameObject not found in the scene. Please ensure the player has the 'Player' tag.");
            return;
        }
        // Gets the Hurtbox component from the player GameObject
        playerHealth = player.GetComponent<Health>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null || playerHealth == null)
        {
            Debug.LogError("LifeOrbReferences: NavMeshAgent component not found on this GameObject or PlayerHealth not found in player.");
        }
    }

}
