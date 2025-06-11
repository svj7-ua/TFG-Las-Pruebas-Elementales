using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEffect : MonoBehaviour
{

    [SerializeField]
    [Range(0.1f, 2f)]
    private float updateTime = 0.2f; // Time in seconds between updates
    
    private Transform target; // The target to home in on
    private float timer; // Timer to track the update time

    // Start is called before the first frame update
    void Start()
    {

        target = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        timer = updateTime; // Initialize the timer with the update time
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (target == null) return; // If the target is not assigned, do nothing

        timer -= Time.deltaTime; // Decrease the timer by the time passed since last frame

        if (timer <= 0f) // If the timer has reached zero
        {
            timer = updateTime; // Reset the timer to the update time

            // Rotate the effect to face the target
            Vector3 direction = (target.position - transform.position).normalized; // Calculate the direction to the target
            Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation that looks in the direction of the target
            lookRotation.x = 0; // Set the X rotation to 0
            lookRotation.z = 0; // Set the Z rotation to 0
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Smoothly rotate towards the target
            //Debug.LogError("HomingEffect is trying to home in on the target: " + target.name);
        }

    }
}
