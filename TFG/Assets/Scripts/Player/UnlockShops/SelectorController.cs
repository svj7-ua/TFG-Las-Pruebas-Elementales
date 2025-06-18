using System.Collections.Generic;

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectorController : MonoBehaviour
{

    private SelectorSlot[,] spellCardPanels = new SelectorSlot[3, 8]; // 3D array to hold spell card panels
    private SelectorSlot[] runePanels; // 2D array to hold rune panels

    [SerializeField]
    private GameObject descriptionPanel; // Panel to display the description of the selected spell card or rune
    [SerializeField]
    private GameObject iconDisplay;

    private int row = -1;
    private int column = -1;

    [SerializeField]
    private Sprite lockedSprite; // Sprite to display when a spell card is locked

    [SerializeField]
    private bool isSpellCardSelector = false;
    private bool itemEquiped = false;
    private int equipedRow = -1; // Row index of the equipped item
    private int equipedColumn = -1; // Column index of the equipped item
    
    [Header("Set Up Only for Runes")]
    [SerializeField]
    private GameObject runesShop_GameObject; // Reference to the RunesShop component

    void Awake()

    {

        descriptionPanel.GetComponent<TextMeshProUGUI>().text = ""; // Clear the description panel at the start
        iconDisplay.GetComponent<Image>().sprite = null; // Clear the icon display at the start

        if (isSpellCardSelector)
        {
            LoadSpellCardPanels(); // Load spell card panels if this is a spell card selector
            if (GameData.equipedSpellCard != EnumSpellCards.None)
            {
                equipedRow = (int)GameData.spellCard.getSpellCardType(); // Set the row index based on the spell card type
                equipedColumn = ((int)GameData.spellCard.getSpellCard()) - 1; // Set the column index based on the spell card
                row = equipedRow; // Set the row index to the equipped row
                column = equipedColumn; // Set the column index to the equipped column
                GameData.spellCard = SpellCard_Factory.CreateSpellCard(GameData.spellCard.getSpellCard(), GameData.spellCard.getSpellCardType()); // Assign the equipped spell card to GameData
                itemEquiped = true; // Set itemEquiped to true as an item is equipped
            }
        }
        else
        {
            LoadRunePanels(); // Load rune panels if this is a rune selector
            if (GameData.equipedRune != EnumRunes.None)
            {
                equipedColumn = (int)GameData.rune.GetRune() - 1; // Set the column index based on the rune
                column = equipedColumn; // Set the column index to the equipped column
                itemEquiped = true; // Set itemEquiped to true as an item is equipped
                GameData.rune = runesShop_GameObject.GetComponent<RunesShop>().GetRuneItemByRune(GameData.rune.GetRune()); // Assign the equipped rune to GameData
            }
        }
        


    }

    public void OpenSelector(bool isSpellCardSelector)
    {

        iconDisplay.SetActive(false); // Hide the icon display at the start
        descriptionPanel.GetComponent<TextMeshProUGUI>().text = ""; // Clear the description panel at the start

        
        if (isSpellCardSelector)
        {
            LoadUnlockedSpellCards(); // Load unlocked spell cards
        }
        else
        {
            LoadUnlockedRunes(); // Load unlocked runes
        }

        if (!itemEquiped)
        {
            UnSelectLastSelected(); // Unselect the last selected panel before opening a new selector
            row = -1; // Reset the row index
            column = -1; // Reset the column index
        }
        else
        {
            UnSelectLastSelected(); // Unselect the last selected panel before opening a new selector
            SelectEquiped(); // Select the equipped item if it exists
        }


    }

    private void LoadUnlockedSpellCards()
    {
        // Loads all unlocked spellcards marked in GameData
        Debug.Log("Loading unlocked spell cards...");
        Debug.Log("SpellCard Panels Length: " + spellCardPanels.GetLength(0) + " x " + spellCardPanels.GetLength(1));


        // Loads melee spell cards
        for (int i = 0; i < spellCardPanels.GetLength(0); i++)
        {

            for (int j = 0; j < spellCardPanels.GetLength(1); j++)
            {
                if (GameData.unlockedSpellCards[j])
                {
                    Debug.Log("Loading spell card: " + (EnumSpellCards)(j + 1) + " of type: " + (EnumSpellCardTypes)i);
                    spellCardPanels[i, j].effect = SpellCard_Factory.CreateSpellCard((EnumSpellCards)(j + 1), (EnumSpellCardTypes)i); // Get the spell card from the factory and assign it to the panel
                    spellCardPanels[i, j].icon.sprite = spellCardPanels[i, j].effect.getIcon(); // Set the icon of the spell card panel
                    spellCardPanels[i, j].gameObject.GetComponent<Button>().interactable = true; // Enable interaction for the panel
                }
            }

        }
    }

    private void LoadUnlockedRunes()
    {
        // Loads all unlocked runes marked in GameData
        RunesShop runesShop = runesShop_GameObject.GetComponent<RunesShop>(); // Get the RunesShop component from the parent object
        if (runesShop == null)
        {
            Debug.LogError("RunesShop component not found in the parent object!"); // Log an error if the RunesShop component is not found
            return; // Exit the method if the RunesShop component is not found
        }

        for (int j = 0; j < runesShop.unlockedRunes.Count; j++)
        {
            Debug.Log("Loading rune: " + runesShop.unlockedRunes[j].GetComponent<IItem>().GetRune().ToString() + " at index: " + ((int)runesShop.unlockedRunes[j].GetComponent<IItem>().GetRune() - 1));
            runePanels[(int)runesShop.unlockedRunes[j].GetComponent<IItem>().GetRune() - 1].rune = runesShop.unlockedRunes[j].GetComponent<IItem>(); // Get the rune from the RunesShop and assign it to the panel
            runePanels[(int)runesShop.unlockedRunes[j].GetComponent<IItem>().GetRune() - 1].icon.sprite = runesShop.unlockedRunes[j].GetComponent<IItem>().getIcon(); // Set the icon of the rune panel
            runePanels[(int)runesShop.unlockedRunes[j].GetComponent<IItem>().GetRune() - 1].gameObject.GetComponent<Button>().interactable = true; // Enable interaction for the panel
        }

    }

    public void Unequip()
    {
        if (!itemEquiped) return;
        if (isSpellCardSelector)
        {
            spellCardPanels[equipedRow, equipedColumn].Unequip();
            SelectSpellCard(row, column); // Re-select the current spell card or rune panel
            GameData.equipedSpellCard = EnumSpellCards.None; // Clear the equipped spell card in GameData
            GameData.equipedSpellCardType = EnumSpellCardTypes.None; // Clear the equipped spell card type in GameData
        }
        else
        {
            runePanels[equipedColumn].Unequip(); // Unselect the equipped rune panel
            GameData.equipedRune = EnumRunes.None; // Clear the equipped rune in GameData
            SelectRune(column); // Re-select the current spell card or rune panel
        }

        equipedColumn = -1; // Reset the equipped column index
        equipedRow = -1; // Reset the equipped row index

        itemEquiped = false; // Set itemEquiped to false as the item is unequipped
        
    }

    public void Equip()
    {

        if (row == -1 && column == -1)
        {
            return; // If no item is selected, return
        }

        if (isSpellCardSelector)
        {
            spellCardPanels[row, column].Equip(); // Equip the selected spell card panel
            equipedRow = row; // Set the equipped row index
            equipedColumn = column; // Set the equipped column index
            itemEquiped = true; // Set itemEquiped to true as an item is equipped
            GameData.equipedSpellCard = spellCardPanels[row, column].effect.getSpellCard(); // Save the equipped spell card to GameData
            GameData.equipedSpellCardType = spellCardPanels[row, column].effect.getSpellCardType(); // Save the equipped spell card type to GameData
            Debug.LogWarning("Equipped Spell Card: " + GameData.equipedSpellCard + " of type: " + GameData.equipedSpellCardType);

            GameData.spellCard = spellCardPanels[row, column].effect;
        }
        else if (column != -1)
        {
            runePanels[column].Equip(); // Equip the selected rune panel
            equipedColumn = column; // Set the equipped column index
            equipedRow = -1; // Set the equipped row index to -1 as this is a rune selector
            itemEquiped = true; // Set itemEquiped to true as an item is equipped

            GameData.equipedRune = runePanels[column].rune.GetRune(); // Save the equipped rune to GameData
            GameData.rune = runePanels[column].rune; // Save the equipped rune item to GameData
        }
        else
        {
            Debug.Log("No rune selected to equip.");
        }

    }

    private void ClearEquippedItem()
    {
        if (column == -1)
        {
            GameData.equipedRune = EnumRunes.None; // Clear the equipped rune in GameData
            GameData.rune = null;
        }
        else
        {
            GameData.equipedSpellCard = EnumSpellCards.None; // Clear the equipped spell card in GameData
            GameData.equipedSpellCardType = EnumSpellCardTypes.None; // Clear the equipped spell card type in GameData
            GameData.spellCard = null; // Clear the equipped spell card item in GameData
        }
    }

    public void OnClick()
    {

        if (row == equipedRow && column == equipedColumn)
        {
            Debug.Log("Item already equipped, unequipping...");
            Unequip(); // If the selected item is already equipped, unequip it
            ClearEquippedItem();
            return;
        }

        if (itemEquiped)
        {
            Unequip();
        }

        Equip(); // Call the Equip method to equip the selected item
    }

    private void UnSelectLastSelected()
    {
        // Deselect the last selected panel
        if (row != -1 && column != -1)
        {
            
            spellCardPanels[row, column].UnSelect();
            descriptionPanel.GetComponent<TextMeshProUGUI>().text = ""; // Clear the description panel
            iconDisplay.GetComponent<Image>().sprite = null; // Clear the icon display
        }
        else if (row == -1 && column != -1)
        {
            runePanels[column].UnSelect();
            descriptionPanel.GetComponent<TextMeshProUGUI>().text = ""; // Clear the description panel
            iconDisplay.GetComponent<Image>().sprite = null; // Clear the icon display
        }
    }

    private void SelectEquiped()
    {
        Debug.Log("Selecting equipped item at row: " + equipedRow + ", column: " + equipedColumn);
        if (isSpellCardSelector)
        {
            Debug.LogWarning("Selecting equipped spell card at row: " + equipedRow + ", column: " + equipedColumn);
            spellCardPanels[equipedRow, equipedColumn].Equip(); // Equip the selected spell card panel
            descriptionPanel.GetComponent<TextMeshProUGUI>().text = spellCardPanels[equipedRow, equipedColumn].effect.getDescription(); // Display the description of the equipped spell card
            iconDisplay.GetComponent<Image>().sprite = spellCardPanels[equipedRow, equipedColumn].icon.sprite; // Display the icon of the equipped spell card
            iconDisplay.SetActive(true); // Ensure the icon display is active
        }
        else
        {
            runePanels[equipedColumn].Equip(); // Equip the selected rune panel
            descriptionPanel.GetComponent<TextMeshProUGUI>().text = runePanels[equipedColumn].rune.getDescription(); // Display the description of the equipped rune
            iconDisplay.GetComponent<Image>().sprite = runePanels[equipedColumn].icon.sprite; // Display the icon of the equipped rune
            iconDisplay.SetActive(true); // Ensure the icon display is active
        }
    }

    private void LoadSpellCardPanels()
    {
        // Load spell card panels from the scene or resources
        // Initialize spellCardPanels with the loaded data

        SelectorSlot[] slots = GetComponentsInChildren<SelectorSlot>();

        for (int i = 0; i < spellCardPanels.GetLength(0); i++)
        {
            for (int j = 0; j < spellCardPanels.GetLength(1); j++)
            {
                spellCardPanels[i, j] = slots[i * spellCardPanels.GetLength(1) + j];
                spellCardPanels[i, j].SetRow(i); // Set the row index of the spell card panel
                spellCardPanels[i, j].SetColumn(j); // Set the column index of the spell card panel
                spellCardPanels[i, j].gameObject.GetComponent<Button>().interactable = false; // Disable interaction for all panels initially
            }
        }

        for (int i = 0; i < spellCardPanels.GetLength(0); i++)
        {

            for (int j = 0; j < spellCardPanels.GetLength(1); j++)
            {

                spellCardPanels[i, j].icon.sprite = lockedSprite; // Set the icon of the spell card panel to the locked sprite


            }

        }

    }

    private void LoadRunePanels()
    {
        // Load rune panels from the scene or resources
        // Initialize runePanels with the loaded data

        runePanels = GetComponentsInChildren<SelectorSlot>();

        for (int i = 0; i < runePanels.Length; i++)
        {
            runePanels[i].SetRow(-1); // Set the row index of the rune panel to -1 as it is not applicable for runes
            runePanels[i].SetColumn(i); // Set the column index of the rune panel
            runePanels[i].icon.sprite = lockedSprite; // Set the icon of the rune panel to the locked sprite
            runePanels[i].gameObject.GetComponent<Button>().interactable = false; // Disable interaction for all panels initially
        }
    }

    public void SetRow(int r)
    {
        row = r; // Set the row index of the selector
    }

    public void SetColumn(int c)
    {
        column = c; // Set the column index of the selector
    }

    public void SelectSpellCard(int r, int c)
    {
        if (r < 0 || r >= spellCardPanels.GetLength(0) || c < 0 || c >= spellCardPanels.GetLength(1))
        {
            Debug.LogError("Invalid row or column index for spell card selection.");
            return;
        }

        // Deselect the last selected panel
        if (row != -1 && column != -1)
        {
            spellCardPanels[row, column].UnSelect();
        }

        row = r; // Set the row index of the selected spell card
        column = c; // Set the column index of the selected spell card

        // Select the specified panel
        spellCardPanels[r, c].Select();

        // Writes the description of the selected spell card to the console

        descriptionPanel.GetComponent<TextMeshProUGUI>().text = spellCardPanels[r, c].effect.getDescription();
        iconDisplay.GetComponent<Image>().sprite = spellCardPanels[r, c].icon.sprite; // Display the icon of the selected spell card
        iconDisplay.SetActive(true); // Ensure the icon display is active

    }

    public void SelectRune(int c)
    {
        if (c < 0 || c >= runePanels.Length)
        {
            Debug.LogError("Invalid row or column index for rune selection.");
            return;
        }

        // Deselect the last selected panel
        if (column != -1)
        {
            runePanels[column].UnSelect();
        }

        column = c; // Set the column index of the selected rune

        // Select the specified panel
        runePanels[c].Select();

        // Writes the description of the selected rune to the console
        descriptionPanel.GetComponent<TextMeshProUGUI>().text = runePanels[c].rune.getDescription();
        iconDisplay.GetComponent<Image>().sprite = runePanels[c].icon.sprite; // Display the icon of the selected rune
        iconDisplay.SetActive(true); // Ensure the icon display is active

    }



}