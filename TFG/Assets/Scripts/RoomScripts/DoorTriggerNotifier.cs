using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorTriggerNotifier : MonoBehaviour
{
    public event Action<Collider> OnTriggerActivated; // Evento sin parámetros, solo notifica que se activó

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔔 Trigger activado por {other.gameObject.name}");
        OnTriggerActivated?.Invoke(other); // Llama al evento si hay suscriptores
    }
}