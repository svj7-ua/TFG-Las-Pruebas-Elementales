using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomManagerScript : MonoBehaviour
{

    [Tooltip("Ignores the Y axis, since that value is unique for each boss it is stored in the prefab")]
    [SerializeField] private Transform bossSpawnPoint;    // Spawn point for the boss
    [SerializeField] private GameObject nextLevelPortal; // Next level portal

    private GameObject bossPrefab; // Prefab of the boss

    // Start is called before the first frame update
    void Awake()
    {

        ChooseBossPrefab(); // Choose the boss prefab
        
    }

    private void ChooseBossPrefab(){
        EnemiesPrefabReferences enemiesPrefabReferences = FindObjectOfType<EnemiesPrefabReferences>();    // Get the enemies prefab references script.
        if (enemiesPrefabReferences == null)
        {
            Debug.LogError("Enemies prefab references script not found in the scene.");
            return;
        }

        // Get the boss prefab from the enemies prefab references script, randomly choosing one from the list strongEnemiesPrefabs.
        bossPrefab = enemiesPrefabReferences.generateStrongEnemy();


        // Todo: upgrade the boss stats depending on the level.
    }

    public void SpawnBoss(){
        if (bossPrefab == null){
            Debug.LogError("Boss prefab is null. Cannot spawn boss.");
            return;
        }

        Vector3 spawnPosition = bossSpawnPoint.position; // Get the spawn position from the boss spawn point
        spawnPosition.y = bossPrefab.transform.position.y; // Set the Y position to the boss prefab's Y position

        // Instantiate the boss prefab at the boss spawn point
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        if (boss.GetComponent<BossReferences>() == null)
        {
            // If the boss is a Twin Boss Fight it adds the waypoints to both children of the boss prefab
            BossReferences[] bossReferences = boss.GetComponentsInChildren<BossReferences>();
            if (bossReferences.Length == 0)
            {
                Debug.LogError("BossReferences component not found in the boss prefab or its children.");
                return; // Return if no BossReferences component is found
            }
            foreach (BossReferences br in bossReferences)
            {
                br.AddWaypoints(GetComponent<BossWaypointsReference>().GetWaypoints()); // Add the waypoints to each boss
            }
        }
        else
        {
            boss.GetComponent<BossReferences>().AddWaypoints(GetComponent<BossWaypointsReference>().GetWaypoints()); // Add the waypoints to the boss
        }
        
        Debug.Log("Boss spawned: " + boss.name);

    }

    public Sprite GetBossSummoningCircleSprite(){
        // Get the boss summoning circle texture from the boss prefab

        BossReferences bossReferences = bossPrefab.GetComponent<BossReferences>();

        if (bossReferences == null)
        {
            bossReferences = bossPrefab.GetComponentInChildren<BossReferences>();
            if (bossReferences == null)
            {
                Debug.LogError("BossReferences component not found in the boss prefab or its children.");
                return null; // Return null if the component is not found
            }
        }

        Sprite sprite = bossReferences.GetBossSummoningCircleSprite();
        if (sprite == null)
        {

            Debug.LogError("Boss summoning circle sprite not found in the boss prefab or its children.");
            return null; // Return null if the sprite is not found

        }
        return sprite;
    }

    public int GetBossSummoningCircleAnimation(){
        
        BossReferences bossReferences = bossPrefab.GetComponent<BossReferences>();

        if (bossReferences == null)
        {
            bossReferences = bossPrefab.GetComponentInChildren<BossReferences>();
            if (bossReferences == null)
            {
                Debug.LogError("BossReferences component not found in the boss prefab or its children.");
                return -1;
            }
        }

        return (int)bossReferences.GetBossType(); // Get the boss type from the boss prefab

    }


}
