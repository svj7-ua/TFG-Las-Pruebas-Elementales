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
    private int attacksPerRotaion = 2; // Number of attacks the boss can perform before switching to Idle (Set as 2, since it will add the level of the boss to the attack prefab, so it will be 3 attacks in total)

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

    private GameObject bossRoom;

    private TwinBossesReferences twinBossReferences; // Reference to the Twin Bosses References script, if applicable

    private AudioManager audioManager; // Reference to the AudioManager script

    private void Awake()
    {
        if (attacksPerRotaion > 5)
        {
            attacksPerRotaion = 5; // Limit the number of attacks per rotation to 5
        }

        FindNavMeshAgent(); // Find the NavMeshAgent component

        FindAnimator(); // Find the animator component

        FindHealthBar(); // Find the health bar in the scene (And sets it up)

        FindSpriteRenderer(); // Find the SpriteRenderer component

        audioManager = FindObjectOfType<AudioManager>(); // Find the AudioManager in the scene
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!"); // Log an error if the AudioManager is not found
        }


    }

    void Start()
    {
        StartBossMusic(); // Start the boss music when the boss is initialized
    }

    public void IncrementAttacksPerRotation(int level)
    {
        Debug.LogWarning("Incrementing attacks per rotation by level: " + level); // Log the level being used to increment attacks
        attacksPerRotaion += level; // Increment the number of attacks per rotation by the level of the boss
        if (attacksPerRotaion > 5)
        {
            attacksPerRotaion = 5; // Limit the number of attacks per rotation to 5
        }
    }

    public int GetAttacksPerRotation()
    {
        return attacksPerRotaion; // Return the number of attacks per rotation
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

            Debug.LogError("Animator not found!"); // Log an error if the Animator component is not found
        }
    }

    private void FindNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        if (navMeshAgent == null)
        {
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

    public void SetBossRoom(GameObject room)
    {
        bossRoom = room; // Set the boss room
    }

    public void SetTwinBossReferences(TwinBossesReferences twinBossRefs)
    {
        twinBossReferences = twinBossRefs; // Set the reference to the Twin Bosses References script
    }

    public void OpenPortal()
    {
        if (twinBossReferences != null)
        {
            if (twinBossReferences.OpenPortal()) // Call the OpenPortal method in the Twin Bosses References script
                StopBossMusic(); // Stop the boss music when the portal is opened
        }
        else
        {
            bossRoom.GetComponent<BossRoomManagerScript>().OpenNextLevelPortal(); // Open the next level portal in the boss room
            StopBossMusic(); // Stop the boss music when the portal is opened

        }
    }

    private void StopBossMusic()
    {
        audioManager.PlayBackgroundMusic(); // Stop the boss battle music
    }
    
    private void StartBossMusic()
    {
        switch (bossType)
        {
            case EnumBosses.Widow:
                audioManager.PlayBossBattleMusic(audioManager.widowBossMusicSource); // Play the Widow boss music
                break;
            case EnumBosses.AncientGolem:
                audioManager.PlayBossBattleMusic(audioManager.golemBossMusicSource); // Play the Golem boss music
                break;
            case EnumBosses.LordOfFire:
                audioManager.PlayBossBattleMusic(audioManager.lordsBossMusicSource); // Play the Lords boss music
                break;
            default:
                break;
        }
    }

}
