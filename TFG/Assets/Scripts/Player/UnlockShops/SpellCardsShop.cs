using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCardsShop : MonoBehaviour
{

    [SerializeField] private List<GameObject> spellCardsShopItemsPositions;

    [SerializeField] private GameObject spellCardShopItemPrefab;

    private List<EnumSpellCards> possibleSpellCardsToGenerate = new List<EnumSpellCards>();

        
    [SerializeField] private GameObject currentGemsTextUI; // Prefab for the rune shop item

    [SerializeField] private GameObject soldOutItemPrefab; // Prefab for the sold out item


    // Start is called before the first frame update
    void Start()
    {
        // Adds to the list of possible spell cards to generate
        for (int i = 1; i <= GameData.unlockedSpellCards.Length; i++)
        {
            EnumSpellCards spellCard = (EnumSpellCards)i;
            // Check if the spell card is not unlocked and is not None
            if (spellCard != EnumSpellCards.None && !GameData.unlockedSpellCards[i - 1])
            {
                Debug.Log("Spell Card " + spellCard + " is not unlocked, adding to possibleSpellCardsToGenerate.");
                possibleSpellCardsToGenerate.Add(spellCard);
            }
            else
            {
                Debug.Log("Spell Card " + spellCard + " is already unlocked or is None, skipping.");
            }
        }

        for (int i = 0; i < spellCardsShopItemsPositions.Count; i++)
        {

            // If the spell card shop item is empty, generate a new spell card
            if (possibleSpellCardsToGenerate.Count > 0)
            {
                int randomIndex = Random.Range(0, possibleSpellCardsToGenerate.Count);
                EnumSpellCards randomSpellCard = possibleSpellCardsToGenerate[randomIndex];

                // Instantiate the spell card shop item
                GameObject spellCardShopItem = Instantiate(spellCardShopItemPrefab, spellCardsShopItemsPositions[i].transform.position, Quaternion.identity);
                spellCardShopItem.transform.SetParent(gameObject.transform);
                spellCardShopItem.GetComponent<BuyUnlockeableController>().SetSpellCard(randomSpellCard);
                spellCardShopItem.GetComponent<BuyUnlockeableController>().SetCurrentGemsTextUI(currentGemsTextUI);

                // Remove the generated spell card from the list of possible spell cards to generate
                possibleSpellCardsToGenerate.RemoveAt(randomIndex);
            }
            else
            {
                // Will Generate an object with a "Sold Out" text
                GameObject soldOutItem = new GameObject("Sold Out");
                soldOutItem.transform.SetParent(gameObject.transform);
            }
            
        }
    }
}
