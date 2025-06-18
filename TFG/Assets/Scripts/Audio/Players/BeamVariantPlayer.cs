using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamVariantPlayer : MonoBehaviour
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


    }

    public void PlayBeamVariant()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.beam_2);
        }
        else
        {
            Debug.LogError("AudioManager is not initialized. Cannot play beam variant sound.");
        }
    }
}
