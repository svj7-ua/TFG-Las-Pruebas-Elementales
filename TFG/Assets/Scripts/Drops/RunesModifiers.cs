using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesModifiers : MonoBehaviour
{




    [Header("Life orbs")]
    [SerializeField] public GameObject lifeOrb;
    [SerializeField] public float lifeOrbDropChance = 0.1f; // 10% chance to drop a life orb
    [SerializeField] public int lifeOrbWeakEnemiesMaxAmount = 3; // Amount of gems to drop, can be adjusted in the inspector
    [SerializeField] public int lifeOrbStrongEnemiesMaxAmount = 5; // Amount of gems to drop, can be adjusted in the inspector
    [SerializeField] public int baseHealing = 10; // Base healing amount for the life orb
    [SerializeField] public int randomHealingRange = 15; // Random healing range to add variability
    [Space]
    [Header("Gems")]
    [SerializeField] public List<GameObject> gems;
    [SerializeField] public float gemDropChance = 0.2f; // 20% chance to drop a gem
    [SerializeField] public int gemWeakEnemiesMaxAmount = 5; // Amount of gems to drop, can be adjusted in the inspector
    [SerializeField] public int gemStrongEnemiesMinAmount = 5; // Minimum amount of gems to drop, can be adjusted in the inspector
    [SerializeField] public int gemStrongEnemiesMaxAmount = 25; // Amount of gems to drop, can be adjusted in the inspector

    [Space]
    [Header("Damage Modifiers")]
    [SerializeField] public float damageMultiplier = 1f;
    [SerializeField] public float rawBasicAttackDamageIncrement = 0f; // Raw damage increment to be added to the player's damage

}
