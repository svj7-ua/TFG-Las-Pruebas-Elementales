using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f; // Ángulo de apertura
    public float closingDuration = 0.1f; // Velocidad de apertura/cierre
    public float openingDuration = 0.3f; // Velocidad de apertura/cierre
    private bool isOpen = true;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        StartCoroutine(OpenDoor());
        
    }

    IEnumerator OpenDoor()
    {
        yield return null;          // Wait for 2 frames to ensure the door starts in the open position
        yield return null;
        transform.rotation = openRotation;
    }

    public void ToggleDoor()
    {
        StopAllCoroutines();
        StartCoroutine(RotateDoor(isOpen ? closedRotation : openRotation, isOpen ? closingDuration : openingDuration));
        isOpen = !isOpen;
    }

    IEnumerator RotateDoor(Quaternion targetRotation, float duration)
    {

        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // normaliza entre 0 y 1
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation; // asegurar que llega al final exacto

    }
}
