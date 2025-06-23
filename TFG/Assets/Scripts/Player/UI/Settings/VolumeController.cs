using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{

    [SerializeField]
    private Slider slider;
    private float volume; // Default volume level
    // Start is called before the first frame update
    void Start()
    {
        volume = GameData.volume; // Load the volume from GameData
        AudioListener.volume = volume; // Set the AudioListener volume to the loaded value
        slider.value = volume; // Set the slider value to the current volume
    }

    // Update is called once per frame
    public void ChangeSlider(float value)
    {
        volume = value; // Update the volume variable
        GameData.volume = volume; // Save the new volume to GameData
        AudioListener.volume = volume; // Set the AudioListener volume to the new value
        Debug.Log("Volume changed to: " + volume); // Log the new volume level
    }
}
