using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;


public class PlayerController_test : MonoBehaviour
{

    public float speed;
    public float groundDistance;

    public LayerMask groundMask;
    public Rigidbody rigidBody;
    public SpriteRenderer spriteRenderer;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        Movement();

        Dash();

    }

    void Movement()
    {
        RaycastHit hit;
        Vector3 castPosition = transform.position;
        castPosition.y += 1;

        if(Physics.Raycast(castPosition, -transform.up, out hit, Mathf.Infinity, groundMask))
        {
            if(hit.collider != null){
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDistance;
                transform.position = movePos;
            }

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 moveDirection = new Vector3(x, 0, z).normalized;
            rigidBody.velocity = moveDirection * speed;

            if(x != 0 || z != 0){
                animator.SetFloat("isRunning", 1);
            } else {
                animator.SetFloat("isRunning", 0);
            }
            if(x < 0){
                spriteRenderer.flipX = true;
            } else if(x > 0){
                spriteRenderer.flipX = false;
            }
        }
    }

    void Dash()
    {
        //When the player press the space key it will dash in the direction it is facing
        //The direction is determined by the last direction the player moved
        //And it can be:
        //  - Up
        //  - Down
        //  - Left
        //  - Right
        //  - Up-Left
        //  - Up-Right
        //  - Down-Left
        //  - Down-Right
        if(Input.GetKeyDown(KeyCode.Space))
        {
            
            

        }
    }

}
