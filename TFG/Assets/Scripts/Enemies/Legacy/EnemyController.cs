using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Transform target;

    private EnemyReferences enemyReferences;

    private float stoppingDistance;

    private EnemyBehaviour enemyBehaviour; // Reference to the enemy behaviour script

    [Header("Enemy Behaviour Type (Used in the Factory)")]
    [SerializeField]
    EnumEnemyBehaviours enemyBehaviourType = EnumEnemyBehaviours.SimpleEnemie; // Type of enemy behaviour

    
    [Header("Attack Prefab(s)")]
    [SerializeField]
    public GameObject[] attackPrefabs; // Array of attack prefabs

    [SerializeField]
    private float attackCooldown = 1f; // Cooldown duration for attacks

    private void Awake()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        target = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the target is tagged as "Player"
    }

    // Start is called before the first frame update
    void Start()
    {
        stoppingDistance = enemyReferences.navMeshAgent.stoppingDistance;
        enemyBehaviour = EnemyBehaviourFactory.CreateEnemyBehaviour(enemyReferences, target, transform, 0, enemyBehaviourType, attackCooldown); // Create the enemy behaviour using the factory
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the enemy is attacking, if not, call the Tick method of the enemy behaviour
        if(!enemyReferences.isAttacking) enemyBehaviour.Tick(); // Call the Tick method of the enemy behaviour (In charge of the pathfinding)
        if(enemyReferences.canAttack) Attack(); // Call the Attack method of the enemy controller
        
        enemyBehaviour.UpdateCooldown(); // Call the UpdateCooldown method of the enemy behaviour (In charge of the cooldown)

    }


    private void Attack()
    {

        if (enemyReferences.attackPrefabs.Length == 0) return; // Check if there are attack prefabs assigned
        if(enemyBehaviour.IsAttackReady() == false) return; // Check if the enemy can attack
        
        enemyBehaviour.StartAttackCooldown();
        enemyBehaviour.AttackAnimation(); // Call the AttackAnimation method of the enemy behaviour to trigger the attack animation

    }
}
