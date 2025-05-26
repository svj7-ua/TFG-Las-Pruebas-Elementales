using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossReferences : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;

    [HideInInspector] public Transform target; // Target for the boss to follow

    [HideInInspector] public Transform player;

    [HideInInspector] public SpriteRenderer spriteRenderer; // Sprite renderer for the boss

    [HideInInspector] public bool isAttacking = false; // Flag to check if the boss is attacking

    [SerializeField] public float pathUpdateDelay = 0.2f; // Delay between path updates in seconds

    [SerializeField]
    private EnumBosses bossType = EnumBosses.none; // Type of the boss

    [Header("Attack Prefab(s)")]
    [SerializeField]
    public GameObject[] attackPrefabs; // Array of attack prefabs

    [SerializeField]
    public int attacksPerRotaion = 3; // Number of attacks the boss can perform before switching to Idle

    private List<Transform> waypoints; // List of waypoints for the boss to follow

    [Header("Boss Summoning Circle")]

    [SerializeField]
    private Sprite bossSummoningCircleSprite; // Texture for the boss summoning circle

    [Header("Boss States")]
    [Tooltip("Initial value shuld generally be moving")]
    [SerializeField]
    private EnumBossesStates currentState = EnumBossesStates.Moving; // Current state of the boss

    [Header("Boss Name")]
    [SerializeField]
    private string bossName = "None"; // Name of the boss, used for debugging and UI

    private void Awake()
    {
        FindNavMeshAgent(); // Find the NavMeshAgent component

        FindAnimator(); // Find the animator component

        FindHealthBar(); // Find the health bar in the scene (And sets it up)

        FindSpriteRenderer(); // Find the SpriteRenderer component
    }

    private void FindSpriteRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Try to find the SpriteRenderer component in the child objects
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found!"); // Log an error if the SpriteRenderer component is not found
            }
        }
    }

    private void FindHealthBar()
    {

        switch (bossType)
        {
            case EnumBosses.LordOfPoison:
                GetComponent<Health>().healthBar = FindObjectOfType<UIReferences>().twinBossHealthBar2; // Find the health bar in the scene
                break;
            case EnumBosses.LordOfFire:
                GetComponent<Health>().healthBar = FindObjectOfType<UIReferences>().twinBossHealthBar1; // Find the health bar in the scene
                break;
            default:
                GetComponent<Health>().healthBar = FindObjectOfType<UIReferences>().bossHealthBar; // Find the health bar in the scene
                //Assign the boss health bar for solo bosses
                break;
        }

        if (GetComponent<Health>().healthBar != null)
        {
            GetComponent<Health>().healthBar.SetActive(true); // Set the health bar active
            GetComponent<Health>().healthBar.GetComponent<Slider>().maxValue = GetComponent<Health>().maxHealth; // Set the max value of the health bar to the max health
            GetComponent<Health>().healthBar.GetComponent<Slider>().value = GetComponent<Health>().currentHealth; // Set the current value of the health bar to the current health
        }
        else
        {
            Debug.LogError("Boss Health Bar not found!"); // Log an error if the health bar is not found
        }
    }

    private void FindAnimator()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        if (animator == null)
        {
            // animator = GetComponentInChildren<Animator>(); // Try to find the Animator component in the child objects
            // if (animator == null)
            // {
            //     Debug.LogError("Animator not found!"); // Log an error if the Animator component is not found
            // }

            Debug.LogError("Animator not found!"); // Log an error if the Animator component is not found
        }
    }

    private void FindNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        if (navMeshAgent == null)
        {
            // // Try to find the NavMeshAgent component in its parent
            // navMeshAgent = GetComponentInParent<NavMeshAgent>();
            // if (navMeshAgent == null)
            // {
            //     Debug.LogError("NavMeshAgent not found!"); // Log an error if the NavMeshAgent component is not found
            // }
            Debug.LogError("NavMeshAgent not found!"); // Log an error if the NavMeshAgent component is not found
        }
    }
    public void AddWaypoints(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints; // Assign the new waypoints to the boss
    }

    public List<Transform> GetWaypoints()
    {
        Debug.Log("Waypoints count: " + waypoints.Count); // Log the number of waypoints
        return waypoints; // Return the list of waypoints
    }

    public Sprite GetBossSummoningCircleSprite()
    {
        return bossSummoningCircleSprite; // Return the boss summoning circle texture
    }

    public EnumBosses GetBossType()
    {
        return bossType; // Return the boss type
    }

    public EnumBossesStates GetCurrentState()
    {
        return currentState; // Return the current state of the boss
    }

    public void SetCurrentState(EnumBossesStates newState)
    {
        Debug.Log("State changed from " + currentState + " to: " + newState); // Log the state change
        currentState = newState; // Set the current state of the boss
    }
    
    public string GetBossName()
    {
        return bossName; // Return the name of the boss
    }

}
