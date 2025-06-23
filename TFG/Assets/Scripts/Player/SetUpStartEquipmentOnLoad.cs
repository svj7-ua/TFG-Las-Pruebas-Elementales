using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpStartEquipmentOnLoad : MonoBehaviour
{

    [SerializeField]
    private GameObject RuneShop; // Reference to the player object
    // Start is called before the first frame update
    void Start()
    {

        if (GameData.equipedSpellCard != EnumSpellCards.None && GameData.equipedSpellCardType != EnumSpellCardTypes.None)
        {
            GameData.spellCard = SpellCard_Factory.CreateSpellCard(GameData.equipedSpellCard, GameData.equipedSpellCardType);
        }
        
        
        if(GameData.equipedRune != EnumRunes.None)
        {
            if (RuneShop != null)
            {
                GameData.rune = RuneShop.GetComponent<RunesShop>().GetRuneItemByRune(GameData.equipedRune);
            }
        }

    }

}
