
using UnityEngine;

[System.Serializable]
public abstract class Item
{

    public abstract void PickUp(GameObject player); // Method to pick up the item

}

[System.Serializable]
public class SpellCardItem : Item
{

    public EnumSpellCards spellCard; // Type of spell card
    public EnumSpellCardTypes spellCardType; // Type of spell card (e.g., melee, ranged, etc.)

    public override void PickUp(GameObject player)
    {
        // Create the spell card using the factory
        IEffect effect = SpellCard_Factory.CreateSpellCard(spellCard, spellCardType);
        //player.GetComponent<PlayerController_test>().AddEffectToInventory(effect); // Add the spell card to the player's inventory
    }

}
