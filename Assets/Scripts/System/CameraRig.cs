using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    
    
    [SerializeField] [Range(5f,25f)]  float maxZoom;
    [SerializeField] [Range(-20f,0.5f)] float minZoom;
    [SerializeField] [Range(2, 20)] float ZoomWithoutTilt;
    float currentZoom;

    float currentMouseTilt;
    [SerializeField] float maxMouseTilt;
    [SerializeField] float minMouseTilt;

    [SerializeField] [Range (0,500)] float rotationspeed;
    [SerializeField] [Range(0, 100)] float cameraZoomSpeed;
    [SerializeField] [Range(0.1f,1)] float cameraTiltToZoomRatio;

    [SerializeField] float cameraUpMovespeed;
    [SerializeField] float cameraSpeedThreshold;

    [SerializeField] float cameraMaxHeight;
    [SerializeField] float cameraMinHeight;

    enum CameraMode { 
    normal,
    alternative,
    PressMousewheel,
    ScrollSteuerung,
    ReverseScrollSteuerung
    };
    [SerializeField] CameraMode cameraMode;

 
    Camera targetCamera;

    private void Start()
    {
        targetCamera = Camera.main;

    }
    private void Update()
    {
        switch (cameraMode)
        {
            case CameraMode.normal:
                CameraMovement();
                Zoom();
                break;
            case CameraMode.alternative:
                AlternativeCameraMove();
                Zoom();
                break;
            case CameraMode.PressMousewheel:
                PressMousewheelCameraMovement();
                Zoom();
                break;
            case CameraMode.ScrollSteuerung:
                MousewheelCameraMovement();
                break;
            case CameraMode.ReverseScrollSteuerung:
                ReverseScrollCameraMovement();
                break;
        }
        

    }

    private void AlternativeCameraMove()
    {
        if(Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * cameraUpMovespeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (transform.position.y > cameraMinHeight && inputAxisMouseY < 0 || transform.position.y < cameraMaxHeight && inputAxisMouseY > 0)
            {
                transform.position += Vector3.up  * Mathf.Clamp(inputAxisMouseY, -cameraSpeedThreshold, cameraSpeedThreshold);
            }

        }
        else if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * rotationspeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (inputAxisMouseY > 0.002 && currentMouseTilt <= maxMouseTilt || inputAxisMouseY < -0.002 && currentMouseTilt >= minMouseTilt)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY);
                currentMouseTilt += inputAxisMouseY;

            }
            

        }
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }



    }
    private void PressMousewheelCameraMovement()
    {
        if (Input.GetMouseButton(2))
        {
            Cursor.visible = false;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * cameraUpMovespeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (transform.position.y > cameraMinHeight && inputAxisMouseY < 0 || transform.position.y < cameraMaxHeight && inputAxisMouseY > 0)
            {
                transform.position += Vector3.up * Mathf.Clamp(inputAxisMouseY, -cameraSpeedThreshold, cameraSpeedThreshold);
            }

        }
        else if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * rotationspeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (inputAxisMouseY > 0.002 && currentMouseTilt <= maxMouseTilt || inputAxisMouseY < -0.002 && currentMouseTilt >= minMouseTilt)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY);
                currentMouseTilt += inputAxisMouseY;

            }


        }
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            Cursor.visible = true;
        }



    }
    private void MousewheelCameraMovement()
    {

        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * rotationspeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (inputAxisMouseY > 0.002 && currentMouseTilt <= maxMouseTilt || inputAxisMouseY < -0.002 && currentMouseTilt >= minMouseTilt)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY);
                currentMouseTilt += inputAxisMouseY;

            }
            
            float inputAxisScroll = Input.GetAxis("Mouse ScrollWheel")*cameraUpMovespeed*10;
            if (transform.position.y > cameraMinHeight && inputAxisScroll < 0 || transform.position.y < cameraMaxHeight && inputAxisScroll > 0)
            {
                    transform.position += Vector3.up * Mathf.Clamp(inputAxisScroll, -cameraSpeedThreshold, cameraSpeedThreshold);
            }
        }
        else Zoom();
        
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            Cursor.visible = true;
        }



    }

    private void ReverseScrollCameraMovement()
    {
        float inputAxisScroll = Input.GetAxis("Mouse ScrollWheel") * cameraUpMovespeed * 10;
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * rotationspeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (inputAxisMouseY > 0.002 && currentMouseTilt <= maxMouseTilt || inputAxisMouseY < -0.002 && currentMouseTilt >= minMouseTilt)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY);
                currentMouseTilt += inputAxisMouseY;

            }

            Zoom();
        }
        else if (transform.position.y > cameraMinHeight && inputAxisScroll < 0 || transform.position.y < cameraMaxHeight && inputAxisScroll > 0)
        {
            transform.position += Vector3.up * Mathf.Clamp(inputAxisScroll, -cameraSpeedThreshold, cameraSpeedThreshold);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            Cursor.visible = true;
        }



    }

    private void CameraMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(targetCamera.ScreenPointToRay(Input.mousePosition)))
            {
                StartCoroutine(MoveTheCamera(Input.mousePosition.y));
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
        }

        if (Input.GetMouseButton(1))
        {
            float inputAxisMouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -0.4f, 0.4f) * rotationspeed;
            float inputAxisMouseX = Input.GetAxis("Mouse X") * rotationspeed;
            transform.rotation = Quaternion.AngleAxis(inputAxisMouseX, Vector3.up) * transform.rotation;

            if (inputAxisMouseY > 0.002 && currentMouseTilt <= maxMouseTilt || inputAxisMouseY < -0.002 && currentMouseTilt >= minMouseTilt)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY);
                currentMouseTilt += inputAxisMouseY;

            }

        }
        
    }

    private void Zoom()
    {
        float inputAxisScroll = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -0.1f, 0.1f);
        if (inputAxisScroll > 0.02 && currentZoom <= maxZoom || inputAxisScroll < -0.02 && currentZoom >= minZoom - ZoomWithoutTilt)
        {

            Vector3 cameraMoveDirection = (targetCamera.transform.position - transform.position).normalized;

            targetCamera.transform.position -= cameraMoveDirection * inputAxisScroll * cameraZoomSpeed;
            /*
            if (currentZoom >= minZoom)
            {
                targetCamera.transform.rotation = Quaternion.AngleAxis(-inputAxisScroll * cameraZoomSpeed * cameraTiltToZoomRatio, targetCamera.transform.right) * targetCamera.transform.rotation;
            }
            */
            currentZoom += inputAxisScroll * Mathf.Sqrt(cameraZoomSpeed);
        }
    }

    IEnumerator MoveTheCamera( float screenCentre)
    {
        while (true)
        {
          
            float MouseDistance = Input.mousePosition.y - screenCentre;

            if (transform.position.y > cameraMinHeight && MouseDistance < 0 || transform.position.y < cameraMaxHeight && MouseDistance > 0)
            {
                transform.position += transform.up * cameraUpMovespeed * Mathf.Clamp(MouseDistance, -cameraSpeedThreshold, cameraSpeedThreshold);
            }
            
            
            yield return new WaitForEndOfFrame();
            
        }
    }

    private void CameraUpDown()
    {
        if (Input.GetMouseButton(2)) 
        {
            float screenCentre = Screen.height / 2;
            float MouseDistance = Input.mousePosition.y - screenCentre;

            if (MouseDistance >= cameraSpeedThreshold)
            {
                targetCamera.transform.position += transform.up * cameraUpMovespeed*-Time.deltaTime;
            }
            if (MouseDistance <= -cameraSpeedThreshold)
            {
                targetCamera.transform.position -= transform.up * cameraUpMovespeed*-Time.deltaTime;
            }


        }
    }
}
