using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeteorRain : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject meteorRainPrefab; // Prefab

    EnumSpellCards spellCard = EnumSpellCards.MeteorRain; // Spellcard type
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee; // Type of spell card
    private float baseDamage = 10.0f; // Base damage of the effect
    private string description = ""; // Description of the effect
    [SerializeField] float durationOfEffect = 1.5f; // Duration of the meteor rain effect
    [SerializeField] float addedChance = 0.1f;


    private void Awake()
    {
        // Obtains the prefab from the EffectManager
        meteorRainPrefab = EffectManager.Instance?.meteorRainPrefab;
        Debug.Log("MeteorRain prefab: " + meteorRainPrefab);
        // Verifies if the prefab is assigned

        if (meteorRainPrefab == null)
        {
            Debug.LogError("⚡ MeteorRain Prefab no asignado en EffectManager.");
        }
    }

    public Sprite getIcon()
    {
        // Returns the icon of the spell card
        return SpellCard_Factory.LoadSpellCardIcon(spellCard, spellCardType);
    }

    public void SetSpellCardType(EnumSpellCardTypes type)
    {
        spellCardType = type;
    }
    public EnumSpellCards getSpellCard()
    {
        return spellCard;
    }

    public void UpgradeEffect()
    {
        // Auments the duration of the meteor rain effect by 0.5 seconds
        durationOfEffect += 0.5f;
        addedChance += 0.1f; // Increments the chance to apply effects by 10%
        Debug.Log("MeteorRain effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a meteor rain under the target ";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a meteor rain under the target ";

        }
        else
        {
            description = "Effect: When dashing, summons a meteor rain under nearby enemies, ";
        }

        description += "which deals damage when an enemy is struck by a meteor.";

        description += "\nDuration: " + durationOfEffect + " seconds"; // Adds the duration of the effect to the description
        description += "\nDamage: " + baseDamage; // Add the damage to the description
        description += "\nDamage Type: " + EnumDamageTypes.Arcane.ToString(); // Add the damage type to the description
        description += "\nChance to apply effects: " + ((meteorRainPrefab.GetComponentInChildren<MeteorRainEffect>().GetChanceToApplyEffects() + addedChance) * 100) + "%"; // Adds the chance to apply effects to the description

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {

        // Instantiate the meteor rain effect at the target's position

        GameObject effectInstance = Instantiate(meteorRainPrefab, new Vector3(target.transform.position.x, 8, target.transform.position.z), Quaternion.identity);
        effectInstance.GetComponentInChildren<MeteorRainEffect>().SetSpellCardType(spellCardType); // Set the spell card type to apply the effects from the active effects inventory
        effectInstance.GetComponentInChildren<MeteorRainEffect>().AugmentChance(addedChance); // Set the duration of the effect
        effectInstance.GetComponentInChildren<MeteorRainEffect>().SetInventoryIndex(index + 1); // Sets the inventory index for the hitbox
        Destroy(effectInstance, durationOfEffect); // Destroys the instance after the duration of the effect

    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }


}