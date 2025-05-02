using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;

    [SerializeField] public float pathUpdateDelay = 0.2f; // Delay between path updates in seconds

    [HideInInspector]
    public bool isAttacking = false; // Flag to check if the enemy is attacking
    [HideInInspector]
    public bool attackOnCooldown = false; // Flag to check if the enemy can attack
    [HideInInspector]
    public bool canAttack = false; // Flag to check if the enemy can attack

    [Header("Attack Prefab(s)")]
    [SerializeField]
    public GameObject[] attackPrefabs; // Array of attack prefabs

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();              // Uncomment if you have an Animator component
    }

    public void SetAttacking()
    {
        // Starts a coroutine to set the isAttacking flag to true for a short duration
        //StartCoroutine(SetAttackingCoroutine(0.3f)); // Set the duration as needed
        //StartCoroutine(StartAttackCooldownCoroutine(1f)); // Set the cooldown duration as needed
    }

}
