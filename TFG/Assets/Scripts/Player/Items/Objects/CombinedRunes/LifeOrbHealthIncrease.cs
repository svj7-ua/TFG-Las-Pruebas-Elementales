using UnityEngine;

public class LifeOrbHealthIncrease : IItem
{
    [SerializeField]
    private EnumRunes rune = EnumRunes.LifeOrbHealthIncrease_Rune; // The rune type for this item
    private GameObject player;

    public LifeOrbHealthIncrease(GameObject player)
    {
        this.player = player;
    }


    public void ApplyItem()
    {
        if (!CheckIfInitializedValues())
            return;
        player.GetComponent<RunesModifiers>().lifeOrbHealthIncrease += 5; // Increase health gained from life orbs by 5
    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("⚠️ ElementalMasterAdepts: Player not set. This item cannot be applied without a player.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return $"This rune auments the player's maximum health by 1 each time a life orb is picked up.";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>($"Runes/Sprites/Life Orb Health Increase"); // Load the icon from Resources folder
    }

    public EnumItemCategories GetItemCategory()
    {
        return EnumItemCategories.Singleton; // Category of the item
    }

    public EnumItemType GetItemType()
    {
        return EnumItemType.Miscelaneous; // Assuming this is the correct item type for this rune
    }

    public int GetNumberOfSpawns()
    {
        return 0;
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
        return false;
    }

    public void RemoveItemEffect()
    {
        // Should not be needed
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
        return null;
    }

    public bool IsFusionRune()
    {
        return true; // This item is not a fusion rune
    }
}