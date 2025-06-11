using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomDataScript : MonoBehaviour
{
    private RunesManager runesManager;    // Reference to the runes manager script, will be used to check if the room is irregular.
    public bool isIrregular = false;

    [SerializeField]
    public Vector2Int size;            // The size of the room. X is the width, Y is the height.

    [SerializeField]
    public List<GameObject> roomPrefabs_doors;   // List of door prefabs
    [SerializeField]
    public List<bool> doorPositions;    // List of door positions -> True if there is a door in that position, False otherwise.
                                        // Order: [up, right, down, left]

    public List<bool> connectedDoors;    // List of connected doors -> True if the door is connected to another room, False otherwise.
                                         // Will s
                                         // Order: [up, right, down, left]

    [SerializeField]
    public List<Vector2Int> doorPositionsCoordinates;    // List of door positions coordinates -> [x, y] coordinates of the doors.
                                                         // Order: [up, right, down, left], if there is no door in that position, the value is [-1, -1].

    [SerializeField]
    public List<Vector2Int> irregularValues;    // List of irregular values -> [x, y] coordinates of the irregular values.
                                                // Order: [x, y], if there is no irregular value in that position, the value is [-1, -1].

    [SerializeField]
    public bool isShop;    // True if the room is a shop, False otherwise.
    [SerializeField]
    public bool isBossRoom;    // True if the room is a shop, False otherwise.

    [SerializeField]
    List<GameObject> spawners;    // List of spawners in the room. Will be used to spawn enemies in the room, each spawner will be able to spawn one enemy.

    private List<GameObject> enemiesGenerated = new List<GameObject>();    // List of enemies in the room. (Will be prepared in the Start function, but will instantiate once the player enters the room)
                                                                           // DURING TEST THEY WILL SPAWN IMMEDIATELY.

    private bool enemiesSpawned = false;    // True if the enemies have been spawned, False otherwise.

    [SerializeField] private bool isOpen = true;
    //[SerializeField] private bool isClosed = false;

    [SerializeField] private List<GameObject> doorTriggers = new List<GameObject>();

    [Space]
    [Header("Reward Generation Variables")]
    [SerializeField] private List<GameObject> runeLocation;
    [SerializeField] private float addedChancePerDoor = 0.1f;    // Chance of generating a rune in the room, will be multiplied by the number of connected doors.
    [SerializeField] private float addedChancePerSpawner = 0.05f;    // Chance of generating a rune in the room, will be multiplied by the number of spawners.
    [SerializeField] private float substractedChancePerRuneSpawner = 0.025f;
    private EnemiesPrefabReferences enemiesPrefabReferences;    // Reference to the enemies prefab references script.

    public void Start()
    {

        enemiesPrefabReferences = FindObjectOfType<EnemiesPrefabReferences>();    // Get the enemies prefab references script.
        runesManager = FindObjectOfType<RunesManager>();    // Get the runes manager script.
        if (runesManager == null)
        {
            Debug.LogError("Runes manager script not found in the scene.");
        }

        if (enemiesPrefabReferences == null)
        {
            Debug.LogError("Enemies prefab references script not found in the scene.");
        }
        // Initialize the connectedDoors list as False, will be updated when the room is connected to another room.
        // Order: [up, right, down, left] or [N, E, S, W]

        for (int i = 0; i < 4; i++)
        {
            connectedDoors.Add(false);
        }

        ActivateTriggers();


    }

    public EnemiesPrefabReferences GetEnemiesPrefabReferences()
    {
        return enemiesPrefabReferences;
    }

    public void Update()
    {
        if (enemiesSpawned)
        {
            isRoomCleared();
        }
    }

    private void ActivateTriggers()
    {


        if (doorTriggers != null && doorTriggers.Count > 0) // Check if the list is not null and has elements (in which case it means that there are triggers to activate)
        {
            foreach (var triggerObject in doorTriggers)
            {
                DoorTriggerNotifier trigger = triggerObject.GetComponent<DoorTriggerNotifier>();

                if (trigger != null)
                {
                    trigger.OnTriggerActivated += OnDoorTriggerEnter; // Subscribe to the event
                }
                else
                {
                    Debug.LogError($"⚠️ El objeto {triggerObject.name} no tiene un DoorTriggerNotifier.");
                }
            }

            Debug.Log($"✅ {doorTriggers.Count} Triggers activados.");
        }
        else
        {
            Debug.Log("No se encontraron Triggers en la lista.");
        }


    }

    private void OnDoorTriggerEnter(Collider other)
    {
        Debug.Log("Trigger activated");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the room");
            if (isOpen)
            {

                isOpen = false;
                //isClosed = true;

                // Closes the doors.
                foreach (var door in roomPrefabs_doors)
                {
                    if (door != null)
                    {
                        door.GetComponent<DoorController>().ToggleDoor();    // Toggle the door

                    }
                }

                // After closing the door, it will be locked ultil player defeats enemies in the room, so we need to disable the triggers
                foreach (GameObject trigger in doorTriggers)
                {
                    trigger.SetActive(false);
                }

                // TODO: SPAWN ENEMIES
                Debug.Log("Spawning enemies");
                generateEnemies();
                enemiesSpawned = true;
            }

        }
    }

    private void generateEnemies()
    {

        for (int i = 0; i < spawners.Count; i++)
        {
            GameObject enemyToSpawn = enemiesPrefabReferences.generateWeakEnemy();    // TODO: Remove after testing (Enemies instantiate when entering the room).         

            GameObject enemy = Instantiate(enemyToSpawn, spawners[i].transform.position, Quaternion.identity);    // TODO: Remove after testing (Enemies instantiate when entering the room).

            Debug.Log("Enemy " + (i + 1) + " is " + enemy.name + " instantiated at: " + spawners[i].transform.position);

            enemiesGenerated.Add(enemy);

            //Instantiate(enemy, spawners[i].transform.position, Quaternion.identity);    // TODO: Remove after testing (Enemies instantiate when entering the room).
            Debug.Log("Enemy " + (i + 1) + " instantiated at: " + spawners[i].transform.position);

            //enemy = chooseEnemie(i);    
            //enemiesGenerated.Add(testEnemie);
        }
    }

    public void isRoomCleared()
    {

        // FOR TESTING PURPOSES
        /*         if(triggerDoors==true){
                    return;
                } */

        // Checks the HP of all enemies in the room, if all enemies are dead, the room is cleared.
        foreach (GameObject enemy in enemiesGenerated)
        {
            //TODO: "PlayerHealth" will be replaced by the enemy health script.
            if (enemy.GetComponent<Health>().currentHealth > 0)
            {
                return;
            }
        }

        GenerateRune();    // Generate a rune if the chance is met.

        // If all enemies are dead, the room is cleared. The doors will open.
        Debug.Log("All enemies defeated, opening doors");
        isOpen = true;
        //isClosed = false;
        enemiesSpawned = false;

        // Opens the doors.
        foreach (var door in roomPrefabs_doors)
        {
            if (door != null)
            {
                Debug.Log("Opening door, all enemies defeated");
                //TODO: Remove coment after testing
                door.GetComponent<DoorController>().ToggleDoor();    // Toggle the door
            }
        }

        // Destroy the enemies in the room.
        foreach (GameObject enemy in enemiesGenerated)
        {
            Destroy(enemy);
        }
        enemiesGenerated.Clear();    // Clear the list of enemies in the room.

    }

    private void GenerateRune()
    {

        float runeChance = 0.0f;
        foreach (bool door in connectedDoors)
        {
            if (door)    // If the door is connected to another room, increase the chance of generating a rune.
            {
                runeChance += addedChancePerDoor;    // Increase the chance of generating a rune.
            }
        }

        runeChance += addedChancePerSpawner * spawners.Count;    // Increase the chance of generating a rune based on the number of spawners in the room.
        runeChance -= substractedChancePerRuneSpawner * runeLocation.Count;    // Decrease the chance of generating a rune based on the number of rune locations in the room.
        Debug.Log("Chance of generating a rune: " + runeChance);

        if (isBossRoom || runesManager.debugMode)
        {
            runeChance = 1.0f;    // If the room is a boss room, the chance of generating a rune is 100%.
        }

        foreach (GameObject runeLoc in runeLocation)
        {
            if (runeLoc != null && UnityEngine.Random.value <= runeChance)
            {
                GameObject rune = runesManager.GenerateRune();    // Generate a rune using the runes manager
                GameObject r = Instantiate(rune, runeLoc.transform.position, Quaternion.identity);    // Instantiate the rune at the reward location
                r.transform.SetParent(gameObject.transform);    // Set the parent of the rune to the root object
                Debug.Log("Reward generated in the room: " + rune.name);
            }

        }
    }


}
