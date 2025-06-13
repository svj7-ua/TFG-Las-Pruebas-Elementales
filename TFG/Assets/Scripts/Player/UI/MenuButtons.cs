using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void BackToLobby()
    {
        Debug.Log("Returning to MainLobby...");
        // Loads the MainLobby scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainLobby");
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

    public void StartGame()
    {

        Debug.Log("Starting game...");
        // Loads the GameScene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainLobby");

            // Loads the player data to the GameData static class
            PlayerGameData playerData = SaveSystem.LoadPlayerData();
            GameData.LoadPlayerData(playerData);

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
        PlayerGameData playerData = new PlayerGameData(GameData.gems, GameData.unlockedSpellCards, GameData.unlockedRunes, GameData.spellCardType, GameData.spellCardTypeEnum, GameData.rune);
        SaveSystem.SavePlayerData(playerData);
        Debug.Log("Player data saved before quitting.");

        Application.Quit();
    }

    public void EndRun()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController_test>().FinishRun();

        // Sets its parent object to inactive
        GameObject parentObject = transform.parent.gameObject;
        parentObject.SetActive(false);
    }
}
