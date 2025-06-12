using System;
using UnityEngine;

public class ElementalMasterAdepts : IItem
{

    private EnumRunes rune;
    private EnumElementTypes elementalType = EnumElementTypes.None; // Elemental type of the item (Will apply ignored resistance or ignored immunity)
    private GameObject player;

    public ElementalMasterAdepts(EnumRunes rune, EnumElementTypes elementalType)
    {
        this.rune = rune;
        this.elementalType = elementalType;
    }


    public void ApplyItem()
    {
        if(!CheckIfInitializedValues())
            return;
        // Apply the item effect to the player
        // Sets the player to ignore immunity to the elemental type
        Debug.Log($"Applying ElementalMasterAdepts with rune: {rune} and elemental type: {elementalType} to player: {player.name}");
        InventoryController inventoryController = player.GetComponent<InventoryController>();
        inventoryController.ignoredImmunities[(int)elementalType] = true;
        inventoryController.elementalAdeptCount[(int)elementalType]++;
    }

    public bool CheckIfInitializedValues()
    {
        if (rune == EnumRunes.None)
        {
            Debug.LogError("⚠️ ElementalMasterAdepts: Rune not set. This item will not have any rune effects.");
            return false;
        }
        if (elementalType == EnumElementTypes.None)
        {
            Debug.LogError("⚠️ ElementalMasterAdepts: Elemental type not set. This item will not have any elemental effects.");
            return false;
        }
        if (player == null)
        {
            Debug.LogError("⚠️ ElementalMasterAdepts: Player not set. This item cannot be applied without a player.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return $"This rune grants the spells of the {elementalType} element the ability to ignore immunity to the {elementalType} element. ";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>($"Runes/Sprites/{elementalType} Rune MasterAdept"); // Load the icon from Resources folder
    }

    public EnumItemCategories GetItemCategory()
    {
        return EnumItemCategories.Singleton; // Category of the item
    }

    public EnumItemType GetItemType()
    {
        return EnumItemType.Ofensive; // Assuming this is the correct item type for this rune
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
}