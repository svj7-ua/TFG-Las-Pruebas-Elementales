using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingArea : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject healingAreaPrefab;

    EnumSpellCards spellCard = EnumSpellCards.HealingArea; // Type of spell card
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Description of the effect
    [SerializeField] float durationOfEffect = 5f; // Duration of the healing area effect

    private void Awake()
    {
        // Obtains the prefab from the EffectManager
        healingAreaPrefab = EffectManager.Instance?.healingAreaPrefab;
        Debug.Log("HealingArea prefab: " + healingAreaPrefab);
        // Verifies if the prefab is assigned

        if (healingAreaPrefab == null)
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

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a healing area under the target which heals the player over time while standing over it.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a healing area under the target which heals the player over time while standing over it.";

        }
        else
        {
            description = "Effect: When dashing, summons a healing area under nearby enemies, which heal the player over time while standing over it.";
        }

        description += "\nDuration: " + durationOfEffect + " seconds"; // Adds the duration of the effect to the description
        description += "\nTotal Healing: " + healingAreaPrefab.GetComponent<HealingArea_Effect>().healingAmount * durationOfEffect + " HP"; // Adds the total healing amount to the description

        return description;
    }

    public void UpgradeEffect()
    {
        // Auments the duration of the healing area effect by 1 second
        durationOfEffect += 1f; // Auments the duration by 1 second
        Debug.Log("HealingArea effect upgraded!");
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instanciar el prefab en la posición del objetivo
        Vector3 spawnPosition = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z); // Adjusts the Y position
        GameObject healingAreaInstance = Instantiate(healingAreaPrefab, spawnPosition, Quaternion.identity);

        // Destroys the instance after the duration of the effect
        Destroy(healingAreaInstance, durationOfEffect);
    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}

