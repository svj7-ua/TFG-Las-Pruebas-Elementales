using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomManagerScript : MonoBehaviour
{

    [Tooltip("Ignores the Y axis, since that value is unique for each boss it is stored in the prefab")]
    [SerializeField] private Transform bossSpawnPoint;    // Spawn point for the boss
    [SerializeField] private GameObject nextLevelPortal; // Next level portal
    private LevelInformation levelInformation; // Reference to the LevelInformation script
    private GameObject bossPrefab; // Prefab of the boss

    [SerializeField]
    private GameObject door;

    [Header("Boss Rewards")]
    [SerializeField]
    private GameObject spellCard;

    // Start is called before the first frame update
    void Awake()
    {
        levelInformation = FindObjectOfType<LevelInformation>(); // Get the LevelInformation script from the scene
        ChooseBossPrefab(); // Choose the boss prefab
        spellCard.SetActive(false); // Deactivate the spell card at the start

    }

    private void ChooseBossPrefab()
    {
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

    public void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError("Boss prefab is null. Cannot spawn boss.");
            return;
        }

        Vector3 spawnPosition = bossSpawnPoint.position; // Get the spawn position from the boss spawn point
        spawnPosition.y = bossPrefab.transform.position.y; // Set the Y position to the boss prefab's Y position

        // Set the boss prefab at the boss spawn point
        //boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        bossPrefab.transform.position = spawnPosition; // Set the position of the boss prefab
        if (bossPrefab.GetComponent<BossReferences>() == null)
        {
            // If the boss is a Twin Boss Fight it adds the waypoints to both children of the boss prefab
            BossReferences[] bossReferences = bossPrefab.GetComponentsInChildren<BossReferences>();
            if (bossReferences.Length == 0)
            {
                Debug.LogError("BossReferences component not found in the boss prefab or its children.");
                return; // Return if no BossReferences component is found
            }
            foreach (BossReferences br in bossReferences)
            {
                br.AddWaypoints(GetComponent<BossWaypointsReference>().GetWaypoints()); // Add the waypoints to each boss
                br.SetTwinBossReferences(bossPrefab.GetComponent<TwinBossesReferences>()); // Set the TwinBossesReferences for each boss
                br.IncrementAttacksPerRotation(levelInformation.GetLevel()); // Increment the attacks per rotation based on the level
            }

            bossPrefab.GetComponent<TwinBossesReferences>().SetBossRoom(gameObject); // Set the boss room for the twin boss fight
        }
        else
        {
            bossPrefab.GetComponent<BossReferences>().AddWaypoints(GetComponent<BossWaypointsReference>().GetWaypoints()); // Add the waypoints to the boss
            bossPrefab.GetComponent<BossReferences>().SetBossRoom(gameObject); // Set the boss room for the boss
            bossPrefab.GetComponent<BossReferences>().IncrementAttacksPerRotation(levelInformation.GetLevel()); // Increment the attacks per rotation based on the level
        }

        bossPrefab.SetActive(true); // Activate the boss prefab
        Debug.Log("Boss spawned: " + bossPrefab.name);

    }

    public Sprite GetBossSummoningCircleSprite()
    {
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

    public int GetBossSummoningCircleAnimation()
    {

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

    public void OpenNextLevelPortal()
    {
        if (nextLevelPortal == null)
        {
            Debug.LogError("Next level portal is not assigned in the BossRoomManagerScript.");
            return; // Return if the next level portal is not assigned
        }

        nextLevelPortal.SetActive(true); // Activate the next level portal
        door.GetComponent<DoorController>().ToggleDoor(); // Toggle the door to open it
        bossPrefab.SetActive(false); // Deactivate the boss prefab
        Debug.Log("Next level portal opened.");
        // Generate a spell card as a reward for defeating the boss
        GenerateSpellCard();
        GetComponent<RoomDataScript>().GenerateRune(); // Generates the rune for the room
    }

    private void GenerateSpellCard()
    {
        // Get a random spell card from the EnumSpellCards enum
        System.Array values = System.Enum.GetValues(typeof(EnumSpellCards));
        spellCard.GetComponent<SpellCard>().SetSpellCard((EnumSpellCards)values.GetValue(Random.Range(1, values.Length))); // Value 0 is reserved for "None"

        // Sets its type randomly between Melee, Ranged and Dash
        System.Array types = System.Enum.GetValues(typeof(EnumSpellCardTypes));
        spellCard.GetComponent<SpellCard>().SetSpellCardType((EnumSpellCardTypes)types.GetValue(Random.Range(0, types.Length-1))); // 0 is Melee, 1 is Ranged, 2 is Dash

        spellCard.SetActive(true); // Activate the spell card
    }


}
