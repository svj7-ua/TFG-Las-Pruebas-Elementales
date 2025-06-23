using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : MonoBehaviour
{
    private Animator animator;

    public LayerMask wallsLayer; // Layermask used to destroy the projectile when it hits a wall

    public LayerMask enemiesLayer; // Layermask used to destroy the projectile when it hits an enemy
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


    }

    private void OnTriggerEnter(Collider collider)
    {
        
        // Once the projectile hits a wall or an object, it is destroyed
        if (((1 << collider.gameObject.layer) & wallsLayer) != 0 || ((1 << collider.gameObject.layer) & objectsLayer) != 0)
        {
            //Debug.LogError("ARROW Collision detected with object: " + collider.gameObject.name);
            Destroy(gameObject);
        }
    }
}
