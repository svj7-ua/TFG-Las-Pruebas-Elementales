using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunesManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> runes; // List of runes that the player can collect
    [SerializeField]
    [Header("Runes Pool: Do not modify this in the inspector.")]
    [Tooltip("Runes Pool: Shows the runes that can be spawned in the scene. Do not modify this in the inspector, it will be initialized automatically.")]
    private List<GameObject> runesPool = new List<GameObject>(); // Pool of runes that can be spawned in the scene
    private List<GameObject> runesInScene = new List<GameObject>(); // List of runes that are currently in the scene (Will be removed from the pool but if the player didn't pick them up, they will be added to the pool again once the next level is loaded)
    private List<GameObject> runesRemovedFromPool = new List<GameObject>(); // List of runes that have been generated in the scene

    private int orbsGenerated = 0; // Counter for the number of orbs generated
    private int[] elementalAdeptItemsGenerated = new int[System.Enum.GetNames(typeof(EnumElementTypes)).Length - 1]; // Counter for the number of elemental adept items generated for each element

    private InventoryController inventoryController; // Reference to the InventoryController to manage the player's inventory
    private OrbitController orbitController; // Reference to the OrbitController to manage the orbs

    [Space]
    [Header("Debug Variables")]
    [SerializeField] public bool debugMode = false; // Flag to enable or disable debug mode
    [SerializeField] private int debugRuneIndexToGenerate = 0; // Index of the rune to generate for debugging purposes

    private void Awake()
    {
        // Gets the component from the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("RunesManager: Player object not found in the scene.");
            return;
        }
        inventoryController = player.GetComponent<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogError("RunesManager: InventoryController component not found on the player.");
        }
        orbitController = player.GetComponent<OrbitController>();
        if (orbitController == null)
        {
            Debug.LogError("RunesManager: OrbitController component not found on the player.");
        }
    }

    void Start()
    {
        runesPool = new List<GameObject>(runes); // Initialize the pool with the runes

        // Check if the Scene is GameScene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            FixStartingPool();
        }

        if (debugMode)
        {
            Debug.LogWarning("RunesManager: Debug mode is enabled. Generating rune at index: " + debugRuneIndexToGenerate);
        }
    }

    public void FixStartingPool()
    {
        if (GameData.rune != null)
        {
            switch (GameData.rune.GetItemCategory())
            {
                case EnumItemCategories.Orb:
                    orbsGenerated++;
                    break;

                case EnumItemCategories.ElementalAdept:
                    if(GameData.rune.GetRune() == EnumRunes.PoisonElementalAdept_Rune)  elementalAdeptItemsGenerated[(int)EnumElementTypes.Poison]++;
                    else if (GameData.rune.GetRune() == EnumRunes.FireElementalAdept_Rune)  elementalAdeptItemsGenerated[(int)EnumElementTypes.Fire]++;
                    else if (GameData.rune.GetRune() == EnumRunes.LightningElementalAdept_Rune)  elementalAdeptItemsGenerated[(int)EnumElementTypes.Lightning]++;
                    else if (GameData.rune.GetRune() == EnumRunes.WindElementalAdept_Rune)  elementalAdeptItemsGenerated[(int)EnumElementTypes.Wind]++;
                    else elementalAdeptItemsGenerated[(int)EnumElementTypes.Arcane]++;
                    break;

                case EnumItemCategories.InfiniteGeneration:
                    break;

                default:
                    RemoveRuneFromPool(GameData.equipedRune);
                    break;
            }
        }
    }

    private void RemoveRuneFromPool(EnumRunes r)
    {
        for (int i = 0; i < runesPool.Count; i++)
        {
            if (runesPool[i].GetComponent<IItem>().GetRune() == r)
            {
                runesRemovedFromPool.Add(runesPool[i]); // Add the rune to the list of removed runes
                runesPool.RemoveAt(i); // Remove the rune from the pool
                Debug.Log("RunesManager: Removed rune from pool: " + r);
                return;
            }
        }
    }

    public GameObject GenerateRune()
    {
        if (runesPool.Count == 0)
        {
            Debug.LogWarning("RunesManager: No runes available to generate.");
            return null;
        }

        // Randomly select a rune from the pool
        int randomIndex = Random.Range(0, runesPool.Count);
        if (debugMode) randomIndex = debugRuneIndexToGenerate; // Use the debug index if debug mode is enabled
        GameObject rune = runesPool[randomIndex];

        switch (rune.GetComponent<IItem>().GetItemCategory())
        {
            case EnumItemCategories.Orb:
                orbsGenerated++;
                if (orbsGenerated >= rune.GetComponent<IItem>().GetNumberOfSpawns())
                {
                    // Removes all orbs from the pool if the maximum number of orbs has been generated

                    Debug.Log("RunesManager: Maximum number of orbs generated. Removing all orbs from the pool.");
                    runesInScene.AddRange(runesPool.FindAll(item => item.GetComponent<IItem>().GetItemCategory() == EnumItemCategories.Orb));
                    runesPool.RemoveAll(item => item.GetComponent<IItem>().GetItemCategory() == EnumItemCategories.Orb);
                }
                break;

            case EnumItemCategories.ElementalAdept:
                EnumElementTypes elementalType = rune.GetComponent<ElementalAdeptItem>().GetElementalType();
                int index = (int)elementalType; // Enum is 0-based, so we can use it directly as an index
                elementalAdeptItemsGenerated[index]++;
                if (elementalAdeptItemsGenerated[index] >= rune.GetComponent<IItem>().GetNumberOfSpawns())
                {
                    // Removes all ElementalAdept items from the pool if the maximum number of ElementalAdept items has been generated for this element

                    Debug.LogWarning($"RunesManager: Maximum number of ElementalAdept items generated for {elementalType}. Cannot generate more.");
                    runesInScene.AddRange(runesPool.FindAll(item => item.GetComponent<IItem>().GetItemCategory() == EnumItemCategories.ElementalAdept && item.GetComponent<ElementalAdeptItem>().GetElementalType() == elementalType));
                    runesPool.RemoveAll(item => item.GetComponent<IItem>().GetItemCategory() == EnumItemCategories.ElementalAdept && item.GetComponent<ElementalAdeptItem>().GetElementalType() == elementalType);

                }

                break;
            case EnumItemCategories.InfiniteGeneration:
                // Removes the rune from the pool if it can't spawn more than once
                runesInScene.Add(rune);
                break;
            default:
                // Removes the rune from the pool if it can't spawn more than once
                runesInScene.Add(rune);
                runesPool.RemoveAt(randomIndex);
                break;
        }
        Debug.LogWarning("RunesManager: Rune generated: " + rune.name + " RunesInScene count: " + runesInScene.Count + " RunesPool count: " + runesPool.Count);
        // Instantiate the rune in the scene

        return rune;
    }


    public void RemoveRuneFromScene(GameObject rune)
    {
        //Debug.LogWarning("RunesManager: Removing rune from scene: " + rune.name + " RunesInScene count: " + runesInScene.Count + " RunesPool count: " + runesPool.Count);
        for (int i = 0; i < runesInScene.Count; i++)
        {
            if (runesInScene[i].GetComponent<IItem>().GetRune() == rune.GetComponent<IItem>().GetRune())
            {
                runesInScene.RemoveAt(i); // Remove the rune from the list of runes in the scene
                break;
            }
        }
        
        //Debug.LogWarning("RunesManager: Rune removed from scene: " + rune.name + " RunesInScene count: " + runesInScene.Count + " RunesPool count: " + runesPool.Count);
    }

    public void ReloadRunesNotPickedUp()
    {

        if (orbitController.GetCurrentNumberOfOrbs() >= 4)
        {
            runesInScene.RemoveAll(item => item.GetComponent<IItem>().GetItemCategory() == EnumItemCategories.Orb);
        }

        // Reload the runes that were not picked up by the player
        foreach (GameObject rune in runesInScene)
        {
            if (!runesPool.Contains(rune))
            {
                runesPool.Add(rune);
            }
        }
        runesInScene.Clear(); // Clear the list of runes in the scene
        orbsGenerated = orbitController.GetCurrentNumberOfOrbs(); // Resets the counter for orbs generated to the current number of orbs in the orbit controller
        
        for (int i = 0; i < elementalAdeptItemsGenerated.Length; i++)
        {
            elementalAdeptItemsGenerated[i] = inventoryController.elementalAdeptCount[i]; // Sets the counter for elemental adept items generated to the current count in the inventory
        }

    }

    

}
