using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    private float movementSpeed;
    public float sprintSpeed;

    Transform camTransform;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        camTransform = this.transform.GetChild(0);
    }

    void FixedUpdate()
    {
        Move();        
    }

    void Update()
    {
        CheckEscape();
    }

    private void Move()
    {
        // player movement - forward, backward, left, right
        float horizontal = Input.GetAxis("Horizontal") * movementSpeed;
        float vertical = Input.GetAxis("Vertical") * movementSpeed;
        characterController.Move((camTransform.right * horizontal + camTransform.forward * vertical) * Time.deltaTime); 
        
        if(Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = sprintSpeed;
        }
        else
        {
            movementSpeed = 30;
        }
        
    }

    private void CheckEscape()
    {
        if (Input.GetKeyDown("escape"))
        {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
