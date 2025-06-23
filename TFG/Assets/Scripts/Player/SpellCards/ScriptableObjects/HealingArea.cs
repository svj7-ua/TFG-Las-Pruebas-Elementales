using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingArea : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject healingAreaPrefab;

    EnumSpellCards spellCard = EnumSpellCards.HealingArea; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Descripción del efecto
    [SerializeField] float durationOfEffect = 5f; // Duración del efecto de curación

    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        healingAreaPrefab = EffectManager.Instance?.healingAreaPrefab;
        //healingAreaPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);      //Should not be necessary
        Debug.Log("HealingArea prefab: " + healingAreaPrefab);
        // Verificar si el prefab está asignado

        if (healingAreaPrefab == null)
        {
            Debug.LogError("⚡ HealingArea Prefab no asignado en EffectManager.");
        }
    }

    public Sprite getIcon()
    {
        // Devuelve el icono del efecto
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

        description += "\nDuration: " + durationOfEffect + " seconds"; // Añadir la duración al final de la descripción
        description += "\nTotal Healing: " + healingAreaPrefab.GetComponent<HealingArea_Effect>().healingAmount * durationOfEffect + " HP"; // Añadir la cantidad de curación al final de la descripción

        return description;
    }

    public void UpgradeEffect()
    {
        // Aumentar su área añadiendo 1 a la escala del prefab
        durationOfEffect += 1f; // Aumentar la duración del efecto
        Debug.Log("HealingArea effect upgraded!");
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instanciar el prefab en la posición del objetivo
        Vector3 spawnPosition = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z); // Ajustar la posición para que esté un poco arriba del objetivo
        GameObject healingAreaInstance = Instantiate(healingAreaPrefab, spawnPosition, Quaternion.identity);

        // Destruir la instancia después de la duración del efecto
        Destroy(healingAreaInstance, durationOfEffect);
    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}

