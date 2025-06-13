using UnityEngine;

[System.Serializable]
public class PlayerGameData
{

    public int gems = 0; // The amount of gems the player has
    public bool[] unlockedSpellCards = new bool[EnumSpellCards.GetValues(typeof(EnumSpellCards)).Length - 1]; // The spell cards the player has unlocked

    public bool[] unlockedRunes = new bool[EnumRunes.GetValues(typeof(EnumRunes)).Length - 1]; // The runes the player has unlocked

    public int equipedSpellCard = 0;
    public int equipedSpellCardType = 0; // The type of spell card the player has equipped
    public int equipedRune = 0; // The type of rune the player has equipped

    public PlayerGameData(PlayerGameData data)
    {
        gems = data.gems;
        unlockedSpellCards = data.unlockedSpellCards;
        unlockedRunes = data.unlockedRunes;
        equipedSpellCard = data.equipedSpellCard;
        equipedSpellCardType = data.equipedSpellCardType;
        equipedRune = data.equipedRune;

    }

    public PlayerGameData(int gems, bool[] unlockedSpellCards, bool[] unlockedRunes, EnumSpellCards equipedSpellCard, EnumSpellCardTypes equipedSpellCardType, EnumRunes equipedRune)
    {
        this.gems = gems;
        this.unlockedSpellCards = unlockedSpellCards;
        this.unlockedRunes = unlockedRunes;
        this.equipedSpellCard = (int)equipedSpellCard;
        this.equipedSpellCardType = (int)equipedSpellCardType;
        this.equipedRune = (int)equipedRune;


    }

    public PlayerGameData()
    {
        // Initialize with default values
        gems = 0;
        unlockedSpellCards = new bool[EnumSpellCards.GetValues(typeof(EnumSpellCards)).Length - 1];
        for (int i = 0; i < unlockedSpellCards.Length; i++)
        {
            unlockedSpellCards[i] = false; // All spell cards are initially locked
        }
        unlockedRunes = new bool[EnumRunes.GetValues(typeof(EnumRunes)).Length - 1];
        for (int i = 0; i < unlockedRunes.Length; i++)
        {
            unlockedRunes[i] = false; // All runes are initially locked
        }
        equipedSpellCard = 0; // Default equipped spell card index
        equipedSpellCardType = 0; // Default equipped spell card type index
        equipedRune = 0; // Default equipped rune index
    }

}