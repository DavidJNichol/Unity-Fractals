using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // horizontal rotation speed
    public float horizontalSpeed = 1f;
    // vertical rotation speed
    public float verticalSpeed = 1f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Camera cam;
    Transform lightTransform;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;

        lightTransform = transform.GetChild(0).transform;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        lightTransform.rotation = this.transform.rotation;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    }
}
