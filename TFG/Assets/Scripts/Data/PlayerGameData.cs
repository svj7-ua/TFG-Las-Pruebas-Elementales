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

    // Settings
    public float volume = 1.0f; // The volume of the game, default is 1.0 (100%)
    public int resolutionIndex = 0; // The index of the resolution selected by the player
    public int displayIndex = 0; // The index of the display selected by the player

    public PlayerGameData(PlayerGameData data)
    {
        gems = data.gems;
        unlockedSpellCards = data.unlockedSpellCards;
        unlockedRunes = data.unlockedRunes;
        equipedSpellCard = data.equipedSpellCard;
        equipedSpellCardType = data.equipedSpellCardType;
        equipedRune = data.equipedRune;
        volume = data.volume; // Copy the volume from the existing data
        resolutionIndex = data.resolutionIndex; // Copy the resolution index from the existing data
        displayIndex = data.displayIndex; // Copy the display index from the existing data

    }

    public PlayerGameData(int gems, bool[] unlockedSpellCards, bool[] unlockedRunes, EnumSpellCards equipedSpellCard, EnumSpellCardTypes equipedSpellCardType, EnumRunes equipedRune, float volume, int resolutionIndex, int displayIndex)
    {
        this.gems = gems;
        this.unlockedSpellCards = unlockedSpellCards;
        this.unlockedRunes = unlockedRunes;
        this.equipedSpellCard = (int)equipedSpellCard;
        this.equipedSpellCardType = (int)equipedSpellCardType;
        this.equipedRune = (int)equipedRune;
        this.volume = volume; // Initialize volume from GameData
        this.resolutionIndex = resolutionIndex; // Initialize resolution index from GameData
        this.displayIndex = displayIndex; // Initialize display index from GameData


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
        volume = 1.0f; // Default volume level
        resolutionIndex = -1; // Default resolution index
        displayIndex = 0; // Default display index
    }

}