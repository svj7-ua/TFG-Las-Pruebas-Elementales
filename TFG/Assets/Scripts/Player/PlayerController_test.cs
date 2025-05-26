using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController_test : MonoBehaviour
{

    public float speed;
    public float dashSpeed = 10.0f; // Dash speed multiplier
    public float dashDuration = 0.1f; // Duration of the dash
    private Vector3 dashDirection; // Direction of the dash
    public float dashCooldown = 0.02f;
    [SerializeField]
    public LayerMask collisionMask;

    private bool canDash = true; // Flag to check if the player can dash
    public float groundDistance;

    public LayerMask groundMask;
    public Rigidbody rigidBody;

    private Animator animator;
    private Vector3 moveDirection;
    private bool isDashing = false;
    public GameObject meleeAttackObject;

    public GameObject rangedAttackObject;

    public float attackDuration = 0.583f;
    private float nextAttackTime = 0.25f;
    public static int noOfClicks = 0;
    float lastClickTime = 0f;
    float maxComboTime = 0.8f;

    private float meleeAttackInstantiateOffset = 2f;

    public float groudDistanceAcepetance;

    [SerializeField]
    GameObject pointerArrow;

    [SerializeField]
    GameObject InventoryUI;

    [SerializeField]
    GameObject dashAOE_Area;

    [SerializeField]
    float rangedAttackCooldown = 3.0f; // Cooldown for ranged attack
    [SerializeField]
    float dashAOE_Cooldown = 10.0f; // Cooldown for dash AOE attack

    private bool isRangedAttackOnCooldown = false; // Flag to check if ranged attack is on cooldown
    private bool isDashAOEOnCooldown = false; // Flag to check if dash AOE attack is on cooldown

    bool isPaused = false;

    [Header("DEBUG: Teclas de habilidades")]
    [SerializeField]
    bool DEBUG_MODE = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        OpenCloseInventory();

        if(!isPaused){
            Movement();
            if(!isDashing){

                // Movement
                //if(!isAttacking())  Movement();
                // Attack
                // Checks if the player can still continue the combo
                if(Time.time - lastClickTime > maxComboTime){
                    noOfClicks = 0;
                }
                if(!isAttackCooldown())    Attack();
                // Checks if the player can still continue the combo

                

            }

            // Dash
            if(!isAttacking() && canDash){
                //Dash();
                if (Input.GetKeyDown(KeyCode.Space))
                {   
                    
                    dashDirection = GetMovementDirection(); // Get the movement direction based on input
                    StartCoroutine(DashCoroutine());
                    //StartCoroutine(DashCooldownTimer()); // Start the cooldown timer
                }
            }
            
            if(DEBUG_MODE){
                DEBUG_1_pressed(); // ConvokeLightning_Effect
                DEBUG_2_pressed(); // ElectricExplosion_Effect
                DEBUG_3_pressed(); // HealingArea_Effect
                DEBUG_4_pressed(); // PoisonEffect
                DEBUG_5_pressed(); // WildFire_Effect
                DEBUG_6_pressed(); // Fireball_Effect
            }
        }
        


    }

    void OpenCloseInventory(){

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                isPaused = true;
                Debug.Log("Game Paused");
                gameObject.GetComponent<InventoryController>().ListEffects();
                InventoryUI.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
                Debug.Log("Game Resumed");
                InventoryUI.SetActive(false);
            }
        }

    }

    Vector3 GetMovementDirection(){
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        return new Vector3(x, 0, z).normalized;
    }

    void Movement()
    {
        RaycastHit hit;
        Vector3 castPosition = transform.position;
        //castPosition.y += 1;

        if(Physics.Raycast(castPosition, -transform.up, out hit, Mathf.Infinity, groundMask))
        {

            if(hit.collider != null && hit.distance <= groundDistance+groudDistanceAcepetance){
            
                moveDirection = GetMovementDirection(); // Get the movement direction based on input
                rigidBody.velocity = moveDirection * speed;

                
                setAnimator(moveDirection.x, moveDirection.z);  // Set the animator to the direction of the input so that the player faces the right direction
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

/*     void Dash()
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
            dashAOE_Area.SetActive(true); // Deactivate the dash area
            //animator.SetFloat("Dash", 1);
        }

        // Handle dashing
        if (isDashing)
        {

            rigidBody.velocity = moveDirection.normalized * dashSpeed;

            // Decrease dash timer
            dashTimer -= Time.deltaTime;

            // End dash when the timer runs out
            if (dashTimer <= 0)
            {   
                Debug.Log("End Dash");
                isDashing = false;
                dashAOE_Area.SetActive(false); // Deactivate the dash area
                //animator.SetFloat("Dash", 0);
            }
        }
    } */

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
                meleeAttack.GetComponentInChildren<Animator>().SetTrigger("Hit1");
                animator.SetFloat("Hit", 1.0f);
                
            }


            if(noOfClicks == 2){
                meleeAttack.GetComponentInChildren<Animator>().SetTrigger("Hit2");
                animator.SetFloat("Hit", 2.0f);
            }


            if(noOfClicks == 3){
                meleeAttack.GetComponentInChildren<Animator>().SetTrigger("Hit3");
                animator.SetFloat("Hit", 1.0f);
                noOfClicks = 0;
            }

            
            //Starts the attack animation
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.0f);


        }


    }

    GameObject activateMeleeAttackObject(){
        Debug.Log("Player attacking: Melee attack");

        // Creates a new attack object
        GameObject meleeAttack = Instantiate(meleeAttackObject, meleeAttackObject.transform.position, meleeAttackObject.transform.rotation);
        //GameObject meleeAttack = Instantiate(meleeAttackObject, meleeAttackObject.transform.position, Quaternion.Euler(90, 0, -pointerArrow.transform.eulerAngles.y));
        meleeAttack.transform.rotation = Quaternion.Euler(0, pointerArrow.transform.eulerAngles.y, 0);
        setDirrectionOfAttackAnimation();

        // switch (direction)
        // {
        //     case 0:
        //         meleeAttack.transform.position = new Vector3(transform.position.x,  meleeAttackObject.transform.position.y, transform.position.z+meleeAttackInstantiateOffset);
                
        //         //sets melee attack object rotation to (90, 0, 0)
        //         meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 0);
        //         break;
        //     case 1:
        //         meleeAttack.transform.position = new Vector3(transform.position.x + meleeAttackInstantiateOffset, meleeAttackObject.transform.position.y, transform.position.z);

        //         //sets melee attack object rotation to (90, 0, 270)
        //         meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 270);
        //         break;
        //     case 2:
        //         meleeAttack.transform.position = new Vector3(transform.position.x,  meleeAttackObject.transform.position.y, transform.position.z-meleeAttackInstantiateOffset);

        //         //sets melee attack object rotation to (90, 0, 0)
        //         meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 180);
        //         break;
        //     case 3:
        //         meleeAttack.transform.position = new Vector3(transform.position.x - meleeAttackInstantiateOffset, meleeAttackObject.transform.position.y, transform.position.z);

        //         //sets melee attack object rotation to (90, 0, 90)
        //         meleeAttack.transform.rotation = Quaternion.Euler(90, 0, 90);
        //         break;
        // }

        meleeAttack.SetActive(true);

        return meleeAttack;
    }

    private Vector3 getDirrectionOfMouse(){

        float angle = pointerArrow.transform.eulerAngles.y;

        if ((angle >= 337.5f && angle < 360f) || (angle >= 0f && angle < 22.5f))
        {
            return new Vector3(0, 0, 1); // UP
        }
        else if (angle >= 22.5f && angle < 67.5f)
        {
            return new Vector3(1, 0, 1); // UP-RIGHT
        }
        else if (angle >= 67.5f && angle < 112.5f)
        {
            return new Vector3(1, 0, 0); // RIGHT
        }
        else if (angle >= 112.5f && angle < 157.5f)
        {
            return new Vector3(1, 0, -1); // DOWN-RIGHT
        }
        else if (angle >= 157.5f && angle < 202.5f)
        {
            return new Vector3(-1, 0, 0); // DOWN
        }
        else if (angle >= 202.5f && angle < 247.5f)
        {
            return new Vector3(-1, 0, -1); // DOWN-LEFT
        }
        else if (angle >= 247.5f && angle < 292.5f)
        {
            return new Vector3(-1, 0, 0); // LEFT
        }
        else
        {
            return new Vector3(-1, 0, 1); // UP-LEFT
        }

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
            if(isRangedAttackOnCooldown){
                //TODO: Maybe add some visual feedback to the player
                Debug.Log("Ranged attack on cooldown");
                return; // If the ranged attack is on cooldown, do not execute the attack
            }
            // Check if the player is attacking (Right mouse button)
            Debug.Log("Player attacking: Ranged attack");
            lastClickTime = Time.time;

            setDirrectionOfAttackAnimation();
            animator.SetBool("Hit_Range", true);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.0f);

            // Creates a new attack object
            GameObject rangedAttack = Instantiate(rangedAttackObject, transform.position, Quaternion.Euler(90, 0, -pointerArrow.transform.eulerAngles.y));

            rangedAttack.SetActive(true);
            StartCoroutine(RangedAttackCooldownTimer()); // Start the cooldown timer

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

    public void AddEffectToInventory(IEffect effect, EnumSpellCardTypes effectType){

        switch (effectType)
        {
            case EnumSpellCardTypes.Melee:
                gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
                break;
            case EnumSpellCardTypes.Ranged:
                gameObject.GetComponent<InventoryController>().AddRangedEffect(effect);
                break;
            case EnumSpellCardTypes.Dash_AOE:
                gameObject.GetComponent<InventoryController>().AddDashEffect(effect);
                break;
            default:
                Debug.LogError("Error: Effect type not found");
                break;
        }

    }

    void DEBUG_1_pressed(){

        if (Input.GetKeyDown(KeyCode.Alpha1) && DEBUG_MODE){
            Debug.Log("DEBUG_1_pressed");
            IEffect effect = ScriptableObject.CreateInstance<ConvokeLightning>();
            gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
        }

    }

    void DEBUG_2_pressed(){

        if (Input.GetKeyDown(KeyCode.Alpha2) && DEBUG_MODE){
            Debug.Log("DEBUG_2_pressed");
            ElectricExplosion effect = ScriptableObject.CreateInstance<ElectricExplosion>();
            gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
        }

    }

    void DEBUG_3_pressed(){

        if (Input.GetKeyDown(KeyCode.Alpha3) && DEBUG_MODE){
            Debug.Log("DEBUG_3_pressed");
            HealingArea effect = ScriptableObject.CreateInstance<HealingArea>();
            gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
        }
    }

    void DEBUG_4_pressed(){

        if (Input.GetKeyDown(KeyCode.Alpha4) && DEBUG_MODE){
            Debug.Log("DEBUG_4_pressed");
            PoisonPuddle effect = ScriptableObject.CreateInstance<PoisonPuddle>();
            gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
        }
    }

    void DEBUG_5_pressed(){

        if (Input.GetKeyDown(KeyCode.Alpha5) && DEBUG_MODE){
            Debug.Log("DEBUG_5_pressed");
            WildFire effect = ScriptableObject.CreateInstance<WildFire>();
            gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
        }

    }

    void DEBUG_6_pressed(){

        if (Input.GetKeyDown(KeyCode.Alpha6) && DEBUG_MODE){
            Debug.Log("DEBUG_6_pressed");
            // GameObject effectObject = new GameObject("FireballEffect");
            // FireExplosion effect = effectObject.AddComponent<FireExplosion>();
            // gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
            FireExplosion effect = ScriptableObject.CreateInstance<FireExplosion>();
            gameObject.GetComponent<InventoryController>().AddMeleeEffect(effect);
        }
    }

