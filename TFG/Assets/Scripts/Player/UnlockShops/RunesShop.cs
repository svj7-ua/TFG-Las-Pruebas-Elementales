using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesShop : MonoBehaviour
{
    [SerializeField] private List<GameObject> runesShopItemsPositions;

    [SerializeField] private GameObject runeShopItemPrefab;
    [SerializeField] private List<GameObject> unlockeableRunes_OriginalList;

    [SerializeField] private List<GameObject> unlockeableRunes_ShopList;
    
    [SerializeField] private GameObject currentGemsTextUI; // Prefab for the rune shop item

    [SerializeField] private GameObject soldOutItemPrefab; // Prefab for the sold out item

    private void Awake()
    {

        foreach (GameObject rune in unlockeableRunes_OriginalList)
        {
            if (!GameData.unlockedRunes[(int)rune.GetComponent<IItem>().GetRune()-1])
            {
                unlockeableRunes_ShopList.Add(rune);
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < runesShopItemsPositions.Count; i++)
        {

            // If the spell card shop item is empty, generate a new spell card
            if (unlockeableRunes_ShopList.Count > 0)
            {
                int randomIndex = Random.Range(0, unlockeableRunes_ShopList.Count);
                GameObject runeToGenerate = unlockeableRunes_ShopList[randomIndex];
                // Instantiate the rune shop item
                GameObject runeShopItem = Instantiate(runeShopItemPrefab, runesShopItemsPositions[i].transform.position, Quaternion.identity);
                runeShopItem.GetComponent<BuyUnlockeableController>().SetRune(runeToGenerate.GetComponent<IItem>().GetRune());
                Debug.Log("Rune to generate: " + runeToGenerate.GetComponent<IItem>().GetRune());
                Debug.Log("Index: " + randomIndex);
                runeShopItem.GetComponent<BuyUnlockeableController>().SetCurrentGemsTextUI(currentGemsTextUI);
                runeShopItem.transform.SetParent(gameObject.transform);
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
