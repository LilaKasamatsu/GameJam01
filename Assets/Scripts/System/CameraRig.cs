using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    
    
    [SerializeField] [Range(5f,25f)]  float maxZoom;
    [SerializeField] [Range(0f,0.5f)] float minZoom;
    [SerializeField] [Range(2, 20)] float ZoomWithoutTilt;
    float currentZoom;

    float currentMouseTilt;
    [SerializeField] float maxMouseTilt;
    [SerializeField] float minMouseTilt;

    [SerializeField] [Range (0,500)] float rotationspeed;
    [SerializeField] [Range(0, 100)] float cameraZoomSpeed;
    [SerializeField] [Range(0.1f,1)] float cameraTiltToZoomRatio;

    Camera targetCamera;

    private void Start()
    {
        targetCamera = Camera.main;

    }
    private void Update()
    {
        CameraMovement();

    }

    private void CameraMovement()
    {
        if (Input.GetMouseButton(1))
        {
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"),-0.4f,0.4f) * rotationspeed/20;
           

            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX * Time.deltaTime, Vector3.up) * transform.rotation;
            if (inputAxisMouseY > 0.002 && currentMouseTilt <= maxMouseTilt || inputAxisMouseY < -0.002 && currentMouseTilt >= minMouseTilt)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY );
                currentMouseTilt += inputAxisMouseY;

            }

        }
        float inputAxisScroll = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -0.1f, 0.1f);
        if (inputAxisScroll > 0.02 && currentZoom <= maxZoom || inputAxisScroll < -0.02 && currentZoom >= minZoom - ZoomWithoutTilt)
        {

            Vector3 cameraMoveDirection = (targetCamera.transform.position - transform.position).normalized;

            targetCamera.transform.position -= cameraMoveDirection * inputAxisScroll * cameraZoomSpeed;
            if (currentZoom >= minZoom)
            {
                targetCamera.transform.rotation = Quaternion.AngleAxis(-inputAxisScroll * cameraZoomSpeed * cameraTiltToZoomRatio, targetCamera.transform.right) * targetCamera.transform.rotation;
            }
            currentZoom += inputAxisScroll * Mathf.Sqrt(cameraZoomSpeed);
        }
    }
}
