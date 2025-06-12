using System;
using UnityEngine;

public interface IItem
{
    public void SetPlayer(GameObject player);
    void ApplyItem();
    void RemoveItemEffect(); // Remove the item effect from the player
    Sprite getIcon(); // Get the icon of the item
    EnumItemType GetItemType(); // Get the spell card type of the item
    string getDescription(); // Get the description of the item
    int GetNumberOfSpawns();

    EnumItemCategories GetItemCategory(); // Get the item category
    EnumRunes GetRune();

    bool IsItemCombinable();
    EnumRunes GetRuneToCombine();

    public IItem GetCombinedRune();

    bool RemovesAllTheCategoryFromSpawning();

    bool CheckIfInitializedValues();

}