using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataScript : MonoBehaviour
{
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
    List<GameObject> spawners;    // List of spawners in the room. Will be used to spawn enemies in the room, each spawner will be able to spawn one enemy.

    private List<GameObject> enemiesGenerated = new List<GameObject>();    // List of enemies in the room. (Will be prepared in the Start function, but will instantiate once the player enters the room)
                                        // DURING TEST THEY WILL SPAWN IMMEDIATELY.

    private bool enemiesSpawned = false;    // True if the enemies have been spawned, False otherwise.

    [SerializeField] Animator doorAnimator = null;

    [SerializeField] private bool isOpen = true;
    [SerializeField] private bool isClosed = false;

    [SerializeField] private List<GameObject> doorTriggers = new List<GameObject>();

    [SerializeField] private bool triggerDoors = false;

    private EnemiesPrefabReferences enemiesPrefabReferences;    // Reference to the enemies prefab references script.

    public void Start()
    {

        enemiesPrefabReferences = FindObjectOfType<EnemiesPrefabReferences>();    // Get the enemies prefab references script.

        if (enemiesPrefabReferences == null)
        {
            Debug.LogError("Enemies prefab references script not found in the scene.");
        }
        // Initialize the connectedDoors list as False, will be updated when the room is connected to another room.
        // Order: [up, right, down, left] or [N, E, S, W]

        for(int i=0; i<4; i++)
        {
            connectedDoors.Add(false);
        }

        // Sets which enemies will be spawned in the room.

        //generateEnemies();

        ActivateTriggers();

        //enemiesSpawned = true;    // TODO: Remove after testing (Enemies instantiate when entering the room).

    }

    public void Update()
    {
        if(enemiesSpawned)
        {
            isRoomCleared();
        }
    }

    private void ActivateTriggers(){
        

        if (doorTriggers != null && doorTriggers.Count > 0) // Check if the list is not null and has elements (in which case it means that there are triggers to activate)
        {
            foreach (var triggerObject in doorTriggers)
            {
                DoorTriggerNotifier trigger = triggerObject.GetComponent<DoorTriggerNotifier>();

                if (trigger != null)
                {
                    trigger.OnTriggerActivated += OnTriggerEnter; // Subscribe to the event
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger activated");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the room");
            if(isOpen){
                
                isOpen = false;
                isClosed = true;

                // Closes the doors.
                foreach (var door in roomPrefabs_doors)
                {
                    if(door != null)
                    {
                        door.GetComponent<DoorController>().ToggleDoor();    // Toggle the door
                        
                    }
                }

                // After closing the door, it will be locked ultil player defeats enemies in the room, so we need to disable the triggers
                foreach(GameObject trigger in doorTriggers){
                    trigger.SetActive(false);
                }

                // TODO: SPAWN ENEMIES
                Debug.Log("Spawning enemies");
                generateEnemies();
                enemiesSpawned = true;
            }

        }
    }

    private void spawnEnemies(){
    
        for(int i=0; i<spawners.Count; i++)
        {
            //GameObject enemy = Instantiate(testEnemie, spawners[i].transform.position, Quaternion.identity);    // TODO: Remove after testing (Enemies instantiate when entering the room).


            //Instantiate(enemy, spawners[i].transform.position, Quaternion.identity);    // TODO: Remove after testing (Enemies instantiate when entering the room).
            Debug.Log("Enemy " + (i+1) + " instantiated at: " + spawners[i].transform.position);
            
            //enemy = chooseEnemie(i);    
            //enemiesGenerated.Add(testEnemie);
        }
        
    }

    private void generateEnemies(){
        
        for(int i=0; i<spawners.Count; i++)
        {   
            GameObject enemyToSpawn = enemiesPrefabReferences.generateWeakEnemy();    // TODO: Remove after testing (Enemies instantiate when entering the room).         

            GameObject enemy = Instantiate(enemyToSpawn, spawners[i].transform.position, Quaternion.identity);    // TODO: Remove after testing (Enemies instantiate when entering the room).
            
            Debug.Log("Enemy " + (i+1) + " is " + enemy.name + " instantiated at: " + spawners[i].transform.position);

            enemiesGenerated.Add(enemy);

            //Instantiate(enemy, spawners[i].transform.position, Quaternion.identity);    // TODO: Remove after testing (Enemies instantiate when entering the room).
            Debug.Log("Enemy " + (i+1) + " instantiated at: " + spawners[i].transform.position);
            
            //enemy = chooseEnemie(i);    
            //enemiesGenerated.Add(testEnemie);
        }
    }

    public void isRoomCleared(){

        // FOR TESTING PURPOSES
/*         if(triggerDoors==true){
            return;
        } */

        // Checks the HP of all enemies in the room, if all enemies are dead, the room is cleared.
        foreach(GameObject enemy in enemiesGenerated)
        {
            //TODO: "PlayerHealth" will be replaced by the enemy health script.
            if(enemy.GetComponent<Health>().currentHealth > 0)
            {
                return;
            }
        }

        // If all enemies are dead, the room is cleared. The doors will open.
        Debug.Log("All enemies defeated, opening doors");
        isOpen = true;
        isClosed = false;
        enemiesSpawned = false;

        // Opens the doors.
        foreach (var door in roomPrefabs_doors)
        {
            if(door != null)
            {
                Debug.Log("Opening door, all enemies defeated");
                //TODO: Remove coment after testing
                door.GetComponent<DoorController>().ToggleDoor();    // Toggle the door
            }
        }

        // Destroy the enemies in the room.
        foreach(GameObject enemy in enemiesGenerated)
        {
            Destroy(enemy);
        }
        enemiesGenerated.Clear();    // Clear the list of enemies in the room.

    }

    // Function to choose which enemy will be spawned in the room.
    private GameObject chooseEnemie(int i){

        return null;

    }

}
