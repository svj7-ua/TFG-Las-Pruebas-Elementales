using System.Collections;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    
    public Transform target;

    private EnemyReferences enemyReferences;

    private float pathUpdateDeadline = 0f; // Delay between path updates in seconds

    [Header("Attacks Settings")]
    private float attackRange; // Range of the attack in units

    [SerializeField]
    private float attackDuration = 0.3f; // Duration of the attack animation in seconds

    [SerializeField]
    private float meleAttackCooldown = 1f; // Cooldown duration for attacks

    [SerializeField]
    private bool spawnsOnPlayer = false;
    [SerializeField]
    private float attack_yOffset = 0.3f; // Y offset for the attack prefab instantiation
    private Vector3 attackPosition; // Position where the attack prefab will be instantiated
    private bool isTargetInStoppingDistance = false; // Flag to check if the target is in stopping distance
    private bool meleeAttackReady = true; // Flag to check if the melee attack

    private bool inStunnedCooldown = false; // Flag to check if the enemy is in stunned cooldown

    private Hurtbox hurtbox; // Reference to the Hurtbox component  

    // Used when the enemy was summoned
    private GameObject lord;

    private GameObject widow;

    private void Awake()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        target = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the target is tagged as "Player"
        hurtbox = GetComponent<Hurtbox>(); // Get the Hurtbox component
    }

    // Start is called before the first frame update
    void Start()
    {
        isTargetInStoppingDistance = false; // Initialize the target in stopping distance flag
        attackRange = enemyReferences.navMeshAgent.stoppingDistance; // Get the stopping distance from the NavMeshAgent
    }

    // Update is called once per frame
    void Update()
    {
        if (lord != null) CheckLord();
        if (widow != null) CheckWidow();

        if (target == null) return; // Check if the target is assigned
        if(hurtbox.IsStunned()){
            if(inStunnedCooldown) StopCoroutine("StartStunCooldown"); // Stop the stun cooldown coroutine if the enemy is already in stunned cooldown
            StartCoroutine(StartStunCooldown(1.5f));
            return;
        } // Check if the enemy is currently attacking
        if (hurtbox.IsTrapped()) return;

        if (enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return; // Check if the enemy is currently attacking

        isTargetInStoppingDistance = Vector3.Distance(transform.position, target.position) <= attackRange;

        if(!isTargetInStoppingDistance){
            UpdatePath(); // Update the path if the target is not in range
        }

        Attack(); // Call the Attack method to check for attacks


    }
    
    private void CheckLord()
    {
        if (hurtbox.health.currentHealth <= 0) // Check if the enemy is dead
        {
            lord.GetComponent<LordBehaviours>().RemoveSlime(gameObject); // Remove the enemy from the lord's list
            return; // Exit the method
        }
        if (lord == null) Destroy(gameObject); // Destroy the object if the lord is null (Would happen if the lord is destroyed, but didn't delete the object)
        if (lord.activeSelf == false) Destroy(gameObject); // Destroy the object if the lord is not active (Would happen if the lord is destroyed, but didn't delete the object)
    }

    public void SetLord(GameObject lordObject)
    {
        lord = lordObject; // Set the lord object
        if (lord == null) Debug.LogError("Lord object is null!"); // Log an error if the lord object is null
    }

    private void CheckWidow()
    {

        if (hurtbox.health.currentHealth <= 0) // Check if the enemy is dead
        {
            widow.GetComponent<WidowBehaviours>().RemoveSlime(gameObject); // Remove the enemy from the widow's list
            return; // Exit the method
        }

        if (widow == null) return; // Check if the widow is assigned
        if (widow.activeSelf == false) Destroy(gameObject); // Destroy the object if the widow is not active (Would happen if the widow is destroyed, but didn't delete the object)
    }

    public void SetWidow(GameObject widowObject)
    {
        widow = widowObject; // Set the widow object
        if (widow == null) Debug.LogError("Widow object is null!"); // Log an error if the widow object is null
    }

    public void UpdatePath()
    {

        if (Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
            if (enemyReferences.navMeshAgent.enabled) enemyReferences.navMeshAgent.SetDestination(target.position);
        }

    }

    private void Attack()
    {

        if(hurtbox.IsStunned()) return; // Check if the enemy is currently attacking
        if (enemyReferences.attackPrefabs.Length == 0) return; // Check if there are attack prefabs assigned
        
        if (isTargetInStoppingDistance && meleeAttackReady) // Check if the target is in stopping distance and if the melee attack is ready
        {
            enemyReferences.navMeshAgent.velocity = Vector3.zero; // Stop the enemy's movement
            meleeAttackReady = false; // Set the melee attack flag to false to prevent immediate re-attack
            enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation
            attackPosition = new Vector3(target.position.x, attack_yOffset, target.position.z); // Set the attack position based on the enemy's position
            Debug.Log("Slime Melee Attack! PlayerPosition: " + target.position + " AttackPosition: " + attackPosition); // Log the melee attack action

            //StartCoroutine(InstantiateAttack()); // Start the coroutine to instantiate the attack prefab
            StartCoroutine(MeleeAttackCooldown(meleAttackCooldown)); // Start the cooldown coroutine


        }

    }

    public void InstantiateAttack()
    {

        if (!enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return; // Check if the enemy is currently attacking
        GameObject attack;

        if(spawnsOnPlayer) // Check if the attack is targeting the player
        {
            attack = Instantiate(enemyReferences.attackPrefabs[0], new Vector3(attackPosition.x, attack_yOffset, attackPosition.z), Quaternion.identity);// Call the Attack method of the enemy behaviour to instantiate the attack prefab
        }
        else
        {
            attack = Instantiate(enemyReferences.attackPrefabs[0], new Vector3(transform.position.x, attack_yOffset, transform.position.z), Quaternion.identity);// Call the Attack method of the enemy behaviour to instantiate the attack prefab // Call the Attack method of the enemy behaviour to instantiate the attack prefab
        }
        // Destroy the attack after the specified duration (attackDuration - Default: 0.3f)
        Destroy(attack, attackDuration); // Adjust the duration as needed
    }

    private IEnumerator MeleeAttackCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        meleeAttackReady = true; // Set the melee attack flag to true after cooldown
    }

    private IEnumerator StartStunCooldown(float stunDuration)
    {

        //if(inStunnedCooldown) yield break; // Exit the coroutine if the enemy is already in stunned cooldown
        //inStunnedCooldown = true; // Set the stunned cooldown flag to true
        if(!meleeAttackReady) yield break; // Exit the coroutine if the enemy is not ready to attack
        meleeAttackReady = false; // Set the melee attack flag to false to prevent immediate re-attack

        inStunnedCooldown = true; // Set the stunned cooldown flag to true
        yield return new WaitForSeconds(stunDuration); // Wait for the stun duration
        inStunnedCooldown = false; // Set the stunned cooldown flag to false after the stun duration
        meleeAttackReady = true; // Set the melee attack flag to true after cooldown


    }

}