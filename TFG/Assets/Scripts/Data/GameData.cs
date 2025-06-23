using System;
using UnityEngine;


// This class will be used to store the game data that needs to be accessed globally
// It will be used to store the player's gems, the type of spell card selected, and the type of rune selected
//  This way, the starting equipment can be set to the player when they start the game.
public static class GameData
{
    public static int gems = 0; // The amount of gems the player has

    public static bool[] unlockedSpellCards = new bool[EnumSpellCards.GetValues(typeof(EnumSpellCards)).Length - 1]; // The spell cards the player has unlocked
    public static bool[] unlockedRunes = new bool[EnumRunes.GetValues(typeof(EnumRunes)).Length - 1]; // The runes the player has unlocked

    public static EnumSpellCards equipedSpellCard = EnumSpellCards.None; // The spell card that is currently equipped by the player
    public static EnumSpellCardTypes equipedSpellCardType = EnumSpellCardTypes.None; // The type of spell card that is currently equipped by the player

    public static EnumRunes equipedRune = EnumRunes.None; // The type of rune the player has selected

    // Settings
    public static float volume = 1.0f; // The volume of the game, default is 1.0 (100%)
    public static int resolutionIndex = 0; // The index of the resolution selected by the player

    public static int displayIndex = 0; // The index of the display selected by the player

    public static void LoadPlayerData(PlayerGameData data)
    {
        gems = data.gems;
        equipedRune = (EnumRunes)data.equipedRune;

        // Load unlocked spell cards
        for (int i = 0; i < data.unlockedSpellCards.Length; i++)
        {
            unlockedSpellCards[i] = data.unlockedSpellCards[i];
        }

        // Load unlocked runes
        for (int i = 0; i < data.unlockedRunes.Length; i++)
        {
            unlockedRunes[i] = data.unlockedRunes[i];
        }

        equipedSpellCard = (EnumSpellCards)data.equipedSpellCard;
        equipedSpellCardType = (EnumSpellCardTypes)data.equipedSpellCardType;

        equipedRune = (EnumRunes)data.equipedRune;

        // Load Settings
        volume = data.volume; // Load the volume from the saved data
        resolutionIndex = data.resolutionIndex; // Load the resolution index from the saved data
        displayIndex = data.displayIndex; // Load the display index from the saved data

    }

    public static int unlockeableRunesAmount = 27; // The amount of runes that can be unlocked

    public static IItem rune = null; // The rune that is currently equipped by the player
    public static IEffect spellCard = null; // The spell card that is currently equipped by the player

}