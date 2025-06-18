using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public List<IEffect> meleeEffectsInventory = new List<IEffect>();
    public List<IEffect> rangedEffectsInventory = new List<IEffect>();
    public List<IEffect> dash_AOE_EffectsInventory = new List<IEffect>();

    [Space]
    [Header("Inventory UI Components")]
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] public GameObject itemSlotPrefab;
    [SerializeField] public GameObject meleeGrid;
    [SerializeField] public GameObject rangedGrid;
    [SerializeField] public GameObject dashAOE_Grid;
    [SerializeField] public GameObject runesGrid;

    [Space]
    [Header("Summary UI")]
    [SerializeField] public GameObject summaryPanel;
    [SerializeField] public GameObject summaryMeleeGrid;
    [SerializeField] public GameObject summaryRangedGrid;
    [SerializeField] public GameObject summaryDashAOE_Grid;
    [SerializeField] public GameObject summaryRunesGrid;
    [SerializeField] public GameObject LevelText;
    [SerializeField] public GameObject TimeText;
    [SerializeField] public GameObject GemsText;
    [SerializeField] public GameObject summaryGemsText;
    [SerializeField] public GameObject summaryLevelText;
    [SerializeField] public GameObject summaryTimeText;

    [Space]
    [Header("Secondary Effects UI")]
    [SerializeField] public GameObject ignitedIcon;
    [SerializeField] public GameObject poisonedIcon;
    [SerializeField] public GameObject arcanedIcon;
    [SerializeField] public GameObject slowedIcon;
    [SerializeField] public GameObject electrifiedIcon;


    [Space]
    public int INVENTORY_SIZE = 8;

    private int[] currentSpellCardsCount = {0, 0, 0}; // [melee, ranged, dash]

    private bool[,] hasItem = new bool[3, 8]; // [type, index]

    [Space]
    [Header("Passive Items")]

    private List<IItem> offensiveItems = new List<IItem>();
    private List<IItem> defensiveItems = new List<IItem>();
    private List<IItem> miscelaneousItems = new List<IItem>();

    private List<IItem> items = new List<IItem>();


    [Header("Variables used to modify effects")]
    [Tooltip("[Poison, Fire, Lightning, Wind, Arcane]")]
    public bool[] ignoredResistances = new bool[5]; // [Poison, Fire, Lightning, Wind, Arcane]
    [Tooltip("[Poison, Fire, Lightning, Wind, Arcane]")]
    public bool[] ignoredImmunities = new bool[5]; // [Poison, Fire, Lightning, Wind, Arcane]

    public int[] elementalResistancesCount = new int[5]; // [Poison, Fire, Lightning, Wind, Arcane]
    public int[] elementalAdeptCount = new int[5]; // [Poison, Fire, Lightning, Wind, Arcane]

    private RunesManager runesManager; // Reference to the RunesManager to manage the player's runes
    public RunesModifiers runesModifiers; // Reference to the RunesModifiers to modify the player's stats based on the runes
    public int gemsPickedUpDuringRun = 0; // Amount of gems the player has picked up during the current run
    [SerializeField]
    private GameObject gemsAmountText; // Reference to the text object that displays the amount of gems (not used yet, but can be used in the future)
    private float timeSpentInRun = 0f; // Time spent in the current run

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            gemsAmountText.GetComponent<TextMeshProUGUI>().text = $": {GameData.gems}";
        }
        else
        {
            // When the game starts, initialize the gems amount text to 0
            gemsAmountText.GetComponent<TextMeshProUGUI>().text = $": {gemsPickedUpDuringRun}"; // Initialize the gems amount text
            SetUpInitialEquipment(); // Set up the initial equipment based on GameData
        }

        
        runesManager = FindObjectOfType<RunesManager>();
        runesModifiers = GetComponent<RunesModifiers>();
        if (runesManager == null)
        {
            Debug.LogError("InventoryController: RunesManager not found in the scene.");
        }
    }

    private void SetUpInitialEquipment()
    {
        if (GameData.spellCard != null)
        {
            switch (GameData.spellCard.getSpellCardType())
            {
                case EnumSpellCardTypes.Melee:
                    AddMeleeEffect(GameData.spellCard);
                    break;
                case EnumSpellCardTypes.Ranged:
                    AddRangedEffect(GameData.spellCard);
                    break;
                case EnumSpellCardTypes.Dash_AOE:
                    AddDashEffect(GameData.spellCard);
                    break;
                default:
                    Debug.LogWarning("Unknown spell card type: " + GameData.spellCard.getSpellCardType());
                    break;
            }
        }

        if (GameData.rune != null)
        {
            AddItem(GameData.rune); // Add the rune to the inventory
        }


    }

    private void Start()
    {
        //Saves the time the run started
        timeSpentInRun = Time.time; // Initialize the time spent in the run
    }
 

    public void AddGems(int amount)
    {
        gemsPickedUpDuringRun += amount; // Add the amount of gems to the player's total picked up during the run
        Debug.Log("Gems added: " + amount + ". Total gems: " + gemsPickedUpDuringRun);
        gemsAmountText.GetComponent<TextMeshProUGUI>().text = $": {gemsPickedUpDuringRun}"; // Sets the text of the gems amount to the current amount of gems
        //UpdateGemsText(); // Update the UI text to reflect the new amount of gems
    }

    private bool isEffectRepeated(IEffect effect, EnumSpellCardTypes type)
    {
        return hasItem[(int)type, (int)effect.getSpellCard() - 1];        // Subtract 1 to get the index, since 0 is none for the spell cards
    }

    private void UpgradeEffect(IEffect effect, EnumSpellCardTypes type)
    {
        var list = type == EnumSpellCardTypes.Melee ? meleeEffectsInventory :
            type == EnumSpellCardTypes.Ranged ? rangedEffectsInventory :
            dash_AOE_EffectsInventory;

        foreach (var item in list){
            if(item.getSpellCard() == effect.getSpellCard()){
                Debug.Log("Upgrading effect: " + item.GetType().Name);
                item.UpgradeEffect(); // Apply the effect to the player
                return; // Exit the loop after applying the effect
            }
        }
    }

    // Items Related Methods

    public void AddItem(GameObject rune)
    {
        IItem item = rune.GetComponent<IItem>();
        item.SetPlayer(gameObject); // Set the player reference in the item

        if (item.IsItemCombinable() && CheckIfTheItemCombines(item)) // Check if the item can be combined with another item
        {
            CombineItem(item); // Combine the item if it can be combined
        }
        else
        {

            AddItem(item); // Add the item to the inventory list 
        }

        Debug.Log("Added item: " + item.GetType().Name);
        
    }
    
    private void AddItem(IItem item)
    {
        item.SetPlayer(gameObject); // Set the player reference in the item
        item.ApplyItem();
        items.Add(item); // Add the item to the inventory list
    }

    public void RemoveItem(IItem item)
    {
        var list = item.GetItemType() == EnumItemType.Ofensive ? offensiveItems :
            item.GetItemType() == EnumItemType.Defensive ? defensiveItems :
            miscelaneousItems;

        if (list.Contains(item))
        {
            list.Remove(item);
            item.RemoveItemEffect(); // Remove the item effect from the player
            Debug.Log("Removed item: " + item.GetType().Name);
        }
        else
        {
            Debug.LogWarning("Item not found in inventory: " + item.GetType().Name);
        }

    }

    private void CombineItem(IItem item)
    {
        IItem combinedItem = item.GetCombinedRune(); // Get the combined rune item
        if (combinedItem != null)
        {
            combinedItem.ApplyItem(); // Apply the combined item effect
            AddItem(combinedItem); // Add the combined item to the inventory
            Debug.Log("Combined item: " + combinedItem.GetType().Name);
        }
        else
        {
            Debug.LogWarning("No combined item found for: " + item.GetType().Name);
        }
    }

    private bool CheckIfTheItemCombines(IItem item)
    {

        for (int i = 0; i < items.Count; i++)
        {
            if (item.GetRuneToCombine() == items[i].GetRune())
            {
                items.RemoveAt(i); // Remove the item that is being combined
                return true; // Return true if the item was combined
            }
        }

        return false;
    }


    
    public void AddMeleeEffect(IEffect effect)
    {
        if (!isEffectRepeated(effect, EnumSpellCardTypes.Melee))
        {
            meleeEffectsInventory.Add(effect);
            currentSpellCardsCount[0]++;
            hasItem[(int)EnumSpellCardTypes.Melee, (int)effect.getSpellCard() - 1] = true; // Set the item as added
        }
        else
        {
            UpgradeEffect(effect, EnumSpellCardTypes.Melee); // Upgrade the effect if it already exists
        }

        Debug.Log("Added impact effect:" + effect.GetType().Name);
    }

    public void AddRangedEffect(IEffect effect)
    {
        if(!isEffectRepeated(effect, EnumSpellCardTypes.Ranged)){
            rangedEffectsInventory.Add(effect);
            currentSpellCardsCount[1]++;
            hasItem[(int)EnumSpellCardTypes.Ranged, (int)effect.getSpellCard()-1] = true; // Set the item as added
        } else {
            UpgradeEffect(effect, EnumSpellCardTypes.Ranged); // Upgrade the effect if it already exists
        }
        
        Debug.Log("Added ranged effect:" + effect.GetType().Name);
    }

    public void AddDashEffect(IEffect effect)
    {
        if(!isEffectRepeated(effect, EnumSpellCardTypes.Dash_AOE)){
            dash_AOE_EffectsInventory.Add(effect);
            currentSpellCardsCount[2]++;
            hasItem[(int)EnumSpellCardTypes.Dash_AOE, (int)effect.getSpellCard()-1] = true; // Set the item as added
        } else {
            UpgradeEffect(effect, EnumSpellCardTypes.Dash_AOE); // Upgrade the effect if it already exists
        }
        Debug.Log("Added dash effect:" + effect.GetType().Name);
    }



    public int[] GetcurrentSpellCardsCount()
    {
        return currentSpellCardsCount;
    }

    public void Summary()
    {

        ListEffects(true); // List effects in summary mode
        
        float timeSpent = Time.time - timeSpentInRun;

        int minutes = (int)timeSpent / 60;
        int seconds = (int)timeSpent % 60;

        summaryTimeText.GetComponent<TextMeshProUGUI>().text = $"Time: {minutes}:{seconds:D2}";

        LevelInformation levelInfo = FindObjectOfType<LevelInformation>();
        if (GetComponent<Health>().currentHealth <= 0)
        {
             LevelText.GetComponent<TextMeshProUGUI>().text = $"Level: {levelInfo.GetLevel()}"; // Get the current level from LevelInformation
        }
        else
        {
            LevelText.GetComponent<TextMeshProUGUI>().text = $"RUN COMPLETED!"; // Get the current level from LevelInformation
        }
        summaryLevelText.GetComponent<TextMeshProUGUI>().text = $"Level: {levelInfo.GetLevel()}"; // Update the summary level text
        summaryGemsText.GetComponent<TextMeshProUGUI>().text = $"Gems: {gemsPickedUpDuringRun}"; // Update the summary gems text

        Debug.Log("Gems before: "+ GameData.gems +" Gems picked up during run: " + gemsPickedUpDuringRun);
        GameData.gems += gemsPickedUpDuringRun; // Add the gems picked up during the run to the total gems
        summaryPanel.SetActive(true); // Show the summary panel

    }

    public void ListEffects(bool summary = false)
    {
        Debug.Log("Effects Inventory: ");

        ListMelee(summary); // Listar efectos de impacto

        ListRanged(summary); // Listar efectos de proyectiles

        ListDash(summary); // Listar efectos de dash/AOE

        ListRunes(summary); // Listar runas

    }

    private void ListRunes(bool summary = false)
    {
        ItemPanel[] slots;
        if (summary)
        {
            slots = summaryRunesGrid.GetComponentsInChildren<ItemPanel>();
        } else {
            slots = runesGrid.GetComponentsInChildren<ItemPanel>();
        }

        Debug.Log("Listing runes: " + items.Count);
        int index = 0;
        foreach (var rune in items)
        {
            Debug.Log("Adding rune to inventory: " + rune.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más runas que espacios

            // Obtener el ícono del efecto
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = rune.getIcon(); // Asignar el ícono al slot
            slots[index].rune = rune; // Asignar el efecto al slot
            index++;
        }
    }

    private void ListMelee(bool summary = false)
    {
        ItemPanel[] slots;
        if (summary)
            slots = summaryMeleeGrid.GetComponentsInChildren<ItemPanel>();
        else
            slots = meleeGrid.GetComponentsInChildren<ItemPanel>();
        Debug.Log("Listing impact effects: " + meleeEffectsInventory.Count);
        int index = 0;
        foreach (var effect in meleeEffectsInventory)
        {
            Debug.Log("Adding to inventory: " + effect.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más efectos que espacios

            // Obtener el ícono del efecto

            // Buscar el componente Image en el slot actual

            Debug.Log("Slot " + slots[index].debugName + " setting icon");
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = effect.getIcon(); // Asignar el ícono al slot
            slots[index].effect = effect; // Asignar el efecto al slot


            index++;
        }

    }

    private void ListRanged( bool summary = false){
        
        ItemPanel[] slots;
        if (summary)
            slots = summaryRangedGrid.GetComponentsInChildren<ItemPanel>();
        else
            slots = rangedGrid.GetComponentsInChildren<ItemPanel>();

        Debug.Log("Listing ranged effects: " + rangedEffectsInventory.Count);
        int index = 0;
        foreach (var effect in rangedEffectsInventory)
        {
            Debug.Log("Adding to inventory: " + effect.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más efectos que espacios

            // Obtener el ícono del efecto

            // Buscar el componente Image en el slot actual

            Debug.Log("Slot " + slots[index].debugName + " setting icon");
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = effect.getIcon(); // Asignar el ícono al slot
            slots[index].effect = effect; // Asignar el efecto al slot


            index++;
        }

    }

    private void ListDash(bool summary = false){
        
        ItemPanel[] slots;
        if (summary)
            slots = summaryDashAOE_Grid.GetComponentsInChildren<ItemPanel>();
        else
            slots = dashAOE_Grid.GetComponentsInChildren<ItemPanel>();

        Debug.Log("Listing dash effects: " + dash_AOE_EffectsInventory.Count);
        int index = 0;
        foreach (var effect in dash_AOE_EffectsInventory)
        {
            Debug.Log("Adding to inventory: " + effect.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más efectos que espacios

            // Obtener el ícono del efecto

            // Buscar el componente Image en el slot actual

            Debug.Log("Slot " + slots[index].debugName + " setting icon");
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = effect.getIcon(); // Asignar el ícono al slot
            slots[index].effect = effect; // Asignar el efecto al slot


            index++;
        }

    }

    public void DEBUG_ListImpactEffects()
    {
        Debug.Log("Impact Effects Inventory: ");
        foreach (var effect in meleeEffectsInventory)
        {
            Debug.Log(effect.GetType().Name);
        }
    }

    public void SwapEffectOrder(int index1, int index2, int type)
    {
        
        var list = type == 0 ? meleeEffectsInventory :
            type == 1 ? rangedEffectsInventory :
            dash_AOE_EffectsInventory;

        if (index1 >= 0 && index2 >= 0 && index1 < list.Count && index2 < list.Count)
        {
            (list[index1], list[index2]) = (list[index2], list[index1]);
        }
    }

    // Applies the effects starting from the index of the inventory up to the end of the inventory.
    public void ApplyEffects(GameObject target, int index, EnumSpellCardTypes type)
    {
        Debug.Log("Applying effects from index " + index + " to target: " + target.name);
        var list = type == EnumSpellCardTypes.Melee ? meleeEffectsInventory :
        type == EnumSpellCardTypes.Ranged ? rangedEffectsInventory :
        dash_AOE_EffectsInventory;

        if(index < list.Count){
            Debug.Log("Applying effect: " + list[index].GetType().Name + " to target: " + target.name);
            list[index].ApplyEffect(target, index);
        } else {
            Debug.Log("No more effects to Apply (Number of effects: " + list.Count + ")");
        }

    }

}
