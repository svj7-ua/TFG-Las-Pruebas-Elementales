using System;
using UnityEngine;


// This class will be used to store the game data that needs to be accessed globally
// It will be used to store the player's gems, the type of spell card selected, and the type of rune selected
//  This way, the starting equipment can be set to the player when they start the game.
public static class GameData
{
    public static int gems = 0; // The amount of gems the player has
    public static EnumSpellCards spellCardType = EnumSpellCards.None; // The type of spell card the player has selected
    public static EnumSpellCardTypes spellCardTypeEnum = EnumSpellCardTypes.None; // The type of spell card the player has selected
    public static EnumRunes rune = EnumRunes.None; // The type of rune the player has selected

    public static bool[] unlockedSpellCards = new bool[EnumSpellCards.GetValues(typeof(EnumSpellCards)).Length - 1]; // The spell cards the player has unlocked
    public static bool[] unlockedRunes = new bool[EnumRunes.GetValues(typeof(EnumRunes)).Length - 1]; // The runes the player has unlocked
    public static void LoadPlayerData(PlayerGameData data)
    {
        gems = data.gems;
        spellCardType = (EnumSpellCards)data.equipedSpellCard;
        spellCardTypeEnum = (EnumSpellCardTypes)data.equipedSpellCardType;
        rune = (EnumRunes)data.equipedRune;

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

    }

}