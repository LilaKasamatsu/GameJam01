using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    float inputAxisMouse;
    float inputAxisScroll;
    [SerializeField] [Range(10f,25f)]  float maxZoom;
    [SerializeField] [Range(0f,0.5f)] float minZoom;
    [SerializeField] [Range(2, 5)] float ZoomWithoutTilt;
    [SerializeField] float currentZoom;

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
        if (Input.GetMouseButton(1))
        {
            inputAxisMouse = Input.GetAxis("Mouse X")*rotationspeed;

            transform.rotation = Quaternion.AngleAxis(inputAxisMouse*Time.deltaTime, transform.up)*transform.rotation;

        }
        inputAxisScroll = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"),-0.1f,0.1f);
        if (inputAxisScroll > 0.02&&currentZoom<=maxZoom || inputAxisScroll < -0.02 && currentZoom>=minZoom-ZoomWithoutTilt)
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
