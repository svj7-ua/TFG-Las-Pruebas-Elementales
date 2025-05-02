using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DireccionController : MonoBehaviour
{


    public Transform target; // Object of reference to move arround
    public float rotationSpeed = 1.0f; // Speed of rotation
    public float circleRadius = 1.0f;
    public float elevationOffset = 0.0f;

    public float deviation = 0.0f;
    private int layerMask;

    //private float angle = 0.0f;

    private void Start()
    {
        //Sets the target position to the center of the screen
        layerMask =  LayerMask.GetMask("MouseLayer");


    }

    void Update()
    {

        if(Time.timeScale == 1)  // Handles rotation if the game is not paused
            HandleRotation();

    }

    private void HandleRotation(){
        RaycastHit _hit;
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, layerMask))
        {
            Vector3 targetPosition = new Vector3(_hit.point.x, transform.position.y, _hit.point.z);
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

}
