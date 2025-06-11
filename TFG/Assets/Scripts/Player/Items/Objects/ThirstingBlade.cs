using UnityEngine;

public class ThirstingBlade : MonoBehaviour, IItem
{

    private EnumRunes rune = EnumRunes.ThirstingBlade_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;
    private EnumItemCategories itemCategory = EnumItemCategories.InfiniteGeneration; // This item can spawn multiple times
    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        // Increases the player's attack speed by 20% and critical strike chance by 10%
        // Increases
        player.GetComponent<RunesModifiers>().rawBasicAttackDamageIncrement += 5.0f;

    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set for Thirsting Blade Item.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        throw new System.NotImplementedException();
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>("Runes/Sprites/Thirsting Blade");
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
        return -1;
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
        return false; // This item does not remove all the category from spawning
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

}