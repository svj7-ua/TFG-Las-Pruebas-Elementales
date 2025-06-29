using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFire : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject wildFirePrefab;

    EnumSpellCards spellCard = EnumSpellCards.WildFire; // Spellcard
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Description

    private float durationOfEffect = 1.5f; // Duration of the wild fire effect
    private float fireAugmentedTime = 0.0f; // Aumented time for the inginted effect
    private void Awake()
    {
        // Obtain the prefab from the EffectManager
        wildFirePrefab = EffectManager.Instance?.wildFirePrefab;
        wildFirePrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("WildFire prefab: " + wildFirePrefab);
        // Verify if the prefab is assigned

         // If the prefab is not assigned, log an error

        if (wildFirePrefab == null)
        {
            Debug.LogError("⚡ WildFire Prefab no asignado en EffectManager.");
        }
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
        // Auments its duration by 0.4 seconds, adding 1 tick of damage
        fireAugmentedTime += 0.4f; // Aument the duration of the fire effect
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a wild fire area under the target, damaging and applying impact effects to all affected enemies and setting them on fire.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a wild fire area under the target, damaging and applying impact effects to all affected enemies and setting them on fire.";
        }
        else
        {
            description = "Effect: When dashing, summons a wild fire area under all enemies in a small area around the player, applying impact effects to all affected enemies.";
        }

        description += "\nDuration: " + durationOfEffect + " seconds";
        description += "\nIgnited duration: " + wildFirePrefab.GetComponent<Player_Hitbox>().GetSecondaryEffectDuration() + fireAugmentedTime + " seconds";
        description += "\nDamage Type: " + EnumDamageTypes.Fire.ToString(); // Add the damage type to the description
        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instantiate the wild fire prefab at the target's position
        Vector3 spawnPosition = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z); // Adjusts the Y position
        GameObject wildFireInstance = Instantiate(wildFirePrefab, spawnPosition, Quaternion.identity);
        wildFireInstance.GetComponent<Player_Hitbox>().AumentSecondaryEffectDuration(fireAugmentedTime); // Aument the duration of the fire effect
        wildFireInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Assign the spell card type to the prefab

        // Sets the inventory index for the hitbox
        wildFireInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destroy the instance after the duration of the effect
        Destroy(wildFireInstance, durationOfEffect);
    }

    public Sprite getIcon()
    {
        // returns the icon of the spell card
        return SpellCard_Factory.LoadSpellCardIcon(spellCard, spellCardType);
    }

    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}
