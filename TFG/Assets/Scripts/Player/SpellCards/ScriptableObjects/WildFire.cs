using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFire : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject wildFirePrefab;

    EnumSpellCards spellCard = EnumSpellCards.WildFire; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private string description = ""; // Descripción del efecto

    private float durationOfEffect = 1.5f; // Duración del efecto de veneno
    private float fireAugmentedTime = 0.0f; // Aumento de daño del efecto de veneno
    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        wildFirePrefab = EffectManager.Instance?.wildFirePrefab;
        wildFirePrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("WildFire prefab: " + wildFirePrefab);
        // Verificar si el prefab está asignado

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
        fireAugmentedTime += 0.4f; // Aumentar la duración del efecto
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
        description += "\nBurning duration: " + wildFirePrefab.GetComponent<Player_Hitbox>().GetSecondaryEffectDuration() + fireAugmentedTime + " seconds";
        description += "\nDamage Type: " + EnumDamageTypes.Fire.ToString(); // Agregar el tipo de daño al final de la descripción
        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instanciar el prefab en la posición del objetivo
        Vector3 spawnPosition = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z); // Ajustar la posición para que esté un poco arriba del objetivo
        GameObject wildFireInstance = Instantiate(wildFirePrefab, spawnPosition, Quaternion.identity);
        wildFireInstance.GetComponent<Player_Hitbox>().AumentSecondaryEffectDuration(fireAugmentedTime); // Aumentar la duración del efecto de fuego
        wildFireInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Asignar el tipo de carta al prefab

        // Configurar el índice del inventario
        wildFireInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destruir la instancia después de 1.5 segundos
        Destroy(wildFireInstance, durationOfEffect);
    }

    public Sprite getIcon()
    {
        // Devuelve el icono del efecto
        return SpellCard_Factory.LoadSpellCardIcon(spellCard, spellCardType);
    }

    public EnumSpellCardTypes getSpellCardType()
    {
        return spellCardType;
    }

}
