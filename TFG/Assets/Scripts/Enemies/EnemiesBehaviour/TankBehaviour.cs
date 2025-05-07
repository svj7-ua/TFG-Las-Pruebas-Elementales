using System.ComponentModel;
using UnityEngine;
using System.Collections;

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
    private float rangedAttackDuration = 0.4f; // Duration of the attack animation in seconds

    [SerializeField]
    private float meleAttackCooldown = 1f; // Cooldown duration for attacks
    [SerializeField]
    private float rangedAttackCooldown = 3f; // Cooldown duration for ranged attacks

    [SerializeField]
    private bool spawnsOnPlayer = false;
    [SerializeField]
    private float attack_yOffset = 0.3f; // Y offset for the attack prefab instantiation

    private bool isTargetInRange = false; // Flag to check if the target is in range
    private bool isTargetInStoppingDistance = false; // Flag to check if the target is in stopping distance

    private bool rangedAttackReady = true; // Flag to check if the ranged attack is ready
    private bool meleeAttackReady = true; // Flag to check if the melee attack

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
        attackRange = enemyReferences.navMeshAgent.stoppingDistance; // Get the stopping distance from the NavMeshAgent
    }

    // Update is called once per frame
    void Update()
    {

        if(target == null) return; // Check if the target is assigned
        if(hurtbox.IsStunned()) return; // Check if the enemy is currently attacking

        isTargetInStoppingDistance = Vector3.Distance(transform.position, target.position) <= attackRange;
        isTargetInRange = Vector3.Distance(transform.position, target.position) <= rangedAttackRange; // Check if the target is within attack range

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

        if (enemyReferences.attackPrefabs.Length == 0) return; // Check if there are attack prefabs assigned
        
        if (isTargetInStoppingDistance && meleeAttackReady) // Check if the target is in stopping distance and if the melee attack is ready
        {
            meleeAttackReady = false; // Set the melee attack flag to false to prevent immediate re-attack
            enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation

            Debug.Log("Golem Melee Attack!"); // Log the melee attack action

            //Now it would instantiate the attack prefab at the enemy's position (at floor level: y = 0.1f)
            //StartCoroutine(InstantiateAttackPrefab(1, new Vector3(transform.position.x, attack_yOffset, transform.position.z), Quaternion.identity, attackDuration)); // Call the Attack method of the enemy behaviour to instantiate the attack prefab
            StartCoroutine(MeleeAttackCooldown(meleAttackCooldown)); // Start the cooldown coroutine
        } else if (isTargetInRange && rangedAttackReady) // Check if the target is in range and if the ranged attack is ready
        {
            rangedAttackReady = false; // Set the ranged attack flag to false to prevent immediate
            enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation

            Debug.Log("Golem Ranged Attack!"); // Log the ranged attack action
            
            StartCoroutine(InstantiateAttackPrefab(0, new Vector3(transform.position.x, attack_yOffset, transform.position.z), CalculateAttackDirection(), rangedAttackDuration)); // Call the Attack method of the enemy behaviour to instantiate the attack prefab
            StartCoroutine(RangedAttackCooldown(rangedAttackCooldown)); // Start the cooldown coroutine
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

        //Rotates the quaternion 90 degrees to the left (MAY CHANGE LATER, SINCE IT'S LIKE THAT BECAUSE OF THE ATTACK PREFAB)
        rotation = Quaternion.Euler(0f, rotation.eulerAngles.y - 90f, 0f);

        Debug.Log("Flat Rotation: " + rotation);
        return rotation;
    }

    private IEnumerator InstantiateAttackPrefab(int prefabIndex, Vector3 position, Quaternion rotation, float attackDuration)
    {
        yield return new WaitForSeconds(0.6f); // Wait for the attack duration
        GameObject attackPrefab = Instantiate(enemyReferences.attackPrefabs[prefabIndex], position, rotation); // Instantiate the attack prefab at the specified position and rotation
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
}