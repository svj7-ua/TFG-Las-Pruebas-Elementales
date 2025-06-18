using UnityEngine;

public class LifeIncreaseItem : MonoBehaviour, IItem
{
    [SerializeField]
    private EnumRunes rune = EnumRunes.LifeIncrease_Rune;
    private EnumItemType itemType = EnumItemType.Miscelaneous;
    private EnumItemCategories itemCategory = EnumItemCategories.InfiniteGeneration; // This item can spawn multiple times

    private GameObject player;

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        // Increases the player's maximum life by 10%

        float maxHealth = player.GetComponent<Health>().maxHealth;
        float addedmaxHealth = maxHealth * 0.1f; // Increase max health by 10%

        player.GetComponent<Health>().UpdateMaxHealth(Mathf.RoundToInt(addedmaxHealth + maxHealth)); // Update the player's max health
        player.GetComponent<Health>().Heal(Mathf.RoundToInt(addedmaxHealth)); // Heal the player for 10% of the new max health
    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set for Life Increase Item.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return "Increases your maximum life by 10% and heals you for the same amount.";
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
        return -1; // This item can spawn multiple times (No limit on spawns)
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
        return true;
    }

    public EnumRunes GetRuneToCombine()
    {
        // This item does not have a rune to combine
        return EnumRunes.LifeOrbHealingIncrease_Rune;
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