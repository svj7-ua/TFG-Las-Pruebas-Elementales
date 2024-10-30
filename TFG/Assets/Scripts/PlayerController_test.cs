using System.Collections;
using System.Collections.Generic;


//using System.Numerics;
using UnityEngine;


public class PlayerController_test : MonoBehaviour
{

    public float speed;
    public float dashSpeed = 10.0f; // Dash speed multiplier
    public float dashDuration = 0.1f; // Duration of the dash
    public float dashCooldown = 0.5f; // Cooldown between dashes
    public float groundDistance;

    public LayerMask groundMask;
    public Rigidbody rigidBody;
    public SpriteRenderer spriteRenderer;

    private Animator animator;
    private Vector3 moveDirection;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;


    private Vector3 mousePosition;
    public Transform meleeAttackObject;

    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0.5f;
    public static int noOfClicks = 0;
    float lastClickTime = 0f;
    float maxComboTime = 0.8f;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        if(!isDashing){

            // Movement
            if(!isAttacking())  Movement();
            // Attack
            if(Time.time - lastClickTime > attackCooldown)    Attack();

        }

        // Dash
        if(!isAttacking())    Dash();


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
            moveDirection = new Vector3(x, 0, z).normalized;
            rigidBody.velocity = moveDirection * speed;

            
            setAnimator(x, z);  // Set the animator to the direction of the input so that the player faces the right direction

        }
    }

    // Sets the animator to the direction of the input sot that the player faces the right direction
    void setAnimator(float x, float z)
    {
            if(x != 0 || z != 0){
                animator.SetFloat("Speed", 1);
                
                // Set the animator to the direction of the input so that the player also faces the right direction once the player stops moving
                if(x == 0){
                    animator.SetFloat("Horizontal_player", 0);
                    if(z > 0){
                        animator.SetFloat("Vertical_player", 1);
                    } else {
                        animator.SetFloat("Vertical_player", -1);
                    }
                } else {
                    animator.SetFloat("Vertical_player", 0);
                    if(x > 0){
                        animator.SetFloat("Horizontal_player", 1);
                    } else {
                        animator.SetFloat("Horizontal_player", -1);
                    }
                }

            } else {
                animator.SetFloat("Speed", 0);
            }
    }

    void Dash()
    {

        // Start cooldown timer if dashing is on cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Initiate dash if the player presses Space, is not dashing, and the cooldown is over
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownTimer <= 0)
        {
            Debug.Log("Dash");
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown; // Reset the cooldown timer
            //animator.SetFloat("Dash", 1);
        }

        // Handle dashing
        if (isDashing)
        {
            // Apply dash velocity in the current movement direction
            rigidBody.velocity = moveDirection.normalized * dashSpeed;
            Debug.Log("Move Direcion: " + moveDirection);

            // Decrease dash timer
            dashTimer -= Time.deltaTime;

            // End dash when the timer runs out
            if (dashTimer <= 0)
            {
                isDashing = false;
                //animator.SetFloat("Dash", 0);
            }
        }
    }


    // Manage player attack
    void Attack(){

        meleAttack();
        rangedAttack();

    }

    bool isAttacking(){
        if (Time.time - lastClickTime > maxComboTime)
        {
            animator.SetFloat("Hit", 0.0f);
            noOfClicks = 0;
            return false;
        } else {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            return true;
        }
    }

    // Manage mele attack
    void meleAttack(){
        
        // Check if the player is attacking (Left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            
            lastClickTime = Time.time;
            noOfClicks++;

            GameObject meleeAttack = activateMeleeAttackObject();

            if(noOfClicks == 1){
                meleeAttack.GetComponent<Animator>().SetBool("Hit1", true);
                animator.SetFloat("Hit", 1.0f);
            }


            if(noOfClicks == 2){
                meleeAttack.GetComponent<Animator>().SetBool("Hit2", true);
                animator.SetFloat("Hit", 2.0f);
            }


            if(noOfClicks == 3){
                meleeAttack.GetComponent<Animator>().SetBool("Hit3", true);
                animator.SetFloat("Hit", 3.0f);
                noOfClicks = 0;
            }


        }


    }

    GameObject activateMeleeAttackObject(){
        Debug.Log("Player attacking");

        // Creates a new attack object
        GameObject meleeAttack = Instantiate(meleeAttackObject.gameObject, meleeAttackObject.position, meleeAttackObject.rotation);

        // Get the mouse position in screen space and convert it to world space using the correct z-depth.
        mousePosition = Input.mousePosition;

        // Convert the mouse and target positions to 2D space (XZ plane)
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y); // Use X and Z for 2D calculation

        // Uses the cursor position to determine the direction of the attack, the same way it is used to determine the direction of the player

        //Converts the coordinates so that the point of origin is the center of the screen
        Vector2 targetPosition2D = new Vector2(Screen.width / 2, Screen.height / 2);

        Vector2 finalPosition2D = mousePosition2D - targetPosition2D;

        int X_value = (int)finalPosition2D.x;
        int Y_value = (int)finalPosition2D.y;

        int X_abs = Mathf.Abs(X_value);
        int Y_abs = Mathf.Abs(Y_value);

        if(X_abs > Y_abs){
            // Horizontal attack

            if(X_value > 0){
                Debug.Log("Right");
                // 1. Moves the attack object to the right of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x + 0.65f, meleeAttackObject.position.y, transform.position.z);

                //sets melee attack object rotation to (90, 0, 270)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 270);

                meleeAttack.gameObject.SetActive(true);

            } else {
                Debug.Log("Left");

                // 1. Moves the attack object to the left of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x - 0.65f, meleeAttackObject.position.y, transform.position.z);

                //sets melee attack object rotation to (90, 0, 90)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 90);

                meleeAttack.gameObject.SetActive(true);
            }

        } else {
            // Vertical attack
            if(Y_value > 0){
                Debug.Log("Up");

                // 1. Moves the attack object to the top of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x,  meleeAttackObject.position.y, transform.position.z+0.65f);
                
                //sets melee attack object rotation to (90, 0, 0)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 0);

                meleeAttack.gameObject.SetActive(true);

            } else {
                Debug.Log("Down");

                // 1. Moves the attack object to the bottom of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x,  meleeAttackObject.position.y, transform.position.z-0.65f);

                //sets melee attack object rotation to (90, 0, 0)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 180);

                meleeAttack.gameObject.SetActive(true);

            }
        }

        return meleeAttack;
    }

    // Manage ranged attack
    void rangedAttack(){

    }
    
    // Collision detection TODO: Add health system, damage, invincibility frames and hit animation
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Player hit by enemy");



        }
    }

}
