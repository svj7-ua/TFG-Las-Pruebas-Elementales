using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : MonoBehaviour
{
    private Animator animator;

    public LayerMask wallsLayer; // Layermask used to destroy the projectile when it hits a wall
    public LayerMask objectsLayer; // Layermask used to destroy the projectile when it hits an object

    public float speed; // Speed of the projectile

    private float generationTime = 0.0f;
    private bool wasShot = false;

    void Start()
    {
        // Obtener el componente Animator del objeto
        animator = GetComponent<Animator>();
        generationTime = Time.time;

    }

    void Update()
    {

        // Waits 0.2 seconds before shooting the projectile
        if(!wasShot && Time.time - generationTime > 0.2f)
        {
            // Shoot the projectile
            //animator.SetTrigger("Shoot");
            GetComponent<Rigidbody>().AddForce(transform.up * speed, ForceMode.Impulse);
            wasShot = true;
        }

        // Move the projectile forward
        //transform.Translate(Vector3.forward * Time.deltaTime * 10);
        // Destroy the projectile if it hits a wall or an object
        // if (Physics.Raycast(transform.position, transform.forward, 0.5f, wallsLayer) || Physics.Raycast(transform.position, transform.forward, 0.5f, objectsLayer))
        // {
        //     Destroy(gameObject);
        // }


    }
}
