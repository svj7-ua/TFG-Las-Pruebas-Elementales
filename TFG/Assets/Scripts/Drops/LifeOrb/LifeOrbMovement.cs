using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeOrbMovement : MonoBehaviour
{


    private LifeOrbReferences lifeOrbReferences; // Reference to the LifeOrbReferences component

    [SerializeField]
    private float delay = 0.2f; // Delay between updates

    private float lastUpdateTime = 0f; // Time of last update
    private bool playerInRange = false; // Whether player is within trigger

    // Start is called before the first frame update
    void Start()
    {
        // Get the LifeOrbReferences component attached to this GameObject
        lifeOrbReferences = GetComponent<LifeOrbReferences>();
        if (lifeOrbReferences == null)
        {
            Debug.LogError("LifeOrbMovement: LifeOrbReferences component not found on this GameObject.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & lifeOrbReferences.playerLayer) != 0)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & lifeOrbReferences.playerLayer) != 0)
        {
            playerInRange = false;
            lifeOrbReferences.navMeshAgent.ResetPath(); // Stop moving when player exits range
        }
    }

    void Update()
    {
        if (playerInRange && Time.time - lastUpdateTime >= delay)
        {
            lastUpdateTime = Time.time;
            lifeOrbReferences.navMeshAgent.SetDestination(lifeOrbReferences.player.transform.position);
        }
    }
}