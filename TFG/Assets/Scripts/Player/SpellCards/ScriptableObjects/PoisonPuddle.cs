using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddle : ScriptableObject, IEffect
{

    [SerializeField]
    GameObject poisonPuddlePrefab; // Prefab

    EnumSpellCards spellCard = EnumSpellCards.PoisonPuddle; // Spellcard
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;
    private float poisonAugmentedDuration = 0.0f;
    private float durationOfEffect = 1.5f; // Duration of the poison puddle effect

    private string description = ""; // Description of the effect

    private void Awake()
    {
        // Obtain the prefab from the EffectManager
        poisonPuddlePrefab = EffectManager.Instance?.poisonPuddlePrefab;
        poisonPuddlePrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Assign the spell card type to the prefab
        Debug.Log("PoisonPuddle prefab: " + poisonPuddlePrefab);
        // Verify if the prefab is assigned

        if (poisonPuddlePrefab == null)
        {
            Debug.LogError("⚡ HealingArea Prefab no asignado en EffectManager.");
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

        durationOfEffect += 0.5f; // Aument the duration of the effect
        poisonAugmentedDuration += 0.5f; // Aument the duration of the poison effect
        Debug.Log("PoisonPuddle effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a poison puddle under the target, poisoning the target and applying impact effects.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a poison puddle under the target, poisoning the target and applying impact effects.";
        }
        else
        {
            description = "Effect: When dashing, summons a poison puddle under all enemies in a small area around the player, poisoning them and applying impact effects.";
        }

        float totalDuration = poisonPuddlePrefab.GetComponent<Player_Hitbox>().GetSecondaryEffectDuration() + poisonAugmentedDuration;

        description += "\nPuddle Duration: " + durationOfEffect + " seconds"; // Add the duration of the effect to the description
        description += "\nSecondary Effect Duration: " + totalDuration + " seconds"; // Add the total poisoned duration to the description
        description += "\nDamage Type: " + EnumDamageTypes.Poison.ToString(); // Add the damage type to the description

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instantiate the poison puddle prefab at the target's position
        Vector3 spawnPosition = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z); // Adjusts the Y position
        GameObject poisonPuddleInstance = Instantiate(poisonPuddlePrefab, spawnPosition, Quaternion.identity);

        poisonPuddleInstance.GetComponent<Player_Hitbox>().AumentSecondaryEffectDuration(poisonAugmentedDuration); // Aument the duration of the poison effect
        poisonPuddleInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Assign the spell card type to the prefab

        // Sets the inventory index for the hitbox
        poisonPuddleInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destroy the instance after the duration of the effect
        Destroy(poisonPuddleInstance, durationOfEffect);
    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}
