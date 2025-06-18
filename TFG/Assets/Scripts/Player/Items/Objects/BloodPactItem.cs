using System;
using UnityEngine;

public class BloodPactItem : MonoBehaviour, IItem
{

    [SerializeField]
    private EnumRunes rune = EnumRunes.BloodPact_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;

    EnumItemCategories itemCategory = EnumItemCategories.Singleton; // This item can spawn multiple times

    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        // Reduces the player's max health by 33% and increases damage dealt by 50%
        float maxHealth = player.GetComponent<Health>().maxHealth;
        maxHealth *= 0.67f; // Reduce max health by 33%
                            // Rouds the value to the nearest integer
        player.GetComponent<Health>().UpdateMaxHealth(Mathf.RoundToInt(maxHealth));

        player.GetComponent<RunesModifiers>().damageMultiplier += 0.5f; // Increase damage dealt by 50%


    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set for Blood Pact Item.");
            return false;
        }

        return true;
    }

    public string getDescription()
    {
        return "Reduces your maximum health by 33% but increases your damage dealt by 50%.";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>("Runes/Sprites/" + rune.ToString());
        
    }

    public EnumItemCategories GetItemCategory()
    {
        return itemCategory;
    }

    public EnumItemType GetItemType()
    {
        return itemType;
    }

    public int GetNumberOfSpawns()
    {
        return 1; // this item does not have a limit on spawns
    }

    public EnumRunes GetRune()
    {
        return rune;
    }

    public EnumRunes GetRuneToCombine()
    {
        return EnumRunes.None; // This item is not combinable
    }

    public bool IsItemCombinable()
    {
        return false; // This item is not combinable
    }

    public void RemoveItemEffect()
    {
        throw new System.NotImplementedException();
    }

    public bool RemovesAllTheCategoryFromSpawning()
    {
        return false;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public IItem GetCombinedRune()
    {
        return null; // This item is not combinable, so it does not have a combined rune
    }

    public bool IsFusionRune()
    {
        return false; // This item is not a fusion rune
    }

}