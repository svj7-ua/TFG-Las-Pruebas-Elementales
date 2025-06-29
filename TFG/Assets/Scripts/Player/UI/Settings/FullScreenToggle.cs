using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FullScreenToggle : MonoBehaviour
{

    [SerializeField]
    private Toggle toggle;

    /// <summary>
    /// Display and resolution couldn't be implemented. TO DO.
    /// </summary>

    [SerializeField]
    private TMP_Dropdown resolutionDropdown; // Optional: Dropdown for selecting resolutions
    private Resolution[] resolutions; // Array to hold available resolutions

    [SerializeField]
    private TMP_Dropdown displayDropdown; // Optional: Dropdown for selecting resolutions
    private Display[] display; // Array to hold available resolutions

    // Start is called before the first frame update
    void Start()
    {

        if (Screen.fullScreen)
        {
            toggle.isOn = true; // Set the toggle to on if the game is in full screen mode
        }
        else
        {
            toggle.isOn = false; // Set the toggle to off if the game is not in full screen mode
        }

        //CheckResolution(); // Check and update the resolution dropdown options
        //CheckDisplay(); // Check and update the display dropdown options


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleFullScreen(bool isOn)
    {
        Screen.fullScreen = isOn; // Set the full screen mode based on the toggle state
    }

    private void CheckDisplay()
    {

        displayDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < Display.displays.Length; i++)
        {
            options.Add("Display " + (i + 1));
        }

        displayDropdown.AddOptions(options);

        // Usa el valor guardado en GameData si está disponible
        if (GameData.displayIndex >= 0 && GameData.displayIndex < Display.displays.Length)
            displayDropdown.value = GameData.displayIndex;
        else
            displayDropdown.value = 0;

        displayDropdown.RefreshShownValue();
            
    }

    public void SetDisplay(int displayIndex)
    {
        if (displayIndex < 0 || displayIndex >= Display.displays.Length)
            return;

        if (!Display.displays[displayIndex].active)
            Display.displays[displayIndex].Activate();

        // Move the main camera to the selected display
        Camera.main.targetDisplay = displayIndex;

        // Update all canvases to the selected display
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas.targetDisplay = displayIndex;
                canvas.gameObject.SetActive(false); // Force refresh
                canvas.gameObject.SetActive(true);
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas.worldCamera = Camera.main;
            }
        }

        // Change the game resolution to match the selected display
        Screen.SetResolution(
            Display.displays[displayIndex].systemWidth,
            Display.displays[displayIndex].systemHeight,
            true // fullscreen
        );

        // Save the selected display index to GameData
        GameData.displayIndex = displayIndex;

        Camera.main.targetDisplay = displayIndex;
        Camera.main.Render(); // Force render the main camera

        Debug.Log("Display changed to: Display " + displayIndex);
    }


    private void CheckResolution()
    {
        // Optional: If you want to update the resolution dropdown based on the current screen resolution
        resolutions = Screen.resolutions; // Get all available resolutions
        resolutionDropdown.ClearOptions(); // Clear existing options

        int screenResolutionIndex = 0; // Variable to hold the index of the current screen resolution

        List<string> options = new List<string>();
        foreach (Resolution res in resolutions)
        {
            if (options.Contains(res.width + " x " + res.height))
                continue; // Skip if the resolution is already in the list to avoid duplicates
            options.Add(res.width + " x " + res.height); // Add each resolution to the list
            if (Screen.fullScreen && res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                screenResolutionIndex = options.Count - 1; // Set the index to the current resolution
            }
        }

        resolutionDropdown.AddOptions(options); // Add the options to the dropdown
        resolutionDropdown.value = screenResolutionIndex; // Set the dropdown to the last option (current resolution)
        resolutionDropdown.RefreshShownValue(); // Refresh the dropdown to show the current value

        if (GameData.resolutionIndex >= 0)
            resolutionDropdown.value = GameData.resolutionIndex; // Set the dropdown value to the saved resolution index from GameData
    }
    
    public void SetResolution(int resolutionIndex)
    {

        GameData.resolutionIndex = resolutionIndex; // Save the selected resolution index to GameData

        Resolution selectedResolution = resolutions[resolutionIndex]; // Get the selected resolution
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen); // Set the screen resolution
    }
}
