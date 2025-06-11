using UnityEngine;

public class SlashItem : MonoBehaviour, IItem
{


    [SerializeField]
    private GameObject slashPrefab; // Prefab of the slash effect

    private GameObject player; // Reference to the player
    private EnumItemType itemType = EnumItemType.Ofensive; // Type of the item
    [SerializeField]
    private EnumElementTypes elementalType = EnumElementTypes.None; // Elemental type of the item (Will apply resistance or immunity)
    [SerializeField]
    private EnumItemCategories itemCategory = EnumItemCategories.Slash; // Category of the item
    private const int NUMBER_OF_SPAWNS = 1; // Number of spawns for this item

    [SerializeField]
    private EnumRunes rune = EnumRunes.None; // Runes associated with the item, if any

    void Start()
    {

        if (rune == EnumRunes.None)
        {
            Debug.LogError("⚠️ SlashItem: Rune not set. This item will not have any rune effects.");
        }
    }

    public void ApplyItem()
    {
        if (!CheckIfInitializedValues()) return;

        // Apply the item effect to the player
        Debug.Log($"Applying SlashItem with prefab: {slashPrefab.name} to player: {player.name}");

        PlayerController_test playerController = player.GetComponent<PlayerController_test>();

        playerController.AddAttackWithAddedEffects(slashPrefab);
    }

    public bool CheckIfInitializedValues()
    {
        if (slashPrefab == null)
        {
            Debug.LogError("⚠️ SlashItem: slashPrefab is not set. Please assign a prefab in the inspector.");
            return false;
        }
        if (player == null)
        {
            Debug.LogError("⚠️ SlashItem: player is not set. Please set the player reference before applying the item.");
            return false;
        }
        if (elementalType == EnumElementTypes.None)
        {
            Debug.LogError("⚠️ SlashItem: elementalType is not set. Please assign an elemental type in the inspector.");
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
        return Resources.Load<Sprite>($"Runes/Sprites/{elementalType} Rune Slash"); // Load the icon from Resources folder
    }

    public EnumItemType GetItemType()
    {
        return itemType;
    }

    public void RemoveItemEffect()
    {
        if (!CheckIfInitializedValues()) return;

        // Remove the item effect from the player

        Debug.Log($"Removing SlashItem with prefab: {slashPrefab.name} from player: {player.name}");

        PlayerController_test playerController = player.GetComponent<PlayerController_test>();

        playerController.RemoveAttackWithAddedEffects(slashPrefab);
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
        // This item does not remove all the category from spawning
        return false;
    }

    public EnumRunes GetRune()
    {
        return rune; // Return the rune associated with the item
    }

}