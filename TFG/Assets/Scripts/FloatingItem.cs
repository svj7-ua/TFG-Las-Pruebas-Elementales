using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public AnimationCurve myCurve;
   
    void Update()
    {
        float time = Mathf.PingPong(Time.time, myCurve.length);
        transform.position = new Vector3(transform.position.x, myCurve.Evaluate(time) + 1.0f, transform.position.z);
    }
}
