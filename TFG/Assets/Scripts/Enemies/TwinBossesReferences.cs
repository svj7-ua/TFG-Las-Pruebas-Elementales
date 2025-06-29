using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBossesReferences : MonoBehaviour
{

    [SerializeField]
    private GameObject boss1;
    [SerializeField]
    private GameObject boss2;

    private GameObject bossRoom; // Reference to the Lord of Poison

    public GameObject GetBoss1()
    {
        return boss1;
    }

    public GameObject GetBoss2()
    {
        return boss2;
    }

    public bool IsBoss1Alive()
    {
        // Returns if the Lord of poison is active in the scene
        return boss1 != null && boss1.activeInHierarchy;
    }

    public bool IsBoss2Alive()
    {
        // Returns if the Lord of fire is active in the scene
        return boss2 != null && boss2.activeInHierarchy;
    }

    public void SetBossRoom(GameObject room)
    {
        // Sets the boss room reference
        bossRoom = room;
    }

    public GameObject GetBossRoom()
    {
        // Returns the boss room reference
        return bossRoom;
    }

    public bool OpenPortal()
    {
        if (bossRoom == null)
        {
            Debug.LogError("Boss room reference is null. Cannot open portal.");
            return false;
        }

        if (!IsBoss1Alive() && !IsBoss2Alive())
        {
            // If both bosses are alive, open the portal
            bossRoom.GetComponent<BossRoomManagerScript>().OpenNextLevelPortal();
            return true;
        }
        else
        {
            Debug.Log("One or both bosses are not alive. Cannot open portal until both are dead.");
            return false;
        }
    }


}
