using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInformation : MonoBehaviour
{
    [SerializeField]
    private int level = 0; // Current level of the dungeon

    public int GetLevel()
    {
        return level; // Returns the current level
    }

    public void NextLevel()
    {
        level++; // Increments the level by 1
        Debug.Log("Level increased to: " + level); // Logs the new level
    }
}
