using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class PlayerController_test : MonoBehaviour
{

    public float speed;
    public float groundDistance;

    public LayerMask groundMask;
    public Rigidbody rigidBody;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 castPosition = transform.position;
        castPosition.y += 1;

        if(Physics.Raycast(castPosition, -transform.up, out hit, Mathf.Infinity, groundMask))
        {
            if(hit.collider != null){
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDistance;
                transform.position = movePos;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(x, 0, z);
            rigidBody.velocity = moveDirection * speed;

            if(x < 0){
                spriteRenderer.flipX = true;
            } else if(x > 0){
                spriteRenderer.flipX = false;
            }
        }
    }
}
