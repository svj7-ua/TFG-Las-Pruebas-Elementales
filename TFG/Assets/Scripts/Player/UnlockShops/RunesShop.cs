using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesShop : MonoBehaviour
{
    [SerializeField] private List<GameObject> runesShopItemsPositions;

    [SerializeField] private GameObject runeShopItemPrefab;
    [SerializeField] private List<GameObject> unlockeableRunes_OriginalList;

    [SerializeField] private List<GameObject> unlockeableRunes_ShopList;

    public List<GameObject> unlockedRunes;

    [SerializeField] private GameObject currentGemsTextUI; // Prefab for the rune shop item

    [SerializeField] private GameObject soldOutItemPrefab; // Prefab for the sold out item

    private void Awake()
    {

        foreach (GameObject rune in unlockeableRunes_OriginalList)
        {
            if (!GameData.unlockedRunes[(int)rune.GetComponent<IItem>().GetRune() - 1])
            {
                unlockeableRunes_ShopList.Add(rune);
            }
            else
            {

                // Adds to the unlocked runes list in order of EnumRunes
                unlockedRunes.Add(rune);

                unlockedRunes.Sort((a, b) => a.GetComponent<IItem>().GetRune().CompareTo(b.GetComponent<IItem>().GetRune()));

            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < runesShopItemsPositions.Count; i++)
        {


            if (unlockeableRunes_ShopList.Count > 0)
            {
                int randomIndex = Random.Range(0, unlockeableRunes_ShopList.Count);
                GameObject runeToGenerate = unlockeableRunes_ShopList[randomIndex];
                // Instantiate the rune shop item
                GameObject runeShopItem = Instantiate(runeShopItemPrefab, runesShopItemsPositions[i].transform.position, Quaternion.identity);
                runeShopItem.GetComponent<BuyUnlockeableController>().SetRunesShop(this);
                runeShopItem.GetComponent<BuyUnlockeableController>().SetRune(runeToGenerate.GetComponent<IItem>().GetRune());
                Debug.Log("Rune to generate: " + runeToGenerate.GetComponent<IItem>().GetRune());
                Debug.Log("Index: " + randomIndex);
                runeShopItem.GetComponent<BuyUnlockeableController>().SetCurrentGemsTextUI(currentGemsTextUI);
                runeShopItem.transform.SetParent(gameObject.transform);

                // Removes the rune from the shop list to avoid duplicates
                unlockeableRunes_ShopList.RemoveAt(randomIndex);
            }
            else
            {
                // Will Generate an object with a "Sold Out" text
                GameObject soldOutItem = new GameObject("Sold Out");
                soldOutItem.transform.SetParent(gameObject.transform);
            }

        }
    }

    public IItem GetRuneItemByRune(EnumRunes rune)
    {
        foreach (GameObject runePrefab in unlockeableRunes_OriginalList)
        {
            if (runePrefab.GetComponent<IItem>().GetRune() == rune)
            {
                return runePrefab.GetComponent<IItem>();
            }
        }
        Debug.LogError("Rune not found: " + rune);
        return null;
    }

    public void AddUnlockedRune(EnumRunes rune)
    {
        GameObject runeObject = null;

        // Find the rune in the original list
        foreach (GameObject runePrefab in unlockeableRunes_OriginalList)
        {
            if (runePrefab.GetComponent<IItem>().GetRune() == rune)
            {
                runeObject = runePrefab;
                break;
            }
        }

        // Adds the rune to the unlocked runes list
        if (runeObject != null)
        {
            unlockedRunes.Add(runeObject);
            unlockedRunes.Sort((a, b) => a.GetComponent<IItem>().GetRune().CompareTo(b.GetComponent<IItem>().GetRune()));
            GameData.unlockedRunes[(int)rune - 1] = true; // Mark the rune as unlocked in GameData
            Debug.Log("Rune unlocked: " + rune);
        }
        else
        {
            Debug.LogError("Rune not found in the original list: " + rune);
        }
    }
}
