using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public float currentHealth = 100f;
    public float maxHealth = 100f; // Maximum health of the player

    [Header("Health Bar (Only for the player, maybe bosses in the future)")]
    public GameObject healthBar; // The health bar object

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
