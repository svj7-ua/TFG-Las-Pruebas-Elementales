using System.ComponentModel;
using UnityEngine;
using System.Collections;
using System;

public class TankBehaviour : MonoBehaviour
{

    public Transform target;

    private EnemyReferences enemyReferences;

    private float pathUpdateDeadline = 0f; // Delay between path updates in seconds

    [Header("Attacks Settings")]
    [SerializeField]
    private float rangedAttackRange = 5f; // Range of the attack in units
    private float attackRange; // Range of the attack in units

    [SerializeField]
    private float attackDuration = 0.3f; // Duration of the attack in seconds

    [SerializeField]
    private float meleAttackCooldown = 1f; // Cooldown duration for attacks
    [SerializeField]
    private float rangedAttackCooldown = 3f; // Cooldown duration for ranged attacks

    // [SerializeField]
    // private bool spawnsOnPlayer = false;
    [SerializeField]
    private float attack_yOffset = 0.2f; // Y offset for the attack prefab instantiation

    private bool isTargetInRange = false; // Flag to check if the target is in range
    private bool isTargetInStoppingDistance = false; // Flag to check if the target is in stopping distance

    private bool rangedAttackReady = true; // Flag to check if the ranged attack is ready
    private bool meleeAttackReady = true; // Flag to check if the melee attack

    private bool inStunnedCooldown = false; // Flag to check if the enemy is in stunned cooldown

    private Hurtbox hurtbox; // Reference to the Hurtbox component

