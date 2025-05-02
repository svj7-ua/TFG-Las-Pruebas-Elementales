using UnityEngine;
public class SpellCard_Factory
{

    public static IEffect CreateSpellCard(EnumSpellCards spellCard, EnumSpellCardTypes spellCardType)
    {

        IEffect spellCardInstance;

        switch (spellCard)
        {
            case EnumSpellCards.ConvokeLightning:
                spellCardInstance = ScriptableObject.CreateInstance<ConvokeLightning>();
                break;
            case EnumSpellCards.FireExplosion:
                spellCardInstance = ScriptableObject.CreateInstance<FireExplosion>();
                break;
            case EnumSpellCards.PoisonPuddle:
                spellCardInstance = ScriptableObject.CreateInstance<PoisonPuddle>();
                break;
            case EnumSpellCards.ElectricExplosion:
                spellCardInstance = ScriptableObject.CreateInstance<ElectricExplosion>();
                break;
            case EnumSpellCards.WildFire:
                spellCardInstance = ScriptableObject.CreateInstance<WildFire>();
                break;
            case EnumSpellCards.HealingArea:
                spellCardInstance = ScriptableObject.CreateInstance<HealingArea>();
                break;
            case EnumSpellCards.WindShield:
                spellCardInstance = ScriptableObject.CreateInstance<WindShield>();
                break;
/*             case EnumSpellCards.Tornado:
                spellCardInstance = ScriptableObject.CreateInstance<Tornado>();
                break; */
            case EnumSpellCards.MeteorRain:
                spellCardInstance = ScriptableObject.CreateInstance<MeteorRain>();
                break;
            default:
                throw new System.ArgumentException("Invalid spell card type: " + spellCard);
        }

        spellCardInstance.SetSpellCardType(spellCardType); // Set the type of the spell card
        return spellCardInstance;
    }

    public static Sprite LoadSpellCardIcon(EnumSpellCards spellCard, EnumSpellCardTypes spellcardType)
    {
        string path = "2D/SpellCards/" + spellCard.ToString()+"_"+spellcardType.ToString()+"Icon";
        Sprite icon = Resources.Load<Sprite>(path);
        if (icon == null)
        {
            Debug.LogWarning("Spell card icon not found at path: " + path); // Log a warning if the icon is not found
            return Resources.Load<Sprite>("2D/SpellCards/null"); // Return a default icon if the specific one is not found
        }
        return icon;
    }

}