// COROUTINES

    IEnumerator DashCoroutine()
    {
        //canDash = false;
        isDashing = true;
        if(!isDashAOEOnCooldown){
            dashAOE_Area.SetActive(true);
            StartCoroutine(DashAOECooldownTimer()); // Start the cooldown timer
        }

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            Vector3 move = dashDirection * dashSpeed * Time.deltaTime;

            // Raycast para detectar colisiones al avanzar
            if (!Physics.Raycast(transform.position, dashDirection, move.magnitude + 1.0f, collisionMask))
            {
                rigidBody.MovePosition(rigidBody.position + move);
            }
            else
            {
                Debug.Log("Dash detenido por colisión");
                break; // Nos detenemos si hay algo en el camino
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        dashAOE_Area.SetActive(false);


    }

    IEnumerator DashCooldownTimer()
    {
        yield return new WaitForSeconds(dashCooldown); // Ahora esto es independiente
        Debug.Log("Dash cooldown finished");
        canDash = true;
    }

    IEnumerator RangedAttackCooldownTimer()
    {
        isRangedAttackOnCooldown = true;
        yield return new WaitForSeconds(rangedAttackCooldown); // Ahora esto es independiente
        isRangedAttackOnCooldown = false; // Resetear el cooldown
        Debug.Log("Ranged attack cooldown finished");
    }

    IEnumerator DashAOECooldownTimer()
    {
        isDashAOEOnCooldown = true;
        yield return new WaitForSeconds(dashAOE_Cooldown); // Ahora esto es independiente
        isDashAOEOnCooldown = false; // Resetear el cooldown
        Debug.Log("Dash AOE cooldown finished");
    }
}