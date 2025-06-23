using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask; // Player LayerMask
    [SerializeField] GameObject pickUpTextPopUp; // Reference to the pick up text pop up prefab so we can activate it when the player is in range of the item
    private bool isInRange = false; // Flag to check if the player is in range of the item

    private GameObject player; // Reference to the player

    private bool isPickingUp = false; // Flag to check if the player is picking up the item

    private RunesManager runesManager; // Reference to the RunesManager to manage the runes

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player in the scene
        if (player == null) // Check if the player is not found
        {
            Debug.LogError("Player not found in the scene. Please make sure the player has the 'Player' tag.");
        }

        runesManager = FindObjectOfType<RunesManager>(); // Get the RunesManager script from the scene
        if (runesManager == null) // Check if the RunesManager is not found
        {
            Debug.LogError("RunesManager script not found in the scene. Please make sure it is present.");
        }
    }

    void Start()
    {

        pickUpTextPopUp.SetActive(false); // Deactivate the pick up text pop up at the start

    }

    void OnTriggerEnter(Collider other)
    {
        if(playerLayerMask == (playerLayerMask | (1 << other.gameObject.layer))){
            isInRange = true; // Set the flag to true when the player enters the trigger
            pickUpTextPopUp.SetActive(true); // Activate the pick up text pop up when the player is in range of the item
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(playerLayerMask == (playerLayerMask | (1 << other.gameObject.layer))){
            isInRange = false; // Set the flag to false when the player exits the trigger
            pickUpTextPopUp.SetActive(false); // Deactivate the pick up text pop up when the player is out of range of the item
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(playerLayerMask == (playerLayerMask | (1 << other.gameObject.layer))){
            if(Input.GetKeyDown(KeyCode.E) && isInRange){ // Check if the player is pressing the E key and is in range of the item
                if (isPickingUp) return; // Prevent multiple pickups at the same time
                isPickingUp = true; // Set the flag to true to prevent multiple pickups
                player.GetComponent<InventoryController>().AddItem(gameObject); // Add the item to the player's inventory

                Debug.Log($"Item {GetComponent<IItem>().GetType().Name} picked up by player: {player.name}"); // Log the item pick up
                runesManager.RemoveRuneFromScene(gameObject); // Remove the item from the RunesManager's list of runes in the scene
                Destroy(gameObject); // Destroy the item after picking it up
            }
        }
    }

}