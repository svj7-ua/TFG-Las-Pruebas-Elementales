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

    public GameObject rangedAttackObject;

    public float attackDuration = 0.583f;
    private float nextAttackTime = 0.25f;
    public static int noOfClicks = 0;
    float lastClickTime = 0f;
    float maxComboTime = 0.8f;

    private float meleeAttackInstantiateOffset = 2.35f;

    public float groudDistanceAcepetance;

    [SerializeField]
    GameObject pointerArrow;


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
        if(!isDashing){

            // Movement
            //if(!isAttacking())  Movement();
            // Attack
            // Checks if the player can still continue the combo
            if(Time.time - lastClickTime > maxComboTime){
                Debug.Log("Reset combo");
                noOfClicks = 0;
            }
            if(!isAttackCooldown())    Attack();
            // Checks if the player can still continue the combo

            

        }

        // Dash
        if(!isAttacking())    Dash();
        
        //checkIfAttackAnimationEnded();
        


    }

    void Movement()
    {
        RaycastHit hit;
        Vector3 castPosition = transform.position;
        //castPosition.y += 1;

        if(Physics.Raycast(castPosition, -transform.up, out hit, Mathf.Infinity, groundMask))
        {

            if(hit.collider != null && hit.distance <= groundDistance+groudDistanceAcepetance){
            

                float x = Input.GetAxisRaw("Horizontal");
                float z = Input.GetAxisRaw("Vertical");
                moveDirection = new Vector3(x, 0, z).normalized;
                rigidBody.velocity = moveDirection * speed;

                
                setAnimator(x, z);  // Set the animator to the direction of the input so that the player faces the right direction
            }

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

    bool isAttackCooldown(){
        if (Time.time - lastClickTime > nextAttackTime)
        {
            return false;
        } else {
            return true;
        }
    }

    bool isAttacking(){
        if (Time.time - lastClickTime > attackDuration)
        {
            animator.SetFloat("Hit", 0.0f);
            animator.SetBool("Hit_Range", false);

            return false;
        } else {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            //animator.SetFloat("Speed", 0);
            return true;
        }
    }

    bool checkIfAttackAnimationEnded(){

        if (Time.time - lastClickTime >= attackDuration)
        {
            animator.SetFloat("Hit", 0.0f);
            animator.SetBool("Hit_Range", false);
            
            return true;
        } else {
            return false;
        }
    }

    // Manage mele attack
    void meleAttack(){

        // Check if the player is attacking (Left mouse button)
        if (Input.GetMouseButtonDown(0))
        {

            lastClickTime = Time.time;
            noOfClicks++;
            animator.SetBool("Hit_Range", false);

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
                animator.SetFloat("Hit", 1.0f);
                noOfClicks = 0;
            }

            
            //Starts the attack animation
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.0f);


        }


    }

    private Vector2 getMousePosition(){
        // Get the mouse position in screen space and convert it to world space using the correct z-depth.
        mousePosition = Input.mousePosition;

        // Convert the mouse and target positions to 2D space (XZ plane)
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y); // Use X and Z for 2D calculation

        // Uses the cursor position to determine the direction of the attack, the same way it is used to determine the direction of the player

        //Converts the coordinates so that the point of origin is the center of the screen
        Vector2 targetPosition2D = new Vector2(Screen.width / 2, Screen.height / 2);

        return mousePosition2D - targetPosition2D;

    }

    GameObject activateMeleeAttackObject(){
        Debug.Log("Player attacking: Melee attack");

        // Creates a new attack object
        GameObject meleeAttack = Instantiate(meleeAttackObject.gameObject, meleeAttackObject.position, meleeAttackObject.rotation);

        Vector2 finalPosition2D = getMousePosition();       //TODO: Substitute to use arrow pointer rotation to determine the direction of the attack

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

                meleeAttack.transform.position = new Vector3(transform.position.x + meleeAttackInstantiateOffset, meleeAttackObject.position.y, transform.position.z);

                //sets melee attack object rotation to (90, 0, 270)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 270);

                meleeAttack.gameObject.SetActive(true);

                //3. Sets de direction of the attack for the player
                animator.SetFloat("Hit_Direction", 4.0f);   // Right

                //4. Sets direction for the player to face after the attack
                animator.SetFloat("Horizontal_player", 1.0f);
                animator.SetFloat("Vertical_player", 0.0f);

            } else {
                Debug.Log("Left");

                // 1. Moves the attack object to the left of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x - meleeAttackInstantiateOffset, meleeAttackObject.position.y, transform.position.z);

                //sets melee attack object rotation to (90, 0, 90)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 90);

                meleeAttack.gameObject.SetActive(true);

                //3. Sets de direction of the attack for the player
                animator.SetFloat("Hit_Direction", 3.0f);   // Left

                //4. Sets direction for the player to face after the attack
                animator.SetFloat("Horizontal_player", -1.0f);
                animator.SetFloat("Vertical_player", 0.0f);
            }

        } else {
            // Vertical attack
            if(Y_value > 0){
                Debug.Log("Up");

                // 1. Moves the attack object to the top of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x,  meleeAttackObject.position.y, transform.position.z+meleeAttackInstantiateOffset);
                
                //sets melee attack object rotation to (90, 0, 0)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 0);

                meleeAttack.gameObject.SetActive(true);

                //3. Sets de direction of the attack for the player
                animator.SetFloat("Hit_Direction", 1.0f);   // Up

                //4. Sets direction for the player to face after the attack
                animator.SetFloat("Horizontal_player", 0.0f);
                animator.SetFloat("Vertical_player", 1.0f);

            } else {
                Debug.Log("Down");

                // 1. Moves the attack object to the bottom of the player
                // 2. Starts the attack animation

                meleeAttack.transform.position = new Vector3(transform.position.x,  meleeAttackObject.position.y, transform.position.z-meleeAttackInstantiateOffset);

                //sets melee attack object rotation to (90, 0, 0)
                meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 180);

                meleeAttack.gameObject.SetActive(true);

                //3. Sets de direction of the attack for the player
                animator.SetFloat("Hit_Direction", 2.0f);   // Down

                //4. Sets direction for the player to face after the attack
                animator.SetFloat("Horizontal_player", 0.0f);
                animator.SetFloat("Vertical_player", -1.0f);                

            }
        }

        return meleeAttack;
    }

    // Sets the direction of the attack animation and the direction the player will face after the attack
    // Returns:
    // 0 - Up
    // 1 - Right
    // 2 - Down
    // 3 - Left
    private int setDirrectionOfAttackAnimation(){

        float angle = pointerArrow.transform.eulerAngles.y;

        if (angle < 45.0f || angle > 315.0f){
            //UP
            // Sets de direction of the attack for the player
            animator.SetFloat("Hit_Direction", 1.0f);   // Up

            // Sets direction for the player to face after the attack
            animator.SetFloat("Horizontal_player", 0.0f);
            animator.SetFloat("Vertical_player", 1.0f);
            return 0;
        } else if (angle >= 45.0f && angle <= 135.0f){
            //RIGHT
            // Sets de direction of the attack for the player
            animator.SetFloat("Hit_Direction", 4.0f);   // Right

            // Sets direction for the player to face after the attack
            animator.SetFloat("Horizontal_player", 1.0f);
            animator.SetFloat("Vertical_player", 0.0f);
            return 1;

        } else if (angle > 135.0f && angle < 225.0f){
            //DOWN
            // Sets de direction of the attack for the player
            animator.SetFloat("Hit_Direction", 2.0f);   // Down

            // Sets direction for the player to face after the attack
            animator.SetFloat("Horizontal_player", 0.0f);
            animator.SetFloat("Vertical_player", -1.0f);
            return 2;
        } else {
            //LEFT
            // Sets de direction of the attack for the player
            animator.SetFloat("Hit_Direction", 3.0f);   // Left

            // Sets direction for the player to face after the attack
            animator.SetFloat("Horizontal_player", -1.0f);
            animator.SetFloat("Vertical_player", 0.0f);
            return 3;
        }

    }

    // Manage ranged attack
    void rangedAttack(){

        // Creates the objecto to be thrown by the player pointing to the mouse direction

        // Check if the player is attacking (Right mouse button)

        if (Input.GetMouseButtonDown(1))
        {
            // Check if the player is attacking (Right mouse button)
            Debug.Log("Player attacking: Ranged attack");
            lastClickTime = Time.time;

            setDirrectionOfAttackAnimation();
            animator.SetBool("Hit_Range", true);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.0f);

            // Creates a new attack object
            GameObject rangedAttack = Instantiate(rangedAttackObject, transform.position, Quaternion.Euler(90, 0, -pointerArrow.transform.eulerAngles.y));

            rangedAttack.SetActive(true);

        }


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
