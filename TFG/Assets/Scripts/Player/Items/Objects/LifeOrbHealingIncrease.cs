using UnityEngine;

public class LifeOrbHealingIncrease : MonoBehaviour, IItem
{

    private EnumRunes rune = EnumRunes.LifeOrbHealingIncrease_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;
    private EnumItemCategories itemCategory = EnumItemCategories.InfiniteGeneration; // This item can spawn multiple times

    private GameObject player;
    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        player.GetComponent<RunesModifiers>().baseHealing += 5;
        player.GetComponent<RunesModifiers>().randomHealingRange += 5;
    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set for Life Orb Healing Increase Item.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return "Increases the minimum and maximum healing received from life orbs by 5.";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>("Runes/Sprites/Life Orb Healing Increase");
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
        return -1; // This item can spawn multiple times
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
        return false; // This item does not remove all items of its category from spawning
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public bool IsItemCombinable()
    {
        // This item is not combinable
        return true;
    }

    public EnumRunes GetRuneToCombine()
    {
        // This item does not have a rune to combine
        return EnumRunes.LifeIncrease_Rune;
    }
    public IItem GetCombinedRune()
    {
        return new LifeOrbHealthIncrease(player); // This item is not combinable, so it does not have a combined rune
    }

    public bool IsFusionRune()
    {
        return false; // This item is not a fusion rune
    }
}