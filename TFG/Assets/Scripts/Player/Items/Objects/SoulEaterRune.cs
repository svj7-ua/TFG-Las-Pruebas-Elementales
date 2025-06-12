using UnityEngine;

public class SoulEaterRune : MonoBehaviour, IItem
{

    
    private EnumRunes rune = EnumRunes.SoulEater_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;

    EnumItemCategories itemCategory = EnumItemCategories.Singleton; // This item can spawn multiple times

    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        player.GetComponent<RunesModifiers>().soulEaterHealthGrowth += 1f; // Increase health growth by 1 when an enemy dies

        // Reduces the player's maximum health by 10%
        player.GetComponent<Health>().UpdateMaxHealth(player.GetComponent<Health>().maxHealth * 0.9f);

    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set for Soul Eater.");
            return false;
        }

        return true;
    }

    public string getDescription()
    {
        return "Reduces your maximum health by 10% but increases your max health by 1 when an enemy dies.";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>("Runes/Sprites/Soul Eater");
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
        return EnumRunes.Vampirism_Rune; // This item is not combinable
    }

    public bool IsItemCombinable()
    {
        return true; // This item is not combinable
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
        return new SoulVampirism(player); // This item is not combinable, so it does not have a combined rune
    }


}