using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeteorRain : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject meteorRainPrefab; // Prefab del efecto de lluvia de meteoros

    EnumSpellCards spellCard = EnumSpellCards.MeteorRain; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee; // Tipo de carta de hechizo

    private string description = ""; // Descripción del efecto
    [SerializeField] float durationOfEffect = 1.5f; // Duración del efecto de lluvia de meteoros
    [SerializeField] float addedChance = 0.1f;


    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        meteorRainPrefab = EffectManager.Instance?.meteorRainPrefab;
        Debug.Log("MeteorRain prefab: " + meteorRainPrefab);
        // Verificar si el prefab está asignado

        if (meteorRainPrefab == null)
        {
            Debug.LogError("⚡ MeteorRain Prefab no asignado en EffectManager.");
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
        // Aumentar su área añadiendo 1 a la escala del prefab
        durationOfEffect += 0.5f; // Aumentar la duración del efecto
        addedChance += 0.1f; // Aumentar la probabilidad de invocación del efecto
        Debug.Log("HealingArea effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a meteor rain under the target ";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a meteor rain under the target ";

        }
        else
        {
            description = "Effect: When dashing, summons a meteor rain under nearby enemies, ";
        }

        description += "which deals damage when an enemy is struck by a meteor.";

        description += "\nDuration: " + durationOfEffect + " seconds"; // Añadir la duración al final de la descripción
        description += "\nDamage: " + 10; // Añadir la cantidad de daño al final de la descripción
        description += "\nDamage Type: " + EnumDamageTypes.Arcane.ToString(); // Añadir el tipo de daño al final de la descripción
        description += "\nChance to apply effects: " + ((meteorRainPrefab.GetComponentInChildren<MeteorRainEffect>().GetChanceToApplyEffects()+addedChance)*100) + "%"; // Añadir la probabilidad de invocación del efecto

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Aplicar el efecto de lluvia de meteoros al objetivo
        if (meteorRainPrefab != null)
        {

            // Instanciar el prefab en la posición del objetivo

            GameObject effectInstance = Instantiate(meteorRainPrefab, new Vector3(target.transform.position.x, 5, target.transform.position.z), Quaternion.identity);
            effectInstance.GetComponentInChildren<MeteorRainEffect>().SetSpellCardType(spellCardType); // Set the spell card type to apply the effects from the active effects inventory
            effectInstance.GetComponentInChildren<MeteorRainEffect>().AugmentChance(addedChance); // Set the duration of the effect
            effectInstance.GetComponentInChildren<MeteorRainEffect>().SetInventoryIndex(index + 1); // Configurar el índice del inventario
            Destroy(effectInstance, durationOfEffect); // Destruir la instancia después de la duración del efecto
        }
        else
        {
            Debug.LogError("⚡ MeteorRain prefab is not assigned in EffectManager.");
        }
    }


}