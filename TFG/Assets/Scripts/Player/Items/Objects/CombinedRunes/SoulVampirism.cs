using UnityEngine;

public class SoulVampirism : IItem
{
    [SerializeField]
    private EnumRunes rune = EnumRunes.SoulVampirism_Rune; // The rune type for this item
    private GameObject player;

    public SoulVampirism(GameObject player)
    {
        this.player = player;
    }

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues())
            return;

        player.GetComponent<RunesModifiers>().soulVampirismLifeGain += 5; // Life gain per enemy killed

    }

    public bool CheckIfInitializedValues()
    {

        if (player == null)
        {
            Debug.LogError("⚠️ Soul Vampirism: Player not set. This item cannot be applied without a player.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return $"This rune grants the ability to heal 5 health per enemy killed. ";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>($"Runes/Sprites/Soul Vampirism"); // Load the icon from Resources folder
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