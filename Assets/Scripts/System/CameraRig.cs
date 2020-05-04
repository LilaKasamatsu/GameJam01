using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    float inputAxisMouse;
    float inputAxisScroll;
    [SerializeField] [Range(80,500)] float rotationspeed;
    [SerializeField] [Range(500, 1000)] float cameraZoomSpeed;
    [SerializeField] [Range(10, 1000)] float cameraZoomTilt;
    Camera targetCamera;

    private void Start()
    {
        targetCamera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            inputAxisMouse = Input.GetAxis("Mouse X")*rotationspeed;

            transform.rotation = Quaternion.AngleAxis(inputAxisMouse*Time.deltaTime, transform.up)*transform.rotation;
        }
        inputAxisScroll = Input.GetAxis("Mouse ScrollWheel");
        if (inputAxisScroll > 0.02 || inputAxisScroll < -0.02)
        {
            Vector3 cameraMoveDirection = (targetCamera.transform.position - transform.position).normalized;
            targetCamera.transform.position -= cameraMoveDirection * inputAxisScroll * Time.deltaTime * cameraZoomSpeed;
            targetCamera.transform.rotation = Quaternion.AngleAxis(-inputAxisScroll * Time.deltaTime * cameraZoomTilt, targetCamera.transform.right)*targetCamera.transform.rotation;
        }
    }

}