    private void Awake()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        hurtbox = GetComponent<Hurtbox>(); // Get the Hurtbox component
        target = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the target is tagged as "Player"
    }

    // Start is called before the first frame update
    void Start()
    {
        isTargetInRange = false; // Initialize the target in range flag
        isTargetInStoppingDistance = false; // Initialize the target in stopping distance flag
        attackRange = enemyReferences.navMeshAgent.stoppingDistance+1f; // Get the stopping distance from the NavMeshAgent
    }

    // Update is called once per frame
    void Update()
    {

        if(target == null) return; // Check if the target is assigned
        if(hurtbox.IsStunned()){
            if(inStunnedCooldown){
                StopCoroutine("StartStunCooldownMele");
                StopCoroutine("StartStunCooldownRanged");
            }
            StartCoroutine(StartStunCooldownMele(1.5f));
            StartCoroutine(StartStunCooldownRanged(1.5f));
            return;
        } // Check if the enemy is currently attacking
        if (hurtbox.IsTrapped()) return;

        float distance = Vector3.Distance(transform.position, target.position); // Calculate the distance to the target

        isTargetInStoppingDistance = distance <= attackRange;
        isTargetInRange = (attackRange+0.2f) <= distance && distance <= rangedAttackRange; // Check if the target is within attack range

        if(!isTargetInStoppingDistance){
            UpdatePath(); // Update the path if the target is not in range
        }

        Attack(); // Call the Attack method to check for attacks


    }

    public void UpdatePath()
    {

        if (Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
            if(enemyReferences.navMeshAgent.enabled)    enemyReferences.navMeshAgent.SetDestination(target.position);
        }

    }

    private void Attack()
    {
        if(hurtbox.IsStunned()) return; // Check if the enemy is currently stunned

        if (enemyReferences.attackPrefabs.Length == 0) return; // Check if there are attack prefabs assigned
        
        if (isTargetInStoppingDistance && meleeAttackReady) // Check if the target is in stopping distance and if the melee attack is ready
        {
            meleeAttackReady = false; // Set the melee attack flag to false to prevent immediate re-attack
            enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation

            Debug.Log("Golem Melee Attack!"); // Log the melee attack action

            //Now it would instantiate the attack prefab at the enemy's position (at floor level: y = 0.1f)
            Debug.Log("yOffset: " + attack_yOffset); // Log the y offset for debugging
            
            StartCoroutine(MeleeAttackCooldown(meleAttackCooldown)); // Start the cooldown coroutine
            StartCoroutine(CooldownBetweenAttacksMele(3f)); // Start the cooldown coroutine between attacks
        } else if (isTargetInRange && rangedAttackReady) // Check if the target is in range and if the ranged attack is ready
        {
            rangedAttackReady = false; // Set the ranged attack flag to false to prevent immediate
            enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation

            Debug.Log("Golem Ranged Attack!"); // Log the ranged attack action
            
            
            StartCoroutine(RangedAttackCooldown(rangedAttackCooldown)); // Start the cooldown coroutine
            StartCoroutine(CooldownBetweenAttacksRanged(3f)); // Start the cooldown coroutine between attacks
        }



    }

    public Quaternion CalculateAttackDirection()
    {

        Debug.Log("AttackDirection called in TankEnemy");
        Debug.Log("Origin: " + transform.position + " Target: " + target.position);

        // Calculates the direction in the XZ plane ignoring the Y axis
        Vector3 flatDirection = target.position - transform.position;
        flatDirection.y = 0f; // Removes the vertical component

        if (flatDirection == Vector3.zero)
        {
            // If the direction is zero, return the identity quaternion (no rotation)
            return Quaternion.identity;
        }

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg, 0f);

        Debug.Log("Flat Rotation: " + rotation);
        return rotation;
    }

    public void GolemAttack()
    {
        if (isTargetInRange)
        {
            RangedAttack(); // Call the ranged attack method if the target is in range
            return;
        }
        else
        {
            ImpactAttack(); // Call the impact attack method if the target is not in range
            
        }
    }

    private void ImpactAttack()
    {
        if (!enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return; // Exit the coroutine if the animation is not playing
        }

        GameObject attackPrefab = Instantiate(enemyReferences.attackPrefabs[0], new Vector3(transform.position.x, attack_yOffset, transform.position.z), Quaternion.identity); // Instantiate the attack prefab at the specified position and rotation
        Destroy(attackPrefab, attackDuration); // Destroy the attack prefab after the specified duration (attackDuration - Default: 0.3f)

    }

    private void RangedAttack()
    {
        if(!enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return; // Exit the coroutine if the animation is not playing
        }

        GameObject attackPrefab = Instantiate(enemyReferences.attackPrefabs[1], new Vector3(transform.position.x, attack_yOffset, transform.position.z), CalculateAttackDirection()); // Instantiate the attack prefab at the specified position and rotation
        Destroy(attackPrefab, attackDuration); // Destroy the attack prefab after the specified duration (attackDuration - Default: 0.3f)
    }

    private IEnumerator MeleeAttackCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        meleeAttackReady = true; // Set the melee attack flag to true after cooldown
    }

    private IEnumerator RangedAttackCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        rangedAttackReady = true; // Set the ranged attack flag to true after cooldown
    }

    private IEnumerator CooldownBetweenAttacksMele(float cooldownTime)
    {

        if(!rangedAttackReady) // Check if the ranged attack is ready
        {
            // If it's not ready, doen't wait for the cooldown time
            yield break; // Exit the coroutine if the ranged attack is not ready
        }
        rangedAttackReady = false; // Set the ranged attack flag to false to prevent immediate re-attack

        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time

        rangedAttackReady = true; // Set the ranged attack flag to true after cooldown
        
    }

    private IEnumerator CooldownBetweenAttacksRanged(float cooldownTime)
    {

        if(!meleeAttackReady) // Check if the ranged attack is ready
        {
            // If it's not ready, doen't wait for the cooldown time
            yield break; // Exit the coroutine if the ranged attack is not ready
        }
        meleeAttackReady = false; // Set the ranged attack flag to false to prevent immediate re-attack

        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time

        meleeAttackReady = true; // Set the ranged attack flag to true after cooldown
        
    }

    // This method adds a small cooldown to the enemy attacks when it is stunned, so it doesn't spam the attack animation just after being unstunned
    private IEnumerator StartStunCooldownMele(float stunDuration)
    {


        if(!meleeAttackReady) yield break; // Exit the coroutine if the enemy is already in stunned cooldown
        meleeAttackReady = false; // Set the stunned cooldown flag to true
        inStunnedCooldown = true; // Set the stunned flag to true
        yield return new WaitForSeconds(stunDuration); // Wait for the stun duration
        inStunnedCooldown = false; // Set the stunned cooldown flag to false after the stun duration
        meleeAttackReady = true; // Set the melee attack flag to true after cooldown

    }

        private IEnumerator StartStunCooldownRanged(float stunDuration)
    {

        if(!rangedAttackReady) yield break; // Exit the coroutine if the enemy is already in stunned cooldown
        rangedAttackReady = false; // Set the stunned cooldown flag to true
        inStunnedCooldown = true; // Set the stunned flag to true
        yield return new WaitForSeconds(stunDuration); // Wait for the stun duration
        inStunnedCooldown = false; // Set the stunned cooldown flag to false after the stun duration
        rangedAttackReady = true; // Set the melee attack flag to true after cooldown

    }
}