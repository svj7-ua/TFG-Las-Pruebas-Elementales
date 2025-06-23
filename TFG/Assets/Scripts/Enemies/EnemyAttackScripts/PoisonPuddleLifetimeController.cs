using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddleLifetimeController : MonoBehaviour
{

    private float lifetime = 1f; // Lifetime of the poison puddle in seconds
    private float timer; // Timer to track the lifetime

    private float startDelay = 0.5f; // Delay before the puddle starts to disappear
    private float startDelayTimer; // Timer for the start delay
    private bool isDelayOver = false; // Flag to check if the delay is over

    [SerializeField]
    bool isWidow = false; // Flag to check if the enemy is a widow
    private void Start()
    {
        if (isWidow) lifetime = Random.Range(2f, 3f);
        else lifetime = Random.Range(4f, 7f); // Randomize the lifetime between 0.5 and 1.5 seconds
        timer = lifetime; // Initialize the timer with the lifetime value
    }


    // Update is called once per frame
    void Update()
    {
        if (isDelayOver)
        {
            timer -= Time.deltaTime; // Decrease the timer by the time passed since last frame
            if (timer <= 0f) // If the timer has reached zero
            {
                Destroy(gameObject); // Destroy the poison puddle game object
            }
        } else
        {
            startDelayTimer += Time.deltaTime; // Increase the start delay timer by the time passed since last frame
            if (startDelayTimer >= startDelay) // If the start delay timer has reached the start delay value
            {
                isDelayOver = true; // Set the flag to true to start the lifetime countdown

                // Sets all the children of this object to be active
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true); // Activate each child game object
                }

                //Activates the collider of the poison puddle
                GetComponent<Collider>().enabled = true; // Enable the collider of the poison puddle
            }
        }

    }
}
