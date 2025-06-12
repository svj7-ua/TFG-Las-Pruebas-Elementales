using UnityEngine;

public class ElementalAdeptItem : MonoBehaviour, IItem
{

    [SerializeField]
    private EnumElementTypes elementalType = EnumElementTypes.None; // Elemental type of the item (Will aply ignored resistance or ignored immunity)
    [SerializeField]
    private EnumItemType itemType = EnumItemType.Ofensive; // Type of the item
    [SerializeField]
    private EnumItemCategories itemCategory = EnumItemCategories.ElementalAdept; // Category of the item
    private const int NUMBER_OF_SPAWNS = 2; // Number of spawns for this item
    private GameObject player;

    [SerializeField]
    private EnumRunes rune = EnumRunes.None; // Runes associated with the item, if any

    void Start()
    {
        if (rune == EnumRunes.None)
        {
            Debug.LogError("⚠️ ElementalAdeptItem: Rune not set. This item will not have any rune effects.");
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        // Apply the item effect to the player
        // Adds ignored resistance or immunity to the player attacks based on the elemental type (Immunity will be granted if the player already ignores the resistance)
        Debug.Log($"Applying ElementalAdeptItem with elemental type: {elementalType} to player: {player.name}");

        InventoryController inventoryController = player.GetComponent<InventoryController>();
        inventoryController.ignoredResistances[(int)elementalType] = true;
        inventoryController.elementalAdeptCount[(int)elementalType]++;

        // if (inventoryController.ignoredResistances[(int)elementalType])
        // {
        //     inventoryController.ignoredImmunities[(int)elementalType] = true;
        // }
        // else
        // {
        //     inventoryController.ignoredResistances[(int)elementalType] = true;
        // }

        // inventoryController.elementalAdeptCount[(int)elementalType]++;

    }

    public string getDescription()
    {
        return $"This rune grants the spells of the {elementalType} element the ability to ignore resistance to the {elementalType} element. ";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>($"Runes/Sprites/{elementalType} Rune Adept"); // Load the icon from Resources folder
    }

    public EnumItemType GetItemType()
    {
        return itemType;
    }

    public void RemoveItemEffect()
    {
        if (!CheckIfInitializedValues()) return;

        // Apply the item effect to the player
        // Adds ignored resistance or immunity to the player attacks based on the elemental type (Immunity will be granted if the player already ignores the resistance)
        Debug.Log($"Applying ElementalAdeptItem with elemental type: {elementalType} to player: {player.name}");

        InventoryController inventoryController = player.GetComponent<InventoryController>();
        if (inventoryController.ignoredImmunities[(int)elementalType])
        {
            inventoryController.ignoredImmunities[(int)elementalType] = false;
        }
        else
        {
            inventoryController.ignoredResistances[(int)elementalType] = false;
        }
    }

    public bool CheckIfInitializedValues()
    {
        if (player == null)
        {
            Debug.LogError("⚠️ ElementalAdeptItem: Player not set. Please set the player before applying the item.");
            return false;
        }

        if (elementalType == EnumElementTypes.None)
        {
            Debug.LogError("⚠️ ElementalAdeptItem: Elemental type not set. Please set the elemental type before applying the item.");
            return false;
        }


        return true;
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
        // This item does not remove all the category from spawning
        return false;
    }

    public EnumElementTypes GetElementalType()
    {
        return elementalType;
    }

    public EnumRunes GetRune()
    {
        return rune;
    }

    public bool IsItemCombinable()
    {
        // This item is not combinable
        return true;
    }

    public EnumRunes GetRuneToCombine()
    {
        // This rune combines with another one of itself to create a more powerful effect (Ignore the resistance or immunity)
        return rune; // Return the rune associated with the item
    }

    public IItem GetCombinedRune()
    {
        EnumRunes combinedRune = EnumRunes.None;

        switch (elementalType)
        {
            case EnumElementTypes.Fire:
                combinedRune = EnumRunes.FireElementalMasterAdept_Rune;
                break;
            case EnumElementTypes.Lightning:
                combinedRune = EnumRunes.LightningElementalMasterAdept_Rune;
                break;
            case EnumElementTypes.Arcane:
                combinedRune = EnumRunes.ArcaneElementalMasterAdept_Rune;
                break;
            case EnumElementTypes.Poison:
                combinedRune = EnumRunes.PoisonElementalMasterAdept_Rune;
                break;
            case EnumElementTypes.Wind:
                combinedRune = EnumRunes.WindElementalMasterAdept_Rune;
                break;
            default:
                Debug.LogError("⚠️ ElementalAdeptItem: Invalid elemental type for combining rune.");
                break;

        }
        IItem elementalMasterAdepts = new ElementalMasterAdepts(combinedRune, elementalType);
        elementalMasterAdepts.SetPlayer(player); // Set the player for the combined rune
        return elementalMasterAdepts; // This item is not combinable, so it does not have a combined rune
    }
}