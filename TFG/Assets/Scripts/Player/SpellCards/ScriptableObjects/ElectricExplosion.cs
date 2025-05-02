using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricExplosion : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject ElectricExplosionPrefab;

    EnumSpellCards spellCard = EnumSpellCards.ElectricExplosion; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;
    
    private string description = ""; // Descripción del efecto

    float scaleAugment = 0.0f;

    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        ElectricExplosionPrefab = EffectManager.Instance?.electricExplosionPrefab;
        ElectricExplosionPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("ElectricExplosion prefab: " + ElectricExplosionPrefab);
        // Verificar si el prefab está asignado

        if (ElectricExplosionPrefab == null)
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
        // Auments its area adding 1 to the scale of the prefab
        scaleAugment += 0.5f;
        Debug.Log("ElectricExplosion effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description =  "Effect: When hitting an enemy with a melee attack, summons an electric explosion over the target, applying impact effects to all affected enemies.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description =  "Effect: When hitting an enemy with a ranged attack, summons an electric explosion over the target, applying impact effects to all affected enemies.";
        }
        else
        {
            description =  "Effect: When dashing, summons an electric explosion over all enemies in a small area around the player, damaging all near enemies and applying impact effects.";
        }

        description += "\n\nArea radius: " + (1+scaleAugment) + "m"; // Agregar el área al final de la descripción
        description += "\nDamage: " + 10; // Agregar el daño aumentado a la descripción
        description += "\nDamage Type: " + EnumDamageTypes.Lightning.ToString(); // Agregar el daño aumentado a la descripción

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instanciar el prefab en la posición del objetivo
        GameObject LightningExplosionInstance = Instantiate(ElectricExplosionPrefab, target.transform.position, Quaternion.identity);

        Vector3 currentScale = LightningExplosionInstance.transform.localScale;
        LightningExplosionInstance.transform.localScale = new Vector3(currentScale.x + scaleAugment, currentScale.y + scaleAugment, currentScale.z + scaleAugment);
        LightningExplosionInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Configurar el tipo de carta de hechizo

        // Configurar el índice del inventario
        LightningExplosionInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destruir la instancia después de 1.5 segundos
        Destroy(LightningExplosionInstance, 0.3f);
    }

}
