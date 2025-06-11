using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemPanel : MonoBehaviour
{

    public InventoryController inventoryController;
    public IEffect effect; // The effect associated with this item slot
    public Image icon; // The icon of the item
    public string debugName; // Debug name for the item

    public GameObject Inventory;

    public Sprite selectedIcon;

    public Sprite unselectedIcon;

    private int row = -1; // Row index of the item panel
    private int column = -1; // Column index of the item panel

    [SerializeField]
    private bool interacteable = true; // Whether the item panel is interactable


    void Start()
    {
        if (interacteable)
        {
            Inventory = GameObject.Find("Inventory"); // Find the InventoryController in the scene
            if (Inventory == null)
            {
                Debug.LogError("InventoryController not found in the scene!"); // Log an error if the InventoryController is not found
            }
        }
    }

    public void SetRow(int r){
        row = r; // Set the row index of the item panel
    }

    public void SetColumn(int c){
        column = c; // Set the column index of the item panel
    }

    public void OnClick(){

        if(icon.sprite == null){ // Check if the icon is null
            return; // If it is, do nothing
        }

        Inventory.GetComponent<InventorySelectionManagementController>().ItemSelected(row, column); // Call the SelectItem method in the InventorySelectionManagementController to select this item
    }

    public void UnSelect(){
        //selected = false;
        gameObject.GetComponent<Image>().sprite = unselectedIcon; // Change the icon to the unselected one
    }

    public void Select(){
        //selected = true;
        gameObject.GetComponent<Image>().sprite = selectedIcon; // Change the icon to the selected one
    }

    public void ChangeEffect(IEffect newEffect){
        effect = newEffect; // Change the effect of the item slot
        icon.sprite = effect.getIcon(); // Change the icon of the item slot
    }
    
}