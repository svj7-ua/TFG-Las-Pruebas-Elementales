using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : MonoBehaviour
{

    [SerializeField] private float duration = 3f; // Duration of the tornado effect
    [SerializeField] private float speed = 5f; // Speed of the tornado
    [SerializeField] private float directinalChangeInterval = 1f; // Interval for changing direction
    [SerializeField] private float directionalRandomness = 180; // Randomness in direction change

    private float time = 0f; // Time since the tornado was created

    private Vector3 moveDirection; // Direction of the tornado

    // Start is called before the first frame update
    void Start()
    {
        moveDirection = GetRandomDirection(); // Get a random direction for the tornado
        InvokeRepeating(nameof(ChangeDirection), 0f, directinalChangeInterval); // Start changing direction at regular intervals
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime; // Move the tornado in the current direction

        time += Time.deltaTime; // Update the time since the tornado was created
        if (time >= duration) // Check if the tornado has reached its duration
        {
            Destroy(gameObject); // Destroy the tornado object
        }
    }

    void ChangeDirection()
    {
        // Change the direction of the tornado randomly within a certain range
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(-directionalRandomness, directionalRandomness), 0f);  // Create a random rotation
        // Apply the random rotation to the current move direction
        moveDirection = randomRotation * moveDirection;
        moveDirection.Normalize();
    }

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f); // Get a random angle
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized; // Return a random direction based on the angle
    }

    public void addDuration(float duration)
    {
        this.duration += duration; // Add duration to the tornado effect
    }

    public float getDuration()
    {
        return duration; // Get the duration of the tornado effect
    }

    public void setDuration(float duration)
    {
        this.duration = duration; // Set the duration of the tornado effect
    }

    public void setSpeed(float speed)
    {
        this.speed = speed; // Set the speed of the tornado effect
    }

    public void addSpeed(float speed)
    {
        this.speed += speed; // Add speed to the tornado effect
    }
}
