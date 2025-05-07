using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTriggersScript : MonoBehaviour
{

    [SerializeField] private bool isOpen = true;
    [SerializeField] private bool isClosed = false;

    [SerializeField] private List<GameObject> doorTriggers = new List<GameObject>();



    private void OnTriggerEnter(Collider other)
    {
        //Logs the player entering the room
        Debug.Log("Player entered the room");

        if (other.CompareTag("Player"))
        {
            if(isOpen){
                
                isOpen = false;
                isClosed = true;

                // After closing the door, it will be locked ultil player defeats enemies in the room, so we need to disable the triggers
                foreach(GameObject trigger in doorTriggers){
                    trigger.SetActive(false);
                }

                // Closes the doors.

                // TODO: SPAWN ENEMIES
            }

        }
    }


}
