using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DireccionController : MonoBehaviour
{


    public Transform target; // Object of reference to move arround
    public float rotationSpeed = 1.0f; // Speed of rotation
    public float circleRadius = 1.0f;
    public float elevationOffset = 0.0f;
    private Vector3 positionOffset;

    private Vector3 mousePosition;

    private Vector2 targetPosition2D;

    //private float angle = 0.0f;

    private void Start()
    {
        //Sets the target position to the center of the screen
        targetPosition2D = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void LateUpdate()
    {
        // // Get the mouse position in screen space and convert it to world space using the correct z-depth.
        mousePosition = Input.mousePosition;

        // Convert the mouse and target positions to 2D space (XZ plane)
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y); // Use X and Z for 2D calculation

        // Calculate the angle using Mathf.Atan2, which gives the angle in radians
        float angleRadians = Mathf.Atan2(mousePosition2D.y - targetPosition2D.y, mousePosition2D.x - targetPosition2D.x);
        
        // Convert the angle from radians to degrees if needed (optional, for debugging)
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        // Calculate the position offset based on the angle
        positionOffset.Set(
            Mathf.Cos(angleRadians) * circleRadius, // X component
            elevationOffset,                        // Y component (elevation/height)
            Mathf.Sin(angleRadians) * circleRadius  // Z component
        );

        // Set the position of the object (triangle) to move around the target
        transform.position = target.position + positionOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.2f); // Adjust Z if necessary

        // Rotates the object (triangle) to point towards the mouse position
        LookAtCursor(angleRadians);
        

        // Log values of Vectors and angle in Console for debugging
        Debug.Log("MousePosition: " + mousePosition);
        Debug.Log("TargetPosition2D: " + targetPosition2D);
        Debug.Log("MousePosition2D: " + mousePosition2D);
        Debug.Log("Angle in Degrees: " + angleDegrees);
    }

    void LookAtCursor(float angleRadians){
        
        // Convert the angle from radians to degrees
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        // Apply the rotation to the object (rotate around the Z axis)
        transform.rotation = Quaternion.Euler(0, 0, angleDegrees - 90); // Subtract 90 to align the triangle's forward direction with the cursor
    }
}
