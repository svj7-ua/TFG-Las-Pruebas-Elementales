using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGolemOrbitalStrikeController : MonoBehaviour
{

    private SphereCollider sphereCollider; // The sphere collider that will be used to check for collisions.
                                           
    [SerializeField]
    private float delay = 1.85f; // The delay before the sphere collider is enabled. (The Delay of the vertical beam)

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>(); // Get the sphere collider component attached to this game object.

        //Set active to false, so it doesn't check for collisions at the start.
        sphereCollider.enabled = false; // Disable the sphere collider at the start.

        StartCoroutine(DelayedEnable(delay)); // Start the coroutine to enable the sphere collider after a delay.

    }

    private IEnumerator DelayedEnable(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay.
        sphereCollider.enabled = true; // Enable the sphere collider after the delay.
    }

}
