using System;
using UnityEngine;

public interface IItem
{
    public void SetPlayer(GameObject player);   // Set the player that will use the item
    void ApplyItem();           // Apply the item effect to the player
    void RemoveItemEffect(); // Remove the item effect from the player. Not Implemented, since runes cannot be removed once applied. Considered for future items.
    Sprite getIcon(); // Get the icon of the item
    EnumItemType GetItemType(); // Get the type of the item (Offensive, Defensive or Miscellaneous)
    string getDescription(); // Get the description of the item
    int GetNumberOfSpawns();    // Get the number of times the item can be spawned

    EnumItemCategories GetItemCategory(); // Get the item category,
                                          // used during rune spawn pool selection
        
    EnumRunes GetRune();        // Get the rune of the item

    bool IsItemCombinable();    // Indicates if the item can be combined with another rune.
    bool IsFusionRune();    // Indicates if the item is a fusion rune already.
    EnumRunes GetRuneToCombine();       // Get the rune to combine with this item, if it is combinable.

    public IItem GetCombinedRune();     // Returns the result rune of the combination, if it is combinable.

    bool RemovesAllTheCategoryFromSpawning();   // Indicates if the item removes all the category from spawning.
                                                //  Only used for orb runes, but can be used for other items in the future.

    bool CheckIfInitializedValues();            // Checks if the item has been initialized with the correct values.

}