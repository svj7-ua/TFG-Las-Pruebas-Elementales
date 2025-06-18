using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindShield : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject WindShieldPrefab; // Prefab del escudo de viento

    EnumSpellCards spellCard = EnumSpellCards.WindShield; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Descripción del efecto
    private float windShieldAugmentedDuration = 0.0f; // Aumento de duración del efecto de escudo de viento
    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        WindShieldPrefab = EffectManager.Instance?.windShieldPrefab;
        WindShieldPrefab.GetComponentInChildren<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("WindshieldPrefab prefab: " + WindShieldPrefab);
        // Verificar si el prefab está asignado

        if (WindShieldPrefab == null)
        {
            Debug.LogError("⚡ ElectricExplosion Prefab no asignado en EffectManager.");
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

    public void UpgradeEffect()
    {
        windShieldAugmentedDuration += 0.2f; // Aumentar la duración del efecto
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

        description += "\nDuration: " + WindShieldPrefab.GetComponent<WindSheldEffect>().getShieldDuration() + windShieldAugmentedDuration + " seconds"; // Añadir la duración al final de la descripción
        description += "\nDamage: " + 10;
        description += "\nDamage Type: " + EnumDamageTypes.Wind.ToString(); // Añadir el tipo de daño al final de la descripción

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instanciar el prefab en la posición del objetivo
        Vector3 position = target.transform.position;
        position.y = 1.5f;
        GameObject WindShieldInstance = Instantiate(WindShieldPrefab, position, Quaternion.identity); // Instanciar el prefab en la posición del objetivo
        WindShieldInstance.GetComponentInChildren<Player_Hitbox>().SetSpellCardType(spellCardType); // Asignar el tipo de carta al prefab
        WindShieldInstance.GetComponent<WindSheldEffect>().AugmentDuration(windShieldAugmentedDuration); // Aumentar la duración del efecto

        // Configurar el índice del inventario
        WindShieldInstance.GetComponentInChildren<Player_Hitbox>().SetInventoryIndex(index + 1);

    }
    
    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }
}
