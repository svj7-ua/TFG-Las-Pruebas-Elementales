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

    private bool isTargetInStoppingDistance = false; // Flag to check if the target is in stopping distance
    private bool meleeAttackReady = true; // Flag to check if the melee attack

    private Hurtbox hurtbox; // Reference to the Hurtbox component  

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

        if(target == null) return; // Check if the target is assigned
        if(hurtbox.IsStunned()) return; // Check if the enemy is currently attacking

        if(enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return; // Check if the enemy is currently attacking

        isTargetInStoppingDistance = Vector3.Distance(transform.position, target.position) <= attackRange;

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
            //enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation
            
            Debug.Log("Slime Melee Attack!"); // Log the melee attack action

            GameObject attack;

            if(spawnsOnPlayer) // Check if the attack is targeting the player
            {
                attack = Instantiate(enemyReferences.attackPrefabs[0], new Vector3(target.transform.position.x, attack_yOffset, target.transform.position.z), Quaternion.identity);// Call the Attack method of the enemy behaviour to instantiate the attack prefab
            }
            else
            {
                attack = Instantiate(enemyReferences.attackPrefabs[0], new Vector3(transform.position.x, attack_yOffset, transform.position.z), Quaternion.identity);// Call the Attack method of the enemy behaviour to instantiate the attack prefab // Call the Attack method of the enemy behaviour to instantiate the attack prefab
            }

            StartCoroutine(MeleeAttackCooldown(meleAttackCooldown)); // Start the cooldown coroutine

            // Destroy the attack after the specified duration (attackDuration - Default: 0.3f)
            Destroy(attack, attackDuration); // Adjust the duration as needed

        }

    }

    private IEnumerator MeleeAttackCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        meleeAttackReady = true; // Set the melee attack flag to true after cooldown
    }

}