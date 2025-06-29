using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorTriggerNotifier : MonoBehaviour
{
    public event Action<Collider> OnTriggerActivated; // Event to notify when the trigger is activated

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔔 Trigger activado por {other.gameObject.name}");
        OnTriggerActivated?.Invoke(other); // Calls the event if there are subscribers
    }
}