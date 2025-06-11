using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRun : MonoBehaviour
{
    public void BackToLobby()
    {

        // Loads the MainLobby scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainLobby");
            if(Time.timeScale != 1f)
            {
                Time.timeScale = 1f; // Reset time scale to normal if it was paused
            }
        }
        else
        {
            Debug.LogError("EndRun: Not in GameScene. Cannot load MainLobby.");
        }

    }
}
