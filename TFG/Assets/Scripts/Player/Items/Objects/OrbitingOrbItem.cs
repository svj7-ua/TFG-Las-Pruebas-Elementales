using System;
using UnityEngine;

public class OrbitingOrbItem : MonoBehaviour, IItem
{

    [SerializeField]
    private EnumElementTypes elementalType = EnumElementTypes.None; // Elemental type of the item
    private GameObject player; // Reference to the player
    private EnumItemType itemType = EnumItemType.Miscelaneous; // Type of the item

    [SerializeField]
    private EnumItemCategories itemCategory = EnumItemCategories.Orb; // Category of the item

    private const int NUMBER_OF_SPAWNS = 4; // Number of spawns for this item

    [SerializeField]
    private EnumRunes rune = EnumRunes.None; // Runes associated with the item, if any

    void Start()
    {
        if (rune == EnumRunes.None)
        {
            Debug.LogError("⚠️ OrbitingOrbItem: Rune not set. This item will not have any rune effects.");
        }
    }

    public void ApplyItem()
    {

        if (!CheckIfInitializedValues()) return;

        // Apply the item effect to the player.

        OrbitController orbit = player.GetComponent<OrbitController>();

        if (orbit == null)
        {
            Debug.LogError("OrbitingOrbItem: Player does not have an OrbitController component.");
            return;
        }

        orbit.ActivateOrb(elementalType);

    }

    public bool CheckIfInitializedValues()
    {
        if (elementalType == EnumElementTypes.None || player == null)
        {
            Debug.LogError("OrbitingOrbItem: Element is not set. Or player is not set.");
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
        return Resources.Load<Sprite>($"Runes/Sprites/{elementalType} Rune Orb"); // Load the icon from Resources folder
    }

    public EnumItemType GetItemType()
    {
        return itemType;
    }

    public void RemoveItemEffect()
    {
        if (!CheckIfInitializedValues()) return;

        // Remove the item effect from the player
        Debug.Log($"Removing OrbitingOrbItem with elemental type: {elementalType} from player: {player.name}");

        OrbitController orbit = player.GetComponent<OrbitController>();

        if (orbit == null)
        {
            Debug.LogError("OrbitingOrbItem: Player does not have an OrbitController component.");
            return;
        }

        orbit.DeactivateOrb(elementalType);
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player; // Set the player reference

    }


    public int GetNumberOfSpawns()
    {
        return NUMBER_OF_SPAWNS;
    }

    public EnumItemCategories GetItemCategory()
    {
        return itemCategory;
    }

    public bool RemovesAllTheCategoryFromSpawning()
    {
        // After 4 orbs are spawned, the category is removed from spawning
        return true;
    }

    public EnumRunes GetRune()
    {
        return rune; // Return the rune associated with the item
    }
}