using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSelector : MonoBehaviour
{

    [SerializeField]
    private GameObject selectorUI;

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private GameObject openSelectorText;
    private bool canOpenSelector = false;
    private bool isSelectorOpen = false;

    [SerializeField]
    private bool isSpellCardSelector = false; // Flag to check if this is a spell card selector

    // Start is called before the first frame update
    void Start()
    {
        openSelectorText.SetActive(false); // Initially hide the text to open the selector
        canOpenSelector = false;
        isSelectorOpen = false;

        Debug.Log(GameData.gems + ", " + GameData.equipedSpellCard + ", " + GameData.equipedSpellCardType + ", " + GameData.equipedRune);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); // Find the player object by tag if not assigned
            if (player == null)
            {
                Debug.LogError("Player object not found! Please assign the player object in the inspector or ensure it has the 'Player' tag.");
            }
        }
        
        if(selectorUI == null)
        {
            Debug.LogError("Selector UI not assigned! Please assign the selector UI in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (canOpenSelector && Input.GetKeyDown(KeyCode.E)) // Check if the player can open the selector and presses the 'E' key
        {
            // if the game is stopped, do not open the selector
            if (Time.timeScale == 0)
            {
                return;
            }

            if (selectorUI == null)
            {
                Debug.LogError("Selector UI not assigned! Please assign the selector UI in the inspector.");
            }
            if (!isSelectorOpen)
            {
                selectorUI.SetActive(true); // Open the selector UI
                selectorUI.GetComponent<SelectorController>().OpenSelector(isSpellCardSelector); // Update the selector UI
                isSelectorOpen = true; // Update the state to open
                player.GetComponent<PlayerController_test>().isSelectorOpen = true;
            }
            else
            {
                selectorUI.SetActive(false); // Close the selector UI
                isSelectorOpen = false; // Update the state to closed
                player.GetComponent<PlayerController_test>().isSelectorOpen = false;
            }
        }

        if (isSelectorOpen && !selectorUI.activeSelf)
        {
            isSelectorOpen = false; // If the selector UI is not active, update the state to closed
            player.GetComponent<PlayerController_test>().isSelectorOpen = false;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            canOpenSelector = true; // If the player enters the trigger, set canOpenSelector to true
            openSelectorText.SetActive(true); // Show the text to open the selector
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            canOpenSelector = false; // If the player exits the trigger, set canOpenSelector to false
            openSelectorText.SetActive(false); // Hide the text to open the selector
            if (isSelectorOpen)
            {
                selectorUI.SetActive(false); // Close the selector UI if it is open
                isSelectorOpen = false; // Update the state to closed
                player.GetComponent<PlayerController_test>().isSelectorOpen = false;
                
            }
        }
    }
}
