using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitbox : MonoBehaviour
{

    public float damage = 10f;
    public Vector3 knockback = new Vector3(0, 0, 0);

    public EnumDamageTypes damageType = EnumDamageTypes.None; // The type of damage this hitbox does.

    [Space]
    [Header("Secondary Effect related variables")]
    [Tooltip("If this hitbox has a secondary effect, set it here.")]

    [SerializeField]
    private bool hasSecondaryEffect = false; // Whether this hitbox has tick damage or not.

    [SerializeField]
    private float secondaryEffectDamage = 0f; // The amount of damage the secondary effect does. (tick damage, mostly)
    [SerializeField]
    private float secondaryEffectDuration = 0f; // The duration of the secondary effect.
    [SerializeField]
    private float secondaryEffectTickInterval = 0f; // The interval between ticks of the secondary effect. (if any)

    [Space]
    [Header("Hitbox related variables")]
    [Tooltip("This is the layer mask that determines which objects the hitbox checks.")]
    public LayerMask layerMask; // This is the layer mask that determines which objects the hitbox checks.

    [Space]
    [Header("Check if this hitbox has to also check for stay collisions")]
    [Tooltip("If this hitbox has to check for stay collisions, set it here.")]
    [SerializeField]
    private bool CheckStay = false; // Whether to check for stay collisions or not.
    [SerializeField]
    [Tooltip("The time between stay collisions.")]
    [Range(0.1f, 1f)]
    private float TickCooldown = 0.1f; // The time to check for stay collisions.
    private float lastTickTime = 0f; // The timer for the stay collisions.

    void Start()
    {
        if(damageType == EnumDamageTypes.None){
            Debug.LogWarning("Hitbox " + gameObject.name + " has no damage type set. Please set a damage type.");
        }

        //Debug.Log("Hitbox " + gameObject.name + " with damage type: " + damageType.ToString() + " has been initialized.");
    }

    private void ApplySecondaryEffect(Hurtbox h){
        h.ApplySecondaryEffect(damageType, secondaryEffectDuration, secondaryEffectDamage, secondaryEffectTickInterval);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (CheckStay)
        {
            //Debug.Log("Hitbox " + gameObject.name + " collided with " + other.gameObject.name + " with damage type: " + damageType.ToString() + " and damage: " + damage.ToString());

            if (layerMask == (layerMask | (1 << other.gameObject.layer)))
            {

                if (Time.time - lastTickTime >= TickCooldown)
                {
                    lastTickTime = Time.time;

                    //Debug.Log("Hitbox " + gameObject.name + " collided with " + other.gameObject.name + " with damage type: " + damageType.ToString() + " and damage: " + damage.ToString());
                                    Hurtbox h = other.GetComponent<Hurtbox>();

                    if (h != null)
                    {

                        float final_damage = h.calculateDamage(damage, damageType);

                        if (final_damage > 0 && hasSecondaryEffect)
                        {
                            ApplySecondaryEffect(h);
                        }

                    }

                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        //Debug.Log("Hitbox " + gameObject.name + " collided with " + other.gameObject.name + " with damage type: " + damageType.ToString() + " and damage: " + damage.ToString());

        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {


            Hurtbox h = other.GetComponent<Hurtbox>();

            if (h != null)
            {

                float final_damage = h.calculateDamage(damage, damageType);

                if (final_damage > 0 && hasSecondaryEffect)
                {
                    ApplySecondaryEffect(h);
                }

            }

        }

    }

}
