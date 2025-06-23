using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPortalController : MonoBehaviour
{
[SerializeField]
    private LayerMask playerLayer; // Layer mask to detect players
    [SerializeField]
    private GameObject nextLevelText; // Text to display when the player is near the portal
    private bool isInTrigger = false; // Flag to check if the player is in the trigger area
    [SerializeField]
    private GameObject sceneLoader; // Reference to the player object

    private void Awake()
    {

        nextLevelText.SetActive(false); // Initially hide the next level text

    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // Loads the GameScene
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainLobby")
            {
                sceneLoader.GetComponentInChildren<LoadingScreen>().LoadScene(2); // GameScene index is 2
            }
            else
            {
                Debug.LogError("StartPortalController: Not in MainMenu scene. Cannot load GameScene.");
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
