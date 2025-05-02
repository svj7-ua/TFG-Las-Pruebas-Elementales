using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : ScriptableObject, IEffect
{
    [SerializeField]
    GameObject TornadoPrefab; // Prefab del tornado

    EnumSpellCards spellCard = EnumSpellCards.None; // Tipo de carta de hechizo
    EnumSpellCardTypes spellCardType = EnumSpellCardTypes.Melee;

    private float durationAugment = 0.0f; // Duración del efecto de veneno
    private string description = ""; // Descripción del efecto

    private void Awake()
    {
        // Obtener el prefab desde el EffectManager
        TornadoPrefab = EffectManager.Instance?.tornadoPrefab;
        TornadoPrefab.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType);
        Debug.Log("ElectricExplosion prefab: " + TornadoPrefab);
        // Verificar si el prefab está asignado

        if (TornadoPrefab == null)
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
        // Adds 1 second to its duration

        durationAugment += 1.0f; // Aumentar la duración del efecto
        Debug.Log("Tornado effect upgraded!");
    }

    public string getDescription()
    {
        if (EnumSpellCardTypes.Melee == spellCardType)
        {
            description = "Effect: When hitting an enemy with a melee attack, summons a tornado over the target which moves randomly and applies impact effects to all affected enemies.";
        }
        else if (EnumSpellCardTypes.Ranged == spellCardType)
        {
            description = "Effect: When hitting an enemy with a ranged attack, summons a tornado over the target which moves randomly and applies impact effects to all affected enemies.";

        }
        else
        {
            description = "Effect: When dashing, summons a tornado over all enemies in a small area around the player, which moves randomly and applies impact effects to all affected enemies.";

        }

        description += "\nDuration: " + TornadoPrefab.GetComponent<TornadoController>().getDuration() +durationAugment + " seconds"; 
        description += "\nDamage: " + TornadoPrefab.GetComponent<Player_Hitbox>().damage; 
        description += "\nDamage Type: " + EnumDamageTypes.Wind.ToString(); // Añadir el tipo de daño al final de la descripción
        return description;
    }


    public void ApplyEffect(GameObject target, int index = 0)
    {

        // Instanciar el prefab en la posición del objetivo
        GameObject TornadoInstance = Instantiate(TornadoPrefab, new Vector3(target.transform.position.x, 0, target.transform.position.z), Quaternion.Euler(-90, 0, 0));
        TornadoInstance.GetComponent<Player_Hitbox>().SetSpellCardType(spellCardType); // Set the spell card type for the instance
        TornadoInstance.GetComponent<TornadoController>().addDuration(durationAugment); // Auments its duration
        // Configurar el índice del inventario
        TornadoInstance.GetComponent<Player_Hitbox>().SetInventoryIndex(index + 1);

    }
}
