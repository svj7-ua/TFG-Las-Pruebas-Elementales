using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeOrbPickUp : MonoBehaviour
{

    private LifeOrbReferences lifeOrbReferences; // Reference to the LifeOrbReferences component

    private RunesModifiers runesModifiers;

    private int healingAmount = 0; // Total healing amount to apply
    // Start is called before the first frame update
    void Start()
    {
        // Get the LifeOrbReferences component attached to this GameObject
        lifeOrbReferences = GetComponentInParent<LifeOrbReferences>();
        runesModifiers = FindObjectOfType<RunesModifiers>(); // Find the RunesModifierManager in the scene
        if (lifeOrbReferences == null)
        {
            Debug.LogError("LifeOrbPickUp: LifeOrbReferences component not found on this GameObject.");
        }

        healingAmount = runesModifiers.baseHealing + Random.Range(0, runesModifiers.randomHealingRange);
    }

    void OnTriggerEnter(Collider other)
    {

        if (((1 << other.gameObject.layer) & lifeOrbReferences.playerLayer) != 0)
        {
            lifeOrbReferences.playerHealth.Heal(healingAmount); // Heal the player
        }
        // Destroy the life orb after picking it up (Destroy the parent object, since the LifeOrbPickUp is a child of the LifeOrb)
        Destroy(lifeOrbReferences.gameObject);

    }

}
