using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f; // Ángulo de apertura
    public float speed = 2f; // Velocidad de apertura/cierre
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
        StartCoroutine(RotateDoor(isOpen ? closedRotation : openRotation));
        isOpen = !isOpen;
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }
        transform.rotation = targetRotation;
        

    }
}
