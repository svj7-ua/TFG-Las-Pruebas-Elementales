using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject FireballExplosionPrefab; // Prefab for fireball explosion effect

    EnumSpellCards spellCard = EnumSpellCards.FireExplosion; // Type of spell card
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Description of the effect
    private float baseDamage = 20.0f; // Base damage of the effect
    float scaleAugment = 0.0f; // Variable to augment the scale of the prefab
    private void Awake()
    {
        // Get the prefab from the EffectManager
        FireballExplosionPrefab = EffectManager.Instance?.fireballExplosionPrefab;
        FireballExplosionPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("Fireball prefab: " + FireballExplosionPrefab);
        // Verifies if the prefab is assigned

        if (FireballExplosionPrefab == null)
        {
            Debug.LogError("⚡ ConvokeLightning Prefab no asignado en EffectManager.");
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
        // Auments its area by adding 0.5 to the scale of the prefab
        scaleAugment += 0.5f;
        Debug.Log("FireExplosion effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a fire explosion over the target, applying impact effects to all affected enemies and setting them on fire.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a fire explosion over the target, applying impact effects to all affected enemies and setting them on fire.";
        }
        else
        {
            description = "Effect: When dashing, summons a fire explosion over all enemies in a small area around the player, applying impact effects to all affected enemies and setting them on fire.";
        }

        description += "Doesn't apply secondary effects.";

        description += "\nArea radius: " + (scaleAugment + 1) + " m"; // Add the area radius to the description
        description += "\nDamage: " + baseDamage; // Add the damage to the description
        description += "\nDamage Type: " + EnumDamageTypes.Fire.ToString(); // Add the damage type to the description

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {

        // Instantiate the fire explosion effect at the target's position
        Vector3 position = target.transform.position;
        position.y = 1.5f; // Adjusts the Y position
        GameObject FireballExplosionInstance = Instantiate(FireballExplosionPrefab, position, Quaternion.identity);

        Vector3 currentScale = FireballExplosionInstance.transform.localScale;
        FireballExplosionInstance.transform.localScale = new Vector3(currentScale.x + scaleAugment, currentScale.y + scaleAugment, currentScale.z + scaleAugment); // auments the scale of the prefab
        FireballExplosionInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Adds the spell card type for the hitbox
        // Sets the inventory index for the hitbox
        FireballExplosionInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destroys the effect after a short delay
        Destroy(FireballExplosionInstance, 0.3f);
    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}
