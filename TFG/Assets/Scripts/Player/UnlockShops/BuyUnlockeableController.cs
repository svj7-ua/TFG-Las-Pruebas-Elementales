using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyUnlockeableController : MonoBehaviour
{

    private int SpellCardSpawn = -1;
    [SerializeField] private int price = 0; // The price of the unlockeable item
    [SerializeField] private GameObject priceTag; // The item to unlock
    [SerializeField] private GameObject gemIcon; // The item to unlock when the player buys it
    [SerializeField] private GameObject pickUpText; // The item to unlock when the player buys it
    [SerializeField] private LayerMask playerLayer; // The layer of the player

    private RunesShop runesShop; // Reference to the RunesShop script

    [SerializeField] private bool isSpellcard = false; // The item to unlock when the player buys it
    private EnumSpellCards spellCard; // The type of spell card to unlock
    private bool isPlayerAbleToBuy = false; // If the player is able to buy the item

    private GameObject currentGemsTextUI;

    [Header("Set Up Only for Runes")]
    [SerializeField]
    private EnumRunes rune = EnumRunes.None; // The type of rune to unlock

    private void Start()
    {

        pickUpText.SetActive(false); // Deactivate the price tag at the start

        if (isSpellcard)
        {
            price = 100;
        }
        else
        {
            price = 50; // Set the price of the rune to 50
        }

    }

    public void SetRunesShop(RunesShop runesShop)
    {
        this.runesShop = runesShop; // Set the reference to the RunesShop script
    }

    public void SetCurrentGemsTextUI(GameObject currentGemsTextUI)
    {
        this.currentGemsTextUI = currentGemsTextUI; // Set the current gems text UI
    }

    public void SetSpellCardSpawn(int spellCardSpawn)
    {
        SpellCardSpawn = spellCardSpawn; // Set the spell card spawn index
    }
    public int GetSpellCardSpawn()
    {
        return SpellCardSpawn; // Return the spell card spawn index
    }

    public void SetRune(EnumRunes rune)
    {
        this.rune = rune; // Set the rune to the correct one
        Sprite sprite = Resources.Load<Sprite>("Runes/Sprites/" + rune.ToString()); // Load the sprite for the rune
        if (sprite == null)
        {
            Debug.LogError("Rune sprite not found for: " + rune.ToString());
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprite; // Set the sprite of the item to the correct one
        }

    }

    public void SetSpellCard(EnumSpellCards spellCard)
    {
        this.spellCard = spellCard; // Set the spell card to the correct one
        Sprite sprite = SpellCard_Factory.LoadSpellCardIcon(spellCard, EnumSpellCardTypes.Melee); // Load the sprite for the spell card
        GetComponent<SpriteRenderer>().sprite = sprite; // Set the sprite of the item to the correct one
    }

    void Update()
    {

        // Check if the player is able to buy the item When the player presses the "E" key
        if (isPlayerAbleToBuy && Input.GetKeyDown(KeyCode.E))
        {

            if (GameData.gems >= price) // Check if the player has enough gems to buy the item
            {
                GameData.gems -= price; // Deduct the price from the player's gems
                currentGemsTextUI.GetComponent<TextMeshProUGUI>().text = ": " + GameData.gems.ToString(); // Update the gems text UI
                priceTag.SetActive(false); // Deactivate the price tag
                pickUpText.SetActive(false); // Deactivate the pick up text
                UnlockItem(); // Call the method to unlock the item

                Destroy(gameObject); // Destroy the item after it has been bought
            }
            else
            {
                Debug.Log("Not enough gems to buy this item!"); // Log a message if the player doesn't have enough gems
            }

        }

    }

    private void UnlockItem()
    {

        if (isSpellcard)
        {
            // Unlock the spell card
            GameData.unlockedSpellCards[(int)spellCard - 1] = true; // Assuming price corresponds to the index of the spell card
            Debug.Log("Spell Card Unlocked: " + spellCard.ToString());
        }
        else
        {
            // Unlock the rune
            GameData.unlockedRunes[(int)rune - 1] = true; // Assuming price corresponds to the index of the rune
            runesShop.AddUnlockedRune(rune); // Add the rune to the shop
            Debug.Log("Rune Unlocked: " + rune.ToString());
        }

    }

    void OnTriggerEnter(Collider other)
    {

        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {

            isPlayerAbleToBuy = true; // Set the flag to true when the player enters the trigger
            pickUpText.SetActive(true); // Activate the price tag when the player is in range of the item

        }

    }
    
    void OnTriggerExit(Collider other)
    {

        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {

            isPlayerAbleToBuy = false; // Set the flag to false when the player exits the trigger
            pickUpText.SetActive(false); // Deactivate the price tag when the player is out of range of the item

        }

    }

}