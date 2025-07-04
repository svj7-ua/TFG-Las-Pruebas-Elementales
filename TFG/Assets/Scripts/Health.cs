using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public float currentHealth = 100f;
    public float maxHealth = 100f; // Maximum health of the player

    [Header("Health Bar (Only for the player, maybe bosses in the future)")]
    [SerializeField] public GameObject healthBar; // The health bar object
    [SerializeField] private GameObject healthValueText; // The text object that displays the health value (not used yet, but can be used in the future)

    private RunesModifiers runesModifierManager; // Reference to the runesModifierManager
    private LevelInformation levelInformation; // Reference to the LevelInformation script

    private bool alreadyDropped = false; // Flag to check if the drops have already been instantiated

    private GameObject root;
    private GameObject player; // Reference to the player object

    void Start()
    {
        levelInformation = FindObjectOfType<LevelInformation>(); // Find the LevelInformation script in the scene


        if (healthBar != null)
        {

            Hurtbox hurtbox = gameObject.GetComponent<Hurtbox>(); // Get the Hurtbox component from the game object
            if (hurtbox != null)
            {
                if (hurtbox.isBoss)
                {
                    SetupBossHealthBar();
                }
                else if (hurtbox.isEnemy)
                {
                    SetupNormalEnemyHealthBar(); // Setup the health bar for normal enemies
                }
                else
                {
                    SetupPlayerHealthBar();
                }
            }
            else
            {
                Debug.LogError("Hurtbox component not found on the game object! Please add a Hurtbox component to the game object.");
            }
            healthBar.SetActive(true);
        }
        else
        {

            Debug.LogError("Health Bar not assigned! Please assign the health bar in the inspector.");

        }

        if (gameObject.name != "Player")
        {
            runesModifierManager = FindObjectOfType<RunesModifiers>(); // Find the runesModifierManager script in the scene
            root = GameObject.Find("RoomsGeneratorRoot"); // Find the root object in the scene
            player = GameObject.Find("Player"); // Find the player object in the scene
        }
    }

    private void SetupNormalEnemyHealthBar()
    {
        // Increments the health based on the current level for all normal enemies (Not bosses neither the player)
        float healthIncrementMultiplier = 0.5f + levelInformation.GetLevel() * 0.5f; // Increases the health increment based on the current level
        maxHealth *= healthIncrementMultiplier; // Increases the max health based on the current level
        currentHealth = maxHealth; // Sets the current health to the max health
        healthBar.GetComponent<Slider>().maxValue = maxHealth; // Multiplies the max health by the current level to increase the difficulty
        // Sets the current value of the health bar to the current health
        healthBar.GetComponent<Slider>().value = currentHealth; // Multiplies the current health by the current level to increase the difficulty
    }

    private void SetupBossHealthBar()
    {
        maxHealth = maxHealth * levelInformation.GetLevel();
        currentHealth = maxHealth; // Sets the current health to the max health
        healthBar.GetComponentInChildren<TextMeshProUGUI>().text = gameObject.GetComponent<BossReferences>().GetBossName(); // Sets the text of the health bar to the current health and max health
        // Sets the max value of the health bar to the Max Health
        healthBar.GetComponent<Slider>().maxValue = maxHealth; // Multiplies the max health by the current level to increase the difficulty
        // Sets the current value of the health bar to the current health
        healthBar.GetComponent<Slider>().value = currentHealth; // Multiplies the current health by the current level to increase the difficulty
    }

    private void SetupPlayerHealthBar()
    {
        // Sets the max value of the health bar to the Max Health
        healthBar.GetComponent<Slider>().maxValue = maxHealth; // Multiplies the max health by the current level to increase the difficulty
        // Sets the current value of the health bar to the current health
        healthBar.GetComponent<Slider>().value = currentHealth; // Multiplies the current health by the current level to increase the difficulty
        healthValueText.GetComponent<TextMeshProUGUI>().text = $"{currentHealth}/{maxHealth}"; // Sets the text of the health bar to the current health and max health
    }

    public void UpdateHealthBar()
    {
        // Update the health bar UI here
        // For example, you can set the fill amount of a UI Image component to reflect the current health
        if (healthBar != null)
        {
            healthBar.GetComponent<Slider>().value = currentHealth;
            if (healthValueText != null)
            {
                healthValueText.GetComponent<TextMeshProUGUI>().text = $"{currentHealth}/{maxHealth}"; // Update the health value text
            }
        }
    }

    public void UpdateMaxHealth(float newMaxhealth)
    {
        if (healthBar != null)
        {

            // Truncate newMaxHealth so it is always a whole number
            newMaxhealth = Mathf.Floor(newMaxhealth); // Ensure the new max health is a whole number

            healthBar.GetComponent<Slider>().maxValue = newMaxhealth; // Update the max value of the health bar
            maxHealth = newMaxhealth; // Update the max health variable

            if (newMaxhealth < currentHealth) // If the new max health is less than the current health
            {
                currentHealth = newMaxhealth; // Set the current health to the new max health
                healthBar.GetComponent<Slider>().value = currentHealth; // Update the health bar value to the new current health
            }

            if (healthValueText != null)
            {
                healthValueText.GetComponent<TextMeshProUGUI>().text = $"{currentHealth}/{maxHealth}"; // Update the health value text
            }
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount; // Increase the current health by the specified amount
        if (currentHealth > maxHealth) // If the current health exceeds the maximum health
        {
            currentHealth = maxHealth; // Set it to the maximum health
        }
        UpdateHealthBar(); // Update the health bar UI
    }

    public void PlayerDeath()
    {
        gameObject.GetComponent<PlayerController_test>().FinishRun();
    }

    public void EntityDeath(bool isBoss = false)
    {
        player.GetComponent<Health>().Heal(runesModifierManager.soulVampirismLifeGain); // Heal the player for the amount of health growth from the Soul Eater rune
        player.GetComponent<Health>().UpdateMaxHealth(player.GetComponent<Health>().maxHealth + runesModifierManager.soulEaterHealthGrowth); // Increase the player's max health by the amount of health growth from the Soul Eater rune

        if (isBoss)
        {
            gameObject.GetComponent<BossReferences>().SetCurrentState(EnumBossesStates.Dead); // Trigger the death animation for the boss
            healthBar.SetActive(false); // Deactivate the health bar
        }
        else
        {
            gameObject.SetActive(false); // Deactivate the game object
        }
        
        InstantiateDrops(isBoss); // Instantiate drops for the enemy

    }

    private void InstantiateDrops(bool isboos)
    {

        if (alreadyDropped) return; // If drops have already been instantiated, exit the method
        alreadyDropped = true; // Set the flag to true to prevent further drops

        DropLifeOrbs(isboos); // Call the method to drop life orbs
        DropGems(isboos); // Call the method to drop gems
    }

    private void DropLifeOrbs(bool isboos)
    {

        int maxNumberOfDrops = runesModifierManager.lifeOrbWeakEnemiesMaxAmount; // Default maximum number of drops
        float chanceToDropLifeOrb = runesModifierManager.lifeOrbDropChance; // Default chance to drop a life orb
        if (isboos)
        {
            maxNumberOfDrops = runesModifierManager.lifeOrbStrongEnemiesMaxAmount; // Increase the maximum number of drops for bosses
            chanceToDropLifeOrb = 1f; // Increase the chance to drop a life orb for bosses
        }

        if (Random.value <= chanceToDropLifeOrb)
        {
            int lifeOrbDrops = Random.Range(1, maxNumberOfDrops + 1); // Randomly determine the number of life orbs to drop
            for (int i = 0; i < lifeOrbDrops; i++)
            {

                // Generates the life orbs a random position around the enemy (+- 1 units in x and z axis)
                Vector3 randomPosition = new Vector3(
                    transform.position.x + Random.Range(-1f, 1f),
                    0.75f,
                    transform.position.z + Random.Range(-1f, 1f)
                );

                // Instantiate the life orb at the random position
                GameObject orb = Instantiate(runesModifierManager.lifeOrb, randomPosition, Quaternion.identity);
                orb.transform.SetParent(root.transform); // Set the parent of the orb to the root object

            }
        }

    }

    private void DropGems(bool isboos)
    {

        int maxNumberOfDrops = runesModifierManager.gemWeakEnemiesMaxAmount; // Default maximum number of drops
        int minNumberOfDrops = 1; // Minimum number of drops is 1
        float chanceToDropGem = runesModifierManager.gemDropChance; // Default chance to drop a gem
        if (isboos)
        {
            maxNumberOfDrops = runesModifierManager.gemStrongEnemiesMaxAmount; // Increase the maximum number of drops for bosses
            minNumberOfDrops = runesModifierManager.gemStrongEnemiesMinAmount;
            chanceToDropGem = 1f; // Increase the chance to drop a gem for bosses
        }

        if (Random.value <= chanceToDropGem)
        {
            int gemDrops = Random.Range(minNumberOfDrops, maxNumberOfDrops + 1); // Randomly determine the number of gems to drop
            for (int i = 0; i < gemDrops; i++)
            {

                // Generates the gems a random position around the enemy (+- 1 units in x and z axis)
                Vector3 randomPosition = new Vector3(
                    transform.position.x + Random.Range(-1f, 1f),
                    0.75f,
                    transform.position.z + Random.Range(-1f, 1f)
                );

                // Instantiate the gem at the random position
                GameObject gem = Instantiate(runesModifierManager.gems, randomPosition, Quaternion.identity);
                gem.transform.SetParent(root.transform); // Set the parent of the gem to the root object

            }
        }

    }
    
}
