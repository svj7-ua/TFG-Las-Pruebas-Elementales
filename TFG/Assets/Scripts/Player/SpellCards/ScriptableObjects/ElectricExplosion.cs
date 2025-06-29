using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricExplosion : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject ElectricExplosionPrefab;

    EnumSpellCards spellCard = EnumSpellCards.ElectricExplosion; // Spell card
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Description of the effect
    private float baseDamage = 20.0f; // Base damage of the effect
    float scaleAugment = 0.0f;

    private void Awake()
    {
        // Obtains the prefab from the EffectManager
        ElectricExplosionPrefab = EffectManager.Instance?.electricExplosionPrefab;
        ElectricExplosionPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("ElectricExplosion prefab: " + ElectricExplosionPrefab);
        // Verifies if the prefab is assigned

        if (ElectricExplosionPrefab == null)
        {
            Debug.LogError("⚡ ElectricExplosion Prefab no asignado en EffectManager.");
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
        // Auments its area adding 1 to the scale of the prefab
        scaleAugment += 0.5f;
        Debug.Log("ElectricExplosion effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons an electric explosion over the target, applying impact effects to all affected enemies.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons an electric explosion over the target, applying impact effects to all affected enemies.";
        }
        else
        {
            description = "Effect: When dashing, summons an electric explosion over all enemies in a small area around the player, damaging all near enemies and applying impact effects.";
        }

        description += "Doesn't apply secondary effects.";

        description += "\n\nArea radius: " + (1 + scaleAugment) + "m"; // Adding the area radius to the description
        description += "\nDamage: " + baseDamage; // Adding the damage to the description
        description += "\nDamage Type: " + EnumDamageTypes.Lightning.ToString(); // Adding the damage type to the description

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instantiate the electric explosion effect at the target's position
        Vector3 position = target.transform.position;
        position.y = 1.5f; // adjusts the Y position
        GameObject LightningExplosionInstance = Instantiate(ElectricExplosionPrefab, position, Quaternion.identity);

        Vector3 currentScale = LightningExplosionInstance.transform.localScale;
        LightningExplosionInstance.transform.localScale = new Vector3(currentScale.x + scaleAugment, currentScale.y + scaleAugment, currentScale.z + scaleAugment);
        LightningExplosionInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Adds the spell card type for the hitbox

        // Sets the inventory index for the hitbox
        LightningExplosionInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destroys the effect after a short delay
        Destroy(LightningExplosionInstance, 0.3f);
    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}
