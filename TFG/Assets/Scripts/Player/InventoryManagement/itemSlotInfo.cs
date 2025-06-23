using System;

[System.Serializable]
public class itemSlotInfo
{
    
    public IEffect effect; // The effect associated with this item slot
    public String name; // The name of the item

    public itemSlotInfo(IEffect effect, String name)
    {
        this.effect = effect;
        this.name = name;
    }

    public itemSlotInfo(IEffect effect)
    {
        this.effect = effect;
        this.name = "Item"; // Default name if not provided
    }

}