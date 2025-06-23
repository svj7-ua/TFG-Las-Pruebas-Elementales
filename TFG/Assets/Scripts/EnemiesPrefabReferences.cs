using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesPrefabReferences : MonoBehaviour{
    
    [SerializeField] private GameObject[] weakEnemiesPrefabs; // Array of enemy prefabs
    [SerializeField] private GameObject[] strongEnemiesPrefabs; // Array of enemy prefabs

    [SerializeField] private GameObject testEnemyPrefab; // Test enemy prefab
    [SerializeField] private GameObject testStrongEnemyPrefab; // Test strong enemy prefab

    [SerializeField] private bool DEBUG_MODE = true;

    private LevelInformation levelInformation; // Reference to the LevelInformation script

    void Awake()
    {
        BogoSortStrongEnemiesList(); // Randomly sort the strong enemies list at the start
        levelInformation = FindObjectOfType<LevelInformation>(); // Find the LevelInformation script in the scene
        if (levelInformation == null)
        {
            Debug.LogError("LevelInformation script not found in the scene. Please ensure it is present.");
        } 
    }
    // Will randomly sort the strong enemies, so they appear in a random order.
    // This is not the BogoSort algorithm, but a simple random shuffle. But the name is kept for fun.
    private void BogoSortStrongEnemiesList()
    {

        for (int i = 0; i < strongEnemiesPrefabs.Length; i++)
        {
            int randomIndex = Random.Range(0, strongEnemiesPrefabs.Length); // Get a random index from the array
            GameObject temp = strongEnemiesPrefabs[i]; // Store the current enemy prefab in a temporary variable
            strongEnemiesPrefabs[i] = strongEnemiesPrefabs[randomIndex]; // Swap the current enemy prefab with the random one
            strongEnemiesPrefabs[randomIndex] = temp; // Set the random enemy prefab to the current one
        }

    }

    // Returns a random game object from the weak enemies prefabs array. If DeuG_MODE is true, it returns the test enemy prefab.
    public GameObject generateWeakEnemy()
    {

        if (DEBUG_MODE)
        {
            return testEnemyPrefab; // Return the test enemy prefab if DEBUG_MODE is true
        }
        else
        {
            int randomIndex = Random.Range(0, weakEnemiesPrefabs.Length); // Get a random index from the array
            return weakEnemiesPrefabs[randomIndex]; // Return the random enemy prefab
        }

    }

    public GameObject generateStrongEnemy(){

        if (DEBUG_MODE){
            return testStrongEnemyPrefab; // Return the test enemy prefab if DEBUG_MODE is true
        } else {
            return strongEnemiesPrefabs[(levelInformation.GetLevel()-1) % strongEnemiesPrefabs.Length]; // Return a strong enemy prefab based on the current level
        }

    }

}
