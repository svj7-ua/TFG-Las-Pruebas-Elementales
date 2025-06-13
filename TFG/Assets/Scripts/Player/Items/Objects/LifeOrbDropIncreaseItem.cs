using UnityEngine;

public class LifeOrbDropIncreaseItem : MonoBehaviour, IItem
{

    private EnumRunes rune = EnumRunes.LifeOrbDropIncrease_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;
    private EnumItemCategories itemCategory = EnumItemCategories.InfiniteGeneration; // This item can spawn multiple times

    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        player.GetComponent<RunesModifiers>().lifeOrbStrongEnemiesMaxAmount += 2; // Increases the maximum amount of life orbs dropped by strong enemies by 2
        player.GetComponent<RunesModifiers>().lifeOrbWeakEnemiesMaxAmount += 1; // Increases the maximum amount of life orbs dropped by weak enemies by 1
    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set for Life Orb Drop Increase Item.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return "Increases the maximum amount of life orbs dropped by strong enemies by 2 and by 1 for weak enemies.";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>("Runes/Sprites/Life Orb Drop Increase");
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
        return -1; // This item can spawn up to 3 times
    }

    public EnumRunes GetRune()
    {
        return rune;
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

    public bool IsItemCombinable()
    {
        // This item is not combinable
        return false;
    }

    public EnumRunes GetRuneToCombine()
    {
        // This item does not have a rune to combine
        return EnumRunes.None;
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