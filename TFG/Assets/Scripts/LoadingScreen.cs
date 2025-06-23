using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{

    public GameObject loadingScreen; // Reference to the loading screen GameObject
    public Slider loadingBar; // Reference to the loading bar Slider component

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex)); // Start the coroutine to load the scene asynchronously
    }


    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingScreen.SetActive(true); // Activate the loading screen
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Normalize the progress to a value between 0 and 1
            Debug.Log("Loading progress: " + (progress * 100) + "%"); // Log the loading progress
            loadingBar.value = progress; // Update the loading bar value

            yield return null; // Wait for the next frame
        }

    }

}
