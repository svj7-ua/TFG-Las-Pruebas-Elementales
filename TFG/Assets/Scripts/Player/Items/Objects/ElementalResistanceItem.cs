using UnityEngine;

public class ElementalResistanceItem : MonoBehaviour, IItem
{

    [SerializeField]
    private EnumElementTypes elementalType = EnumElementTypes.None; // Elemental type of the item (Will apply resistance or immunity)
    [SerializeField]
    private EnumItemType itemType = EnumItemType.Defensive; // Type of the item

    [SerializeField]
    private EnumItemCategories itemCategory = EnumItemCategories.ElementalResistance; // Category of the item
    private const int NUMBER_OF_SPAWNS = 1; // Only one spawn for this item, since applying immunity would be too powerful
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

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        // Apply the item effect to the player
        Debug.Log($"Applying ElementalResistanceItem with elemental type: {elementalType} to player: {player.name}");

        // Adds resistance or immunity to the player based on the elemental type
        Hurtbox hurtbox = player.GetComponent<Hurtbox>();
        InventoryController inventoryController = player.GetComponent<InventoryController>();

        switch (elementalType)
        {
            case EnumElementTypes.Fire:
                if (hurtbox.isResistantToFire)
                    hurtbox.isInmuneToFire = true;
                else
                    hurtbox.isResistantToFire = true;
                break;
            case EnumElementTypes.Lightning:
                if (hurtbox.isResistantToLightning)
                    hurtbox.isInmuneToLightning = true;
                else
                    hurtbox.isResistantToLightning = true;
                break;
            case EnumElementTypes.Poison:
                if (hurtbox.isResistantToPoison)
                    hurtbox.isInmuneToPoison = true;
                else
                    hurtbox.isResistantToPoison = true;
                break;
            case EnumElementTypes.Arcane:
                if (hurtbox.isResistantToArcane)
                    hurtbox.isInmuneToArcane = true;
                else
                    hurtbox.isResistantToArcane = true;
                break;
            case EnumElementTypes.Wind:
                if (hurtbox.isResistantToWind)
                    hurtbox.isInmuneToWind = true;
                else
                    hurtbox.isResistantToWind = true;
                break;
            default:
                Debug.LogError("ElementalResistanceItem: Invalid elemental type. Element: " + elementalType.ToString());
                break;
        }

        inventoryController.elementalResistancesCount[(int)elementalType]++;
        Debug.Log($"ElementalResistanceItem: {elementalType} resistance count: {inventoryController.elementalResistancesCount[(int)elementalType]}");

    }

    public bool CheckIfInitializedValues()
    {
        // Check if the elemental type and item type are initialized
        if (elementalType == EnumElementTypes.None || player == null)
        {
            Debug.LogError("ElementalResistanceItem: Elemental type or player not initialized.");
            return false;
        }
        return true;
    }

    public string getDescription()
    {
        return $"This rune grants the player resistance to the {elementalType} element, reducing damage by 50%. ";
    }

    public Sprite getIcon()
    {
        return Resources.Load<Sprite>($"Runes/Sprites/{elementalType} Rune Resistance"); // Load the icon from Resources folder
    }

    public EnumItemType GetItemType()
    {
        return itemType; // Return the item type
    }

    public void RemoveItemEffect()
    {
        if (!CheckIfInitializedValues()) return;

        // Remove the item effect from the player
        Debug.Log($"Removing ElementalResistanceItem with elemental type: {elementalType} from player: {player.name}");

        Hurtbox hurtbox = player.GetComponent<Hurtbox>();

        switch (elementalType)
        {
            case EnumElementTypes.Fire:
                if (hurtbox.isInmuneToFire)
                    hurtbox.isInmuneToFire = false;
                else
                    hurtbox.isResistantToFire = false;
                break;
            case EnumElementTypes.Lightning:
                if (hurtbox.isInmuneToLightning)
                    hurtbox.isInmuneToLightning = false;
                else
                    hurtbox.isResistantToLightning = false;
                break;
            case EnumElementTypes.Poison:
                if (hurtbox.isInmuneToPoison)
                    hurtbox.isInmuneToPoison = false;
                else
                    hurtbox.isResistantToPoison = false;
                break;
            case EnumElementTypes.Arcane:
                if (hurtbox.isInmuneToArcane)
                    hurtbox.isInmuneToArcane = false;
                else
                    hurtbox.isResistantToArcane = false;
                break;
            case EnumElementTypes.Wind:
                if (hurtbox.isInmuneToWind)
                    hurtbox.isInmuneToWind = false;
                else
                    hurtbox.isResistantToWind = false;
                break;
            default:
                Debug.LogError("ElementalResistanceItem: Invalid elemental type. Element: " + elementalType.ToString());
                break;
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
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

    public EnumRunes GetRune()
    {
        return rune; // Return the rune associated with the item
    }

    public bool IsItemCombinable()
    {
        // This item is not combinable
        return false;
    }

    public EnumRunes GetRuneToCombine()
    {
        // This item does not have a rune to combine
        return EnumRunes.None;
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