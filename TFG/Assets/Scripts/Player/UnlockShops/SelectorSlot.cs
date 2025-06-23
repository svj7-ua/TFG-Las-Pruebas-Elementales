using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorSlot : MonoBehaviour
{

    [SerializeField]
    public SelectorController selectorController; // Reference to the SelectorController managing the selection
    public IEffect effect; // The effect associated with this item slot
    public IItem rune; // The item associated with this item slot
    public Image icon; // The icon of the item
    public string debugName; // Debug name for the item

    public Sprite selectedIcon;

    public Sprite unselectedIcon;

    public Sprite equipedIcon;

    public Sprite equipedAndSelectedIcon;

    private int row = -1; // Row index of the item panel
    private int column = -1; // Column index of the item panel

    private bool isEquiped = false; // Whether the item is equipped

    [SerializeField]
    private bool interacteable = true; // Whether the item panel is interactable

    [SerializeField]
    private bool isSpellCardSelector = false;

    void Start()
    {
        if (interacteable)
        {
            if (selectorController == null)
            {
                selectorController = GameObject.Find("SelectorSpellCards").GetComponent<SelectorController>(); // Find the SelectorController in the scene
                if (selectorController == null)
                {
                    Debug.LogError("SelectorController not found in the scene!"); // Log an error if the SelectorController is not found
                }
            }
        }
    }
    public void SetRow(int r)
    {
        row = r; // Set the row index of the item panel
    }

    public void SetColumn(int c)
    {
        column = c; // Set the column index of the item panel
    }


    public void OnClick()
    {

        if (icon.sprite == null)
        { // Check if the icon is null
            return; // If it is, do nothing
        }

        if (isSpellCardSelector)
        {
            selectorController.SelectSpellCard(row, column); // Call the SelectItem method in the SelectorController to select this item
        }
        else
        {
            selectorController.SelectRune(column); // Call the SelectItem method in the SelectorController to select this rune
        }

    }

    public void UnSelect()
    {
        //selected = false;
        if(isEquiped)
        {
            gameObject.GetComponent<Image>().sprite = equipedIcon; // Change the icon to the equipped one
            return; // If the item is equipped, do not change the icon to unselected
        }
        gameObject.GetComponent<Image>().sprite = unselectedIcon; // Change the icon to the unselected one
    }

    public void Select()
    {
        //selected = true;
        if(isEquiped)
        {
            gameObject.GetComponent<Image>().sprite = equipedAndSelectedIcon; // Change the icon to the equipped and selected one
            return; // If the item is equipped, do not change the icon to selected
        }
        gameObject.GetComponent<Image>().sprite = selectedIcon; // Change the icon to the selected one
    }

    public void Equip()
    {
        gameObject.GetComponent<Image>().sprite = equipedAndSelectedIcon; // Change the icon to the equipped one
        isEquiped = true; // Set the item as equipped
    }

    public void Unequip()
    {
        gameObject.GetComponent<Image>().sprite = unselectedIcon; // Change the icon to the unequipped one
        isEquiped = false; // Set the item as unequipped
    }
    
}
