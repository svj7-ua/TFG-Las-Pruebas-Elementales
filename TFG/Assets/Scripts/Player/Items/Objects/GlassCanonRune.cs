using UnityEngine;

public class GlassCanonRune : MonoBehaviour, IItem
{

    [SerializeField]
    private EnumRunes rune = EnumRunes.GlassCanon_Rune;
    private EnumItemType itemType = EnumItemType.Ofensive;

    EnumItemCategories itemCategory = EnumItemCategories.Singleton; // This item can spawn multiple times

    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;



        player.GetComponent<RunesModifiers>().damageMultiplier += 1f; // Increase damage dealt by 100%
        player.GetComponent<RunesModifiers>().receivedDamageMultiplier += 1f; // Increase damage received by 100%


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
        return "Your attacks deal 100% more damage, but you also receive 100% more damage from all sources.";
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