using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySelectionManagementController : MonoBehaviour
{
    private ItemPanel[,] panels = new ItemPanel[3,8]; // Array of item panels for each row

    private int selectedRow; // Index of the selected row
    private int selectedColumn; // Index of the selected column

    [SerializeField] private GameObject statsShowcase;
    [SerializeField] private GameObject iconShowcase;

    [SerializeField] private GameObject player; // Reference to the player object

    private InventoryController inventoryController; // Reference to the InventoryController

    // Start is called before the first frame update
    void Start()
    {

        selectedRow = -1; // Initialize the selected row index to -1 (no selection)
        selectedColumn = -1; // Initialize the selected column index to -1 (no selection)


        ItemPanel[] itemPanels = gameObject.GetComponentsInChildren<ItemPanel>(); // Get all item panels in the children of this object
        int aux_index = 0; // Auxiliary index to iterate through the item panels

        for (int i = 0; i < panels.GetLength(0); i++)
        {
            for (int j = 0; j < panels.GetLength(1); j++)
            {
                panels[i, j] = itemPanels[aux_index]; // Assign the item panel to the array
                panels[i, j].SetRow(i); // Set the row index of the item panel
                panels[i, j].SetColumn(j); // Set the column index of the item panel
                aux_index++;
            }
        }

        inventoryController = player.GetComponent<InventoryController>(); // Get the InventoryController component from the player object

    }

    void Awake()
    {
        selectedColumn = -1; // Initialize the selected column index to -1 (no selection)
        selectedRow = -1; // Initialize the selected row index to -1 (no selection)
    }

    public void ItemSelected(int row, int column){
        if(selectedRow != -1 && selectedColumn != -1){
            panels[selectedRow, selectedColumn].UnSelect(); // Unselect the previous item
        }

        if(selectedColumn == column && selectedRow == row){

            panels[selectedRow, selectedColumn].UnSelect(); // Unselect the previous item
            selectedRow = -1; // Reset the selected row index
            selectedColumn = -1; // Reset the selected column index

            //TODO: Unselecting an item should hide its description in the UI
            iconShowcase.GetComponent<Image>().sprite = null; // Hide the icon showcase
            iconShowcase.SetActive(false); // Hide the icon showcase
            statsShowcase.GetComponent<TextMeshProUGUI>().text = ""; // Hide the stats showcase
            statsShowcase.SetActive(false); // Hide the stats showcase

            return; // If the same item is clicked, do nothing
        }

        selectedRow = row; // Set the selected row
        selectedColumn = column; // Set the selected column
        panels[selectedRow, selectedColumn].Select(); // Select the new item

        //TODO: Selecting an item should show its description in the UI
        iconShowcase.GetComponent<Image>().sprite = panels[selectedRow, selectedColumn].icon.sprite; // Show the icon showcase with the selected item's icon
        Debug.Log("Selected item: " + panels[selectedRow, selectedColumn].icon.sprite.name); // Log the name of the selected item
        iconShowcase.SetActive(true); // Show the icon showcase

        statsShowcase.GetComponent<TextMeshProUGUI>().text = panels[selectedRow, selectedColumn].effect.getDescription(); // Show the description of the selected item in the stats showcase
        statsShowcase.SetActive(true); // Show the stats showcase

        Debug.Log("Selected row: " + selectedRow + ", column: " + selectedColumn); // Log the selected row and column
    }

    public void OnClickLeft(){
        Debug.Log("HOLI IZ: " + selectedRow + " " + selectedColumn);
        if(selectedRow == -1 || selectedColumn <= 0){
            return;
        }

        Debug.Log("Moving left selected icon: " + panels[selectedRow, selectedColumn].icon.sprite); // Log the movement to the left

        panels[selectedRow, selectedColumn].UnSelect(); // Unselect the previous item
        IEffect auxEffect = panels[selectedRow, selectedColumn].effect; // Get the effect of the selected item
        selectedColumn--; // Move to the left
        panels[selectedRow, selectedColumn].Select(); // Select the new item
        iconShowcase.GetComponent<Image>().sprite = panels[selectedRow, selectedColumn].icon.sprite; // Show the icon showcase with the selected item's icon

        panels[selectedRow, selectedColumn+1].ChangeEffect(panels[selectedRow, selectedColumn].effect); // Change the effect of the item to the right to the effect of the selected item
        panels[selectedRow, selectedColumn].ChangeEffect(auxEffect); // Change the effect of the selected item to the effect of the item to the right

        //Swaps effects
        inventoryController.SwapEffectOrder(selectedColumn, selectedColumn+1, selectedRow); // Swap the effects in the inventory controller
    }

    public void OnClickRight(){

        Debug.Log("HOLI DER: " + selectedRow + " " + selectedColumn);
        if(selectedRow == -1 || selectedColumn >= panels.GetLength(1)-1 || selectedColumn == -1){
            return;
        }

        var aux = inventoryController.GetCurrentItemsCount();

        Debug.Log("Current items count: " + aux[selectedRow]); // Log the current items count
        Debug.Log("Selected column: " + selectedColumn); // Log the selected column
        if(selectedColumn >= aux[selectedRow]-1) return; // Checks if the item selectes is the last one in the row

        Debug.Log("Moving left selected icon: " + panels[selectedRow, selectedColumn].icon.sprite); 
        panels[selectedRow, selectedColumn].UnSelect(); // Unselect the previous item
        IEffect auxEffect = panels[selectedRow, selectedColumn].effect; // Get the effect of the selected item
        selectedColumn++; // Move to the right
        panels[selectedRow, selectedColumn].Select(); // Select the new item
        iconShowcase.GetComponent<Image>().sprite = panels[selectedRow, selectedColumn].icon.sprite; // Show the icon showcase with the selected item's icon

        panels[selectedRow, selectedColumn-1].ChangeEffect(panels[selectedRow, selectedColumn].effect); // Change the effect of the item to the left to the effect of the selected item
        panels[selectedRow, selectedColumn].ChangeEffect(auxEffect); // Change the effect of the selected item to the effect of the item to the left

        //Swaps effects
        inventoryController.SwapEffectOrder(selectedColumn-1, selectedColumn, selectedRow); // Swap the effects in the inventory controller
    }



}
