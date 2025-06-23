using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerGameData playerData = SaveSystem.LoadPlayerData();
        GameData.LoadPlayerData(playerData);

        //GameData.gems = 9000;               // COMENT AFTER TESTING
        SetScreen();                     // Set the screen resolution and display
        SetVolume();                     // Set the audio volume

    }

    void SetScreen()
    {
        int target = GameData.displayIndex;

        if (target > 0 && target < Display.displays.Length)
            Display.displays[target].Activate();

        Camera.main.targetDisplay = target;
        Screen.SetResolution(Display.displays[target].systemWidth, Display.displays[target].systemHeight, true);
    }

    void SetVolume()
    {

        AudioListener.volume = GameData.volume; // Set the AudioListener volume to the loaded value

    }
}
