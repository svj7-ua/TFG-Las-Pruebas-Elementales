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

    private float augmentedDamage = 0.0f; // Daño aumentado por el efecto

    private string description = "";


    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        convokeLightningPrefab = EffectManager.Instance?.convokeLightningPrefab;
        convokeLightningPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("ConvokeLightning prefab: " + convokeLightningPrefab);
        // Verificar si el prefab está asignado

        if (convokeLightningPrefab == null)
        {
            Debug.LogError("⚡ ConvokeLightning Prefab no asignado en EffectManager.");
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
        // Aquí puedes implementar la lógica para mejorar el efecto si es necesario
        augmentedDamage += 5.0f; // Aumentar el daño del efecto
        Debug.Log("ConvokeLightning effect upgraded!");
    }

    public string getDescription()
    {

        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemie with a melee attack, summons a lightning strike over the target, appling impact efects to the enemie.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemie with a ranged attack, summons a lightning strike over the target, appling impact efects to the enemie.";
        }
        else
        {
            description = "Effect: When dashing, summons a lightning strike over all enemies in a small area around the player, damaging all near enemies and applying impact effects.";
        }

        description += "\nDamage: " + (10+augmentedDamage); // Agregar el daño aumentado a la descripción
        description +=  "\nDamage Type: " + EnumDamageTypes.Lightning.ToString(); // Agregar el tipo de daño a la descripción

        // Devuelve la descripción del efecto
        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {

        // Instanciar el prefab en la posición del objetivo
        GameObject convokeLightningInstance = Instantiate(convokeLightningPrefab, target.transform.position, Quaternion.identity);

        convokeLightningInstance.GetComponent<Player_Hitbox>().AugmentDamage(augmentedDamage); // Aumentar el daño del hitbox
        convokeLightningInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Configurar el tipo de carta de hechizo
        // Configurar el índice del inventario
        convokeLightningInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destruir la instancia después de 1.5 segundos
        Destroy(convokeLightningInstance, 0.3f);
    }

}
