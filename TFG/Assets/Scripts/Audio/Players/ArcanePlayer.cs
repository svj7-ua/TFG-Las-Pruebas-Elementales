using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcanePlayer : MonoBehaviour
{
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {

        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene. Please ensure it is present.");
        }
        
        audioManager.PlaySFX(audioManager.arcane); // Play background music at the start

    }
}
