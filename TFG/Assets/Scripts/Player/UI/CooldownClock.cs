using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CooldownClock : MonoBehaviour
{

    private Image cooldownImage; // Reference to the cooldown image
    private float cooldownTime; // The total cooldown time in seconds

    private bool startCooldown = false; // Flag to check if the cooldown has started
    private float elapsedTime = 0f; // Time elapsed since the cooldown started
    // Start is called before the first frame update
    void Start()
    {
        cooldownImage = GetComponent<Image>(); // Get the Image component attached to this GameObject
        if (cooldownImage == null)
        {
            Debug.LogError("CooldownClock: Image component not found on this GameObject. Please ensure it has an Image component.");
        }
        else
        {
            cooldownImage.fillAmount = 0f; // Initialize the fill amount to 0
        }
    }

    public void SetCooldown(float cooldownTime)
    {
        if (cooldownImage != null)
        {
            // Set the fill amount based on the cooldown time
            cooldownImage.fillAmount = 1f; // Reset to full before starting the cooldown
            this.cooldownTime = cooldownTime; // Store the cooldown time
            startCooldown = true; // Start the cooldown
            elapsedTime = 0f; // Reset the elapsed time

        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (startCooldown && cooldownImage != null)
        {
            elapsedTime += Time.deltaTime; // Increment the elapsed time by the time since the last frame
            float fillAmount = 1f - (elapsedTime / cooldownTime); // Calculate the fill amount based on the elapsed time
            cooldownImage.fillAmount = Mathf.Clamp(fillAmount, 0f, 1f); // Update the fill amount, ensuring it stays between 0 and 1

            if (elapsedTime >= cooldownTime)
            {
                startCooldown = false; // Stop the cooldown when the time is up
                cooldownImage.fillAmount = 0f; // Reset the fill amount to 0
            }
        }

    }
}
