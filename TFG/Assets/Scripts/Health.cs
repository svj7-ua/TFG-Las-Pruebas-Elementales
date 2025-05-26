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

    void Start()
    {
        if (healthBar != null)
        {
            // Sets the Health Bar active
            healthBar.SetActive(true);
            // Sets the max value of the health bar to the Max Health
            healthBar.GetComponent<Slider>().maxValue = maxHealth;  
            // Sets the current value of the health bar to the current health
            healthBar.GetComponent<Slider>().value = currentHealth;

            if (gameObject.name != "Player")
            {
                healthBar.GetComponentInChildren<TextMeshProUGUI>().text = gameObject.GetComponent<BossReferences>().GetBossName(); // Sets the text of the health bar to the current health and max health
            }
        }
    }

    public void UpdateHealthBar()
    {
        // Update the health bar UI here
        // For example, you can set the fill amount of a UI Image component to reflect the current health
        if (healthBar != null)
        {
            healthBar.GetComponent<Slider>().value = currentHealth;
            //TODO: Maybe add a lerp effect to the health bar? Or a kind of animation, like a wobble effect?
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

    public void EntityDeath(){

        //Will handle what happens when the entity dies. Mainly it will play the death animation. (Enemies are destroyed when the room is cleared, so no need to destroy them.)

    }

    public void UpdateMaxHealth(float newMaxHealth){

        float healthDifference = newMaxHealth - maxHealth; // Calculate the difference between the new max health and the current max health
        if (healthDifference > 0)
        {
            currentHealth += healthDifference; // Increase the current health by the difference
        }
        else if (currentHealth > newMaxHealth)
        {
            currentHealth = newMaxHealth; // Decrease the current health to the new max health if it's greater
        }

        maxHealth = newMaxHealth; // Update the max health value

        if(healthBar != null)
        {
            healthBar.GetComponent<Slider>().maxValue = maxHealth; // Update the max value of the health bar
        }
    }

}
