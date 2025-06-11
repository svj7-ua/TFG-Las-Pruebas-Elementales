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

    float scaleAugment = 0.0f; // Variable to augment the scale of the prefab
    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        FireballExplosionPrefab = EffectManager.Instance?.fireballExplosionPrefab;
        FireballExplosionPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("Fireball prefab: " + FireballExplosionPrefab);
        // Verificar si el prefab está asignado

        if (FireballExplosionPrefab == null)
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
        // Aumentar su área añadiendo 1 a la escala del prefab
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
        } else {
            description = "Effect: When dashing, summons a fire explosion over all enemies in a small area around the player, applying impact effects to all affected enemies and setting them on fire.";
        }

        description +="\nArea radius: " + (scaleAugment + 1) + " m"; // Add the area radius to the description
        description += "\nDamage: " + 10; // Add the damage to the description
        description += "\nDamage Type: " + EnumDamageTypes.Fire.ToString(); // Add the damage type to the description

        return description;
    }

    public void ApplyEffect(GameObject target, int index = 0)
    {

        // Instanciar el prefab en la posición del objetivo
        Vector3 position = target.transform.position;
        position.y = 1.5f; // Asegurarse de que la posición Y sea la misma que la del prefab
        GameObject FireballExplosionInstance = Instantiate(FireballExplosionPrefab, position, Quaternion.identity);

        Vector3 currentScale = FireballExplosionInstance.transform.localScale;
        FireballExplosionInstance.transform.localScale = new Vector3(currentScale.x + scaleAugment, currentScale.y + scaleAugment, currentScale.z + scaleAugment); // Aumentar el tamaño del prefab
        FireballExplosionInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Asignar el tipo de carta al prefab
        // Configurar el índice del inventario
        FireballExplosionInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

        // Destruir la instancia después de 1.5 segundos
        Destroy(FireballExplosionInstance, 0.3f);
    }

}
