using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvokeLightning : ScriptableObject, IEffect
{

    [SerializeField]
    GameObject convokeLightningPrefab;
    EnumSpellCards spellCard = EnumSpellCards.ConvokeLightning;
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;
    private float baseDamage = 10.0f; // Base damage of the effect
    private float augmentedDamage = 0.0f; // Incremented damage for the effect

    private string description = "";


    private void Awake()
    {
        // Obtains the prefab from the EffectManager
        convokeLightningPrefab = EffectManager.Instance?.convokeLightningPrefab;
        convokeLightningPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("ConvokeLightning prefab: " + convokeLightningPrefab);
        // Verifies if the prefab is assigned

        if (convokeLightningPrefab == null)
        {
            Debug.LogError("⚡ ConvokeLightning Prefab no asignado en EffectManager.");
        }
    }

    public Sprite getIcon()
    {
        // returns the icon of the spell card
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
        augmentedDamage += 10.0f; // Increments the damage by 10
        Debug.Log("ConvokeLightning effect upgraded!");
    }

    public string getDescription()
    {

        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a lightning strike over the target, appling impact efects to the enemy.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a lightning strike over the target, appling impact efects to the enemy.";
        }
        else
        {
            description = "Effect: When dashing, summons a lightning strike over all enemies in a small area around the player, damaging all near enemies and applying impact effects.";
        }

        description += "\nDamage: " + (baseDamage + augmentedDamage); // Adds the damage to the description
        description += "\nDamage Type: " + EnumDamageTypes.Lightning.ToString(); // Adds the damage type to the description

        // Returns the description of the effect
        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {

        // Instantiate the lightning strike effect at the target's position
        Vector3 position = target.transform.position;
        position.y = 0.2f; // Ajusts the Y position
        GameObject convokeLightningInstance = Instantiate(convokeLightningPrefab, position, Quaternion.identity);

        convokeLightningInstance.GetComponent<Player_Hitbox>().AugmentDamage(augmentedDamage); // Augment the damage of the effect
        convokeLightningInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Sets the spell card type for the hitbox
        // Sets the inventory index for the hitbox
        convokeLightningInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destroys the effect after a short delay
        Destroy(convokeLightningInstance, 0.3f);
    }

    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}
