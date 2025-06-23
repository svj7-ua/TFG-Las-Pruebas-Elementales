using UnityEngine;

public class FireExplosionPlayer : MonoBehaviour
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
        
        audioManager.PlaySFX(audioManager.fireBall); // Play background music at the start

    }
}