using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawHitboxMovementScript : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve myCurve;

    private float timer = 0f;
    private float previousValue = 0f;
    private bool hasFinished = false;

    void Update()
    {
        if (hasFinished) return;

        timer += Time.deltaTime;

        float curveLength = myCurve.keys[myCurve.length - 1].time;

        if (timer >= curveLength)
        {
            timer = curveLength;
            hasFinished = true;
        }

        float currentValue = myCurve.Evaluate(timer);
        float delta = currentValue - previousValue;
        previousValue = currentValue;

        // Mueve la hitbox hacia adelante en la dirección local del objeto
        transform.position += transform.forward * delta;
        // subtract 90 degrees from the rotation to make it face the right direction
        
    }
}