using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// This script is attached to the hurtbox object, which is a child of the player object.
// The hurtbox object is a trigger collider that is used to detect when the player is hit by an enemy attack. When the hurtbox is triggered by an enemy hitbox, it sends a message to the player object to reduce the player's health.
public class Hurtbox : MonoBehaviour
{

    private float HIT_ANIMATION_TIME = 0.4f; // Time of the hit animation

    public Health health;
    public Animator animator; // Reference to the animator component

    [Header("Enemy Types")]
    public bool isEnemy; // Indicates if the hurtbox is an enemy or not
    public bool isBoss;

    private bool isPoisoned = false; // Indicates if is poisoned
    private float poisonDuration = 0.0f; // Duration of the poison effect
    private float poisonTickTime = 0.0f; // Cooldown time between ticks

    private float lastPoisonTime = 0.0f; // Time of the last poison tick
    private float poisonAmmount = 0.0f; // Amount of health to lose

    private bool isIgnited = false; // Indicates if is on fire

    private float fireDuration = 0.0f; // Duration of the fire effect

    private float fireTickTime = 0.0f; // Cooldown time between ticks

    private float lastFireTime = 0.0f; // Time of the last fire tick

    private float fireAmmount = 0.0f; // Amount of health to lose
    [Header("Immunities and Resistances")]
    public bool isInmuneToPoison = false; // Indicates if is immune to poison
    public bool isResistantToPoison = false; // Indicates if is resistant to poison
    [Space]
    public bool isInmuneToFire = false; // Indicates if is immune to fire
    public bool isResistantToFire = false; // Indicates if is resistant to fire
    [Space]
    public bool isInmuneToLightning = false; // Indicates if is immune to lightning
    public bool isResistantToLightning = false; // Indicates if is resistant to lightning
    [Space]
    public bool isInmuneToWind = false; // Indicates if is immune to wind
    public bool isResistantToWind = false; // Indicates if is resistant to wind
    [Space]
    public bool isInmuneToArcane = false; // Indicates if is immune to arcane
    public bool isResistantToArcane = false; // Indicates if is resistant to arcane

    private bool isStunned = false; // Indicates if is stunned

    private bool isTrapped = false; // Indicates if is trapped

    // Boss references, only initialized if the object is a boss
    private BossReferences bossReferences; // Reference to the boss references script

    private bool hitCoroutineRunning = false; // Flag to check if the hit animation coroutine is running
    private bool bossHitCoroutineRunning = false; // Flag to check if the boss hit animation coroutine is running

    private InventoryController playerInventoryController; // Reference to the player's inventory controller

    private void Start()
    {
        health = GetComponentInParent<Health>();
        animator = GetComponentInParent<Animator>(); // Get the animator component from the parent object
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player object by tag
        if (player != null)
        {
            playerInventoryController = player.GetComponent<InventoryController>(); // Get the inventory controller component from the player object
        }
        else
        {
            Debug.LogError("Player object not found, inventory controller couldn't be set.");
        }
    }

    public void SetBossReferences(BossReferences bossReferences)
    {
        this.bossReferences = bossReferences; // Set the boss references script
    }

    private void Update()
    {

        if (isPoisoned && (!isInmuneToPoison || (isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Poison])))
        {
            ApplyPoison(); // Apply poison effect if the player is poisoned and not immune
        }

