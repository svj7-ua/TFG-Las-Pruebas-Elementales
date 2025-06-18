using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{

    [SerializeField]
    private GameObject sceneLoader; // Reference to the loading screen GameObject
    [SerializeField]
    private GameObject settingsMenu; // Reference to the settings menu GameObject
    [SerializeField]
    private GameObject tutorialMenu; // Reference to the tutorial menu GameObject

    public void BackToLobby()
    {
        Debug.Log("Returning to MainLobby...");
        // Loads the MainLobby scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            sceneLoader.GetComponentInChildren<LoadingScreen>().LoadScene(1); // MainLobby scene index is 1
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f; // Reset time scale to normal if it was paused
            }
        }
        else
        {
            Debug.LogError("EndRun: Not in GameScene. Cannot load MainLobby.");
        }

    }

    public void OpenTutorial_MainMenu()
    {
        Debug.Log("Opening tutorial from Main Menu...");
        // Opens the tutorial menu from the main menu
        if (tutorialMenu != null)
        {
            tutorialMenu.SetActive(true);
            settingsMenu.SetActive(false); // Ensure settings menu is closed
        }
        else
        {
            Debug.LogError("Tutorial menu not found!");
        }
    }
    
    public void OpenSettings()
    {
        Debug.Log("Opening settings menu...");
        // Opens the settings menu
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
            tutorialMenu.SetActive(false); // Ensure tutorial menu is closed
        }
        else
        {
            Debug.LogError("Settings menu not found!");
        }
    }

    public void OpenTutorial()
    {
        Debug.Log("Opening tutorial menu...");
        // Opens the tutorial menu
        if (tutorialMenu != null)
        {
            tutorialMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("Tutorial menu not found!");
        }
    }

    public void CloseTutorial()
    {
        Debug.Log("Closing tutorial menu...");
        // Closes the tutorial menu
        if (tutorialMenu != null)
        {
            tutorialMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("Tutorial menu not found!");
        }
    }

    public void CloseSettings()
    {
        Debug.Log("Closing settings menu...");
        // Closes the settings menu
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("Settings menu not found!");
        }
    }

    public void StartGame()
    {

        Debug.Log("Starting game...");
        // Loads the GameScene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {

            // Loads the player data to the GameData static class
            sceneLoader.GetComponentInChildren<LoadingScreen>().LoadScene(1); // MainLobby scene index is 1



        }
        else
        {
            Debug.LogError("StartGame: Not in MainMenu. Cannot load MainLobby.");
        }
    }

    public void QuitGame()
    {
        // Quits the application
        Debug.Log("Quitting game...");
        // Saves the player data before quitting
        Debug.LogWarning("Stored Data: " + GameData.gems + ", " + GameData.equipedSpellCard + ", " + GameData.equipedSpellCardType + ", " + GameData.equipedRune);
        PlayerGameData playerData = new PlayerGameData(GameData.gems, GameData.unlockedSpellCards, GameData.unlockedRunes, GameData.equipedSpellCard, GameData.equipedSpellCardType, GameData.equipedRune, GameData.volume, GameData.resolutionIndex, GameData.displayIndex);
        SaveSystem.SavePlayerData(playerData);
        Debug.Log("Player data saved before quitting.");

        Application.Quit();
    }

    public void EndRun()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController_test>().FinishRun();

        // Sets its parent object to inactive
        
        gameObject.SetActive(false);
    }
}
