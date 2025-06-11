using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelPortalController : MonoBehaviour
{

    [SerializeField]
    private LayerMask playerLayer; // Layer mask to detect players
    [SerializeField]
    private GameObject nextLevelText; // Text to display when the player is near the portal

    private GameObject MapGenerator; // Reference to the MapGenerator script
    private LevelInformation levelInformation; // Reference to the LevelInformation script
    private GameObject player; // Reference to the player object

    private bool isInTrigger = false; // Flag to check if the player is in the trigger area

    private void Awake()
    {
        MapGenerator = GameObject.Find("MapGeneration");
        levelInformation = FindObjectOfType<LevelInformation>(); // Find the LevelInformation script in the scene
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player object by tag
        if (levelInformation == null)
        {
            Debug.LogError("LevelInformation script not found in the scene. Please ensure it is present.");
            return;
        }
        if (MapGenerator == null)
        {
            Debug.LogError("MapGenerator not found in the scene.");
            return;
        }
        nextLevelText.SetActive(false); // Initially hide the next level text

    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // If the player is in the trigger area and presses E, open the next level portal
            if (MapGenerator != null)
            {
                if (levelInformation.GetLevel() < 3)
                    MapGenerator.GetComponent<RoomsGenerator>().Generate();
                else
                {
                    player.GetComponent<PlayerController_test>().FinishRun();
                }
            }
            else
            {
                Debug.LogError("MapGenerator or EnemiesManager is null. Cannot open next level portal.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if ((playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            // If the player enters the trigger, show the next level text
            nextLevelText.SetActive(true);
            isInTrigger = true;
        }

    }

    void OnTriggerExit(Collider other)
    {

        if ((playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            // If the player exits the trigger, hide the next level text
            nextLevelText.SetActive(false);
            isInTrigger = false;
        }

    }

}