        if (isIgnited && (!isInmuneToFire || (isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Fire])))
        {
            ApplyFire(); // Apply fire effect if the player is on fire and not immune
        }

    }

    public void EnemyHit()
    {
        if (gameObject.activeSelf)
        {
            if (!isBoss)
            {
                // Check if the Coroutine is already running, if not, start it
                if (!hitCoroutineRunning)
                {
                    hitCoroutineRunning = true; // Set the flag to true
                    StartCoroutine(HitAnimationStunHandler()); // Start the hit animation coroutine
                }
            }
            else
            {
                // Check if the Coroutine is already running, if not, start it
                if (!bossHitCoroutineRunning)
                {
                    bossHitCoroutineRunning = true; // Set the flag to true
                    StartCoroutine(BossHitAnimationStunHandler()); // Start the hit animation coroutine

                }
            }
        }
    }

    public void PoisonTarget(float poisonAmmount, float poisonDuration, float poisonTickTime)
    {
        if (!isResistantToPoison || (isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Poison]))
        { // Check if the player is not immune to poison
            this.poisonAmmount = poisonAmmount; // Set the amount of health to heal
            this.poisonDuration = poisonDuration; // Set the duration of the poison effect

        }
        else
        {
            // IF the player is resistant to poison, all effects are halved
            this.poisonAmmount = poisonAmmount / 2.0f; // Set the amount of health to heal
            this.poisonDuration = poisonDuration / 2.0f; // Set the duration of the poison effect
        }
        this.poisonTickTime = poisonTickTime; // Set the cooldown time between ticks
        isPoisoned = true; // Set the poisoned state to true
        lastPoisonTime = Time.time; // Set the last poison time to the current time
    }

    public bool isTargetPoisoned()
    {
        return isPoisoned; // Return the poisoned state
    }

    private void ApplyPoison()
    {
        poisonDuration -= Time.deltaTime; // Reduce the duration of the poison effect
        if (poisonDuration >= 0.0f)
        {

            // Check if enough time has passed since the last poison tick
            if (Time.time - lastPoisonTime >= poisonTickTime)
            {
                // Apply poison effect to the object
                health.currentHealth -= poisonAmmount; // Reduce health by the poison amount
                health.UpdateHealthBar(); // Update the health bar UI
                // if the enemy has no healt bar it will do nothing

                Debug.Log("Poisoned " + gameObject.name + " for " + poisonAmmount + " health. Current health: " + health.currentHealth);

                CheckDeath(); // Check if the object is dead

                lastPoisonTime = Time.time; // Update the last poison time
            }

        }
        else
        {
            isPoisoned = false; // Reset the poison effect
        }
    }

    public void SetOnFire(float fireAmmount, float fireDuration, float fireTickTime)
    {

        Debug.Log("SetOnFire called on " + gameObject.name); // Debug message to check if the function is called
        if (!isResistantToFire || (isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Fire]))
        { // Check if the player is not immune to fire
            this.fireAmmount = fireAmmount; // Set the amount of health to heal
            this.fireDuration = fireDuration; // Set the duration of the fire effect

        }
        else
        {
            // IF the player is resistant to fire, all effects are halved
            this.fireAmmount = fireAmmount / 2.0f; // Set the amount of health to heal
            this.fireDuration = fireDuration / 2.0f; // Set the duration of the poison effect
        }
        this.fireTickTime = fireTickTime; // Set the cooldown time between ticks
        lastFireTime = Time.time; // Set the last poison time
        isIgnited = true; // Set the poisoned state to true

    }

    public bool isOnFire()
    {
        return isIgnited; // Return the poisoned state
    }

    private void ApplyFire()
    {
        Debug.Log("ApplyFire called on " + gameObject.name); // Debug message to check if the function is called
        fireDuration -= Time.deltaTime; // Reduce the duration of the poison effect
        if (fireDuration >= 0.0f)
        {

            // Check if enough time has passed since the last poison tick
            if (Time.time - lastFireTime >= fireTickTime)
            {
                // Apply poison effect to the object
                health.currentHealth -= fireAmmount; // Reduce health by the poison amount
                health.UpdateHealthBar(); // Update the health bar UI
                // if the enemy has no healt bar it will do nothing

                Debug.Log("Burnt " + gameObject.name + " for " + fireAmmount + " health. Current health: " + health.currentHealth);

                CheckDeath(); // Check if the object is dead

                lastFireTime = Time.time; // Update the last poison time
            }

        }
        else
        {
            isIgnited = false; // Reset the poison effect
        }
    }

    public void TrapTarget(Vector3 position)
    {

        if (isBoss)
        {
            // Bosses cannot be trapped, so we return
            return;
        }

        Debug.Log("StunTarget called on " + gameObject.name); // Debug message to check if the function is called

        // Pauses the enemy navmesh agent to stop the enemy from moving
        gameObject.GetComponent<NavMeshAgent>().isStopped = true; // Disable the enemy navmesh agent to stop the enemy from moving
        isTrapped = true; // Set the trapped state to true 

    }

    public void FreeTarget()
    {

        Debug.Log("FreeTarget called on " + gameObject.name); // Debug message to check if the function is called

        // Enable the enemy controller script to allow the enemy to move and the enemy collider so it becomes vulnerable
        gameObject.GetComponent<NavMeshAgent>().isStopped = false; // Enable the enemy navmesh agent to allow the enemy to move
        isTrapped = false; // Set the trapped state to false
        gameObject.GetComponent<NavMeshAgent>().ResetPath(); // Reset the path of the navMeshAgent
    }

    IEnumerator HitAnimationStunHandler()
    {
        // Disable the navmesh agent to stop the enemy from moving
        animator.SetTrigger("Damaged");
        gameObject.GetComponent<NavMeshAgent>().isStopped = true; // Disable the enemy navmesh agent to stop the enemy from moving
        //gameObject.GetComponent<NavMeshAgent>().ResetPath(); // Reset the path of the navMeshAgent
        isStunned = true; // Set the stunned state to true
        //Gets the Hit animation time from the animator
        HIT_ANIMATION_TIME = animator.GetCurrentAnimatorStateInfo(0).length; // Get the length of the hit animation
        Debug.Log("Hit animation time: " + HIT_ANIMATION_TIME); // Debug message to check the hit animation time

        yield return new WaitForSeconds(HIT_ANIMATION_TIME); // Wait for the stun time

        if (!isTrapped)
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = false; // Enable the enemy navmesh agent to allow the enemy to move again
            gameObject.GetComponent<NavMeshAgent>().ResetPath(); // Reset the path of the navMeshAgent

        }
        isStunned = false; // Set the stunned state to false
        hitCoroutineRunning = false; // Set the flag to false


    }

    IEnumerator BossHitAnimationStunHandler()
    {

        if (bossReferences.GetCurrentState() == EnumBossesStates.Idle)
        {
            Debug.Log("Boss hit animation stun handler called"); // Debug message to check if the function is called
            animator.SetTrigger("Damaged");
            gameObject.GetComponent<NavMeshAgent>().isStopped = true; // Disable the enemy navmesh agent to stop the enemy from moving
            gameObject.GetComponent<NavMeshAgent>().ResetPath(); // Reset the path of the navMeshAgent
            yield return new WaitForSeconds(5.0f); // Wait for the stun time
            animator.SetTrigger("EndDamaged"); // Set the end hit animation trigger
            gameObject.GetComponent<NavMeshAgent>().isStopped = false; // Enable the enemy navmesh agent to allow the enemy to move again
            bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the current state to idle
            yield return null;
        }
        // else
        // {
        //     Debug.LogWarning("Boss is not in idle state, cannot perform hit animation. " + bossReferences.GetCurrentState());
        // }

        bossHitCoroutineRunning = false; // Set the flag to false

    }

    public bool IsStunned()
    {
        return isStunned; // Return the stunned state
    }

    public float calculateDamage(float baseDamage, EnumDamageTypes damageType)
    {
        float finalDamage = baseDamage; // Start with the base damage
        switch (damageType)
        {
            case EnumDamageTypes.Fire:
                if (isInmuneToFire && !(isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Fire])) finalDamage = 0.0f;
                if (isResistantToFire && !(isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Fire])) finalDamage = baseDamage / 2.0f; // Fire damage is halved if the target is resistant to fire
                break;
            case EnumDamageTypes.Lightning:
                if (isInmuneToLightning && !(isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Lightning])) finalDamage = 0.0f;
                if (isResistantToLightning && !(isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Lightning])) finalDamage = baseDamage / 2.0f; // Electric damage is halved if the target is resistant to electricity
                break;

            case EnumDamageTypes.Poison:
                if (isInmuneToPoison && !(isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Poison])) finalDamage = 0.0f;
                if (isResistantToPoison && !(isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Poison])) finalDamage = baseDamage / 2.0f; // Poison damage is halved if the target is resistant to poison
                break;
            case EnumDamageTypes.Wind:
                if (isInmuneToWind && !(isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Wind])) finalDamage = 0.0f;
                if (isResistantToWind && !(isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Wind])) finalDamage = baseDamage / 2.0f; // Wind damage is halved if the target is resistant to wind
                break;
            case EnumDamageTypes.Arcane:
                if (isInmuneToArcane && !(isEnemy && playerInventoryController.ignoredImmunities[(int)EnumElementTypes.Arcane])) finalDamage = 0.0f;
                if (isResistantToArcane && !(isEnemy && playerInventoryController.ignoredResistances[(int)EnumElementTypes.Arcane])) finalDamage = baseDamage / 2.0f; // Arcane damage is halved if the target is resistant to arcane
                break;
            // More cases can be added here for different damage types
            default:
                break; // Default case, return base damage
        }

        if (isBoss || isEnemy)
        {
            finalDamage *= playerInventoryController.runesModifiers.damageMultiplier;
        }

        finalDamage = Mathf.RoundToInt(finalDamage);

        health.currentHealth -= finalDamage; // Reduce health by the final damage amount
        if(health.currentHealth < 0) health.currentHealth = 0; // Ensure health does not go below 0

        health.UpdateHealthBar(); // Update the health bar UI
        CheckDeath();
        // if the enemy has no healt bar it will do nothing

        return finalDamage;
    }

    private void CheckDeath()
    {
        if (health.currentHealth <= 0)
        { // Check if the health is less than or equal to 0
            health.currentHealth = 0; // Set health to 0
            if (isEnemy)
            { // Check if the object is an enemy

                health.EntityDeath(isBoss); // Call the entity death function to handle the death of the enemy

            }
            else
            {
                health.PlayerDeath(); // Call the player death function to handle the death of the player
            }

        }
    }

    public bool IsTrapped()
    {
        return isTrapped; // Return the trapped state
    }

}
