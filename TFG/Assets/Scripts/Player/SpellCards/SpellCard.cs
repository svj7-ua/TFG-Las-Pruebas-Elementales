using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : MonoBehaviour
{

    [SerializeField] LayerMask playerLayerMask; // Player LayerMask
    [SerializeField] EnumSpellCards spellCard; // Type of spell card
    [SerializeField] GameObject player; // Reference to the player
    [SerializeField] GameObject pickUpTextPopUp; // Reference to the pick up text pop up prefab so we can activate it when the player is in range of the item
    [SerializeField] GameObject spellCardPrefab; // Reference to the spellcard so it can be changed to the correct one once it is created
    private GameObject shop;
    private bool isInShop = false; // Flag to check if the player is in the shop
    [SerializeField]
    EnumSpellCardTypes spellCardType; // Type of spell card for the player to use
    private bool isInRange = false; // Flag to check if the player is in range of the item

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player in the scene
        if (player == null) // Check if the player is not found
        {
            Debug.LogError("Player not found in the scene. Please make sure the player has the 'Player' tag.");
        }
        pickUpTextPopUp.SetActive(false); // Deactivate the pick up text pop up at the start
        spellCardPrefab.GetComponent<SpriteRenderer>().sprite = SpellCard_Factory.LoadSpellCardIcon(spellCard, spellCardType); // Set the sprite of the spell card to the correct one

    }

    public void SetSpellCard(EnumSpellCards spellCard)
    {
        this.spellCard = spellCard; // Set the spell card to the correct one
    }

    public EnumSpellCards GetSpellCard()
    {
        return spellCard; // Return the spell card type
    }
    public void SetSpellCardType(EnumSpellCardTypes spellCardType)
    {
        this.spellCardType = spellCardType; // Set the spell card type to the correct one
    }

    public void SetItemInShop(GameObject shop)
    {
        this.shop = shop; // Set the shop reference
        isInShop = true; // Set the flag to true when the player is in the shop
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
                player.GetComponent<PlayerController_test>().AddEffectToInventory(SpellCard_Factory.CreateSpellCard(spellCard, spellCardType), spellCardType); // Add the spell card to the player's inventory
                if(isInShop){
                    shop.GetComponent<ShopController>().RemoveAllItemsFromShop(); // Remove all items from the shop if the player is in the shop
                }
                Destroy(gameObject); // Destroy the item after picking it up
            }
        }
    }

}
