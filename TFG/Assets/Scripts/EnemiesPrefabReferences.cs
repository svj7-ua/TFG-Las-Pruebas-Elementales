using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesPrefabReferences : MonoBehaviour{
    
    [SerializeField] private GameObject[] weakEnemiesPrefabs; // Array of enemy prefabs
    [SerializeField] private GameObject[] strongEnemiesPrefabs; // Array of enemy prefabs

    [SerializeField] private GameObject testEnemyPrefab; // Test enemy prefab
    [SerializeField] private GameObject testStrongEnemyPrefab; // Test strong enemy prefab

    [SerializeField] private bool DEBUG_MODE = true;


    // Returns a random game object from the weak enemies prefabs array. If DeuG_MODE is true, it returns the test enemy prefab.
    public GameObject generateWeakEnemy(){

        if (DEBUG_MODE){
            return testEnemyPrefab; // Return the test enemy prefab if DEBUG_MODE is true
        } else {
            int randomIndex = Random.Range(0, weakEnemiesPrefabs.Length); // Get a random index from the array
            return weakEnemiesPrefabs[randomIndex]; // Return the random enemy prefab
        }

    }

    public GameObject generateStrongEnemy(){

        if (DEBUG_MODE){
            return testStrongEnemyPrefab; // Return the test enemy prefab if DEBUG_MODE is true
        } else {
            int randomIndex = Random.Range(0, strongEnemiesPrefabs.Length); // Get a random index from the array
            return strongEnemiesPrefabs[randomIndex]; // Return the random enemy prefab
        }

    }

}
