using UnityEngine;

public class VampirismRune : MonoBehaviour, IItem
{
    [SerializeField]
    private EnumRunes rune = EnumRunes.BloodPact_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;

    EnumItemCategories itemCategory = EnumItemCategories.Singleton; // This item can spawn multiple times

    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        player.GetComponent<RunesModifiers>().vampirism = true; // Enable vampirism effect

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
        return "Grants a 5% chance to heal 1 health point when hitting an enemy.";
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
        return EnumRunes.SoulEater_Rune; // This item is not combinable
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

    public bool IsFusionRune()
    {
        return false; // This item is not a fusion rune
    }
}