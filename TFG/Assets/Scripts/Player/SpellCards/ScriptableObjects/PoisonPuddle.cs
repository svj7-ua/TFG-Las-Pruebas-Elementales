using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddle : ScriptableObject, IEffect
{

    [SerializeField]
    GameObject poisonPuddlePrefab; // Prefab del área de veneno

    EnumSpellCards spellCard = EnumSpellCards.PoisonPuddle; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;
    private float poisonAugmentedDuration = 0.0f;
    private float durationOfEffect = 1.5f; // Duración del efecto de veneno

    private string description = ""; // Descripción del efecto

    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        poisonPuddlePrefab = EffectManager.Instance?.poisonPuddlePrefab;
        poisonPuddlePrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Asignar el tipo de carta al prefab
        Debug.Log("PoisonPuddle prefab: " + poisonPuddlePrefab);
        // Verificar si el prefab está asignado

        if (poisonPuddlePrefab == null)
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

    public void UpgradeEffect()
    {
        // Aumentar su área añadiendo 1 a la escala del prefab
        durationOfEffect += 0.5f; // Aumentar la duración del efecto
        poisonAugmentedDuration += 0.2f; // Aumentar la duración del efecto
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

        float totalDuration = poisonPuddlePrefab.GetComponent<PoisonArea>().poisonDuration+poisonAugmentedDuration;

        description += "\nDuration: " + durationOfEffect + " seconds"; // Añadir la duración al final de la descripción
        description += "\nTotal Poison Duration: " + totalDuration + " seconds"; // Añadir la duración total del veneno al final de la descripción
        description += "\nDamage Type: " + EnumDamageTypes.Poison.ToString(); // Añadir el tipo de daño al final de la descripción

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {
        // Instanciar el prefab en la posición del objetivo
        Vector3 spawnPosition = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z); // Ajustar la posición para que esté un poco arriba del objetivo
        GameObject poisonPuddleInstance = Instantiate(poisonPuddlePrefab, spawnPosition, Quaternion.identity);

        poisonPuddleInstance.GetComponent<PoisonArea>().AugmentDuration(poisonAugmentedDuration); // Aumentar la duración del efecto de veneno
        poisonPuddleInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Asignar el tipo de carta al prefab

        // Configurar el índice del inventario
        poisonPuddleInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destruir la instancia después de la duración del efecto
        Destroy(poisonPuddleInstance, durationOfEffect);
    }

}
