using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindShield : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject WindShieldPrefab; // WindShield prefab to be instantiated

    EnumSpellCards spellCard = EnumSpellCards.WindShield; // Type of spell card
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = "";
    private float windShieldAugmentedDuration = 0.0f; // Augmented duration for the wind shield effect
    private void Awake()
    {
        // Load the WindShield prefab from the EffectManager
        WindShieldPrefab = EffectManager.Instance?.windShieldPrefab;
        WindShieldPrefab.GetComponentInChildren<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("WindshieldPrefab prefab: " + WindShieldPrefab);
        // Check if the WindShieldPrefab is assigned
        if (WindShieldPrefab == null)
        {
            Debug.LogError("⚡ ElectricExplosion Prefab no asignado en EffectManager.");
        }

    }

    public Sprite getIcon()
    {
        // Return the icon for the spell card
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
        windShieldAugmentedDuration += 0.2f; // Aument the duration of the wind shield effect by 0.2 seconds
        Debug.Log("WindShield effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a wind shield under the target, applying impact effects to all affected enemies.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a wind shield under the target, applying impact effects to all affected enemies.";
        }
        else
        {
            description = "Effect: When dashing, summons a wind shield trapping all nearby enemies in a small area, applying impact effects to all affected enemies.";
        }
        description += " Trapped enemies are immobilized while inside the wind shield.";

        description += "\nDuration: " + WindShieldPrefab.GetComponent<WindSheldEffect>().getShieldDuration() + windShieldAugmentedDuration + " seconds"; // Add the duration of the wind shield effect
        description += "\nDamage: " + 10;
        description += "\nDamage Type: " + EnumDamageTypes.Wind.ToString(); // Add the damage type of the wind shield effect

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instantiate the WindShield prefab at the target's position
        Vector3 position = target.transform.position;
        position.y = 1.5f;
        GameObject WindShieldInstance = Instantiate(WindShieldPrefab, position, Quaternion.identity); // Create the WindShield instance
        WindShieldInstance.GetComponentInChildren<Player_Hitbox>().SetSpellCardType(spellCardType); // Assign the spell card type to the hitbox
        WindShieldInstance.GetComponent<WindSheldEffect>().AugmentDuration(windShieldAugmentedDuration); // Augment the duration of the wind shield effect

        // Tells the WindShield instance to apply the next effect in the inventory
        WindShieldInstance.GetComponentInChildren<Player_Hitbox>().SetInventoryIndex(index + 1);

    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }
}
