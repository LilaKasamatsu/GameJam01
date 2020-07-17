using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    
    
    [SerializeField] [Range(5f,25f)]  float maxZoom;
    [SerializeField] [Range(-20f,0.5f)] float minZoom;
    [SerializeField] [Range(2, 20)] float ZoomWithoutTilt;
    [SerializeField] float currentZoom;

    [SerializeField] float currentMouseTilt;
    [SerializeField] float maxMouseTilt;
    [SerializeField] float minMouseTilt;

    [SerializeField] [Range (0,500)] float rotationspeed;
    [SerializeField] [Range(0, 100)] float cameraZoomSpeed;
    [SerializeField] [Range(0.1f,1)] float cameraTiltToZoomRatio;

    [SerializeField] float cameraUpMovespeed;
    [SerializeField] float cameraSpeedThreshold;

    [SerializeField] float cameraMaxHeight;
    [SerializeField] float cameraMinHeight;

    public static CameraRig instance;
    public bool winningAnimation;
    [SerializeField] Vector3 CameraStartposition;
    Quaternion CameraRigStartingRotation;
    Animation anim;

    enum CameraMode { 
    normal,
    alternative,
    PressMousewheel,
    ScrollSteuerung,
    ReverseScrollSteuerung
    };
    [SerializeField] CameraMode cameraMode;

 
    Camera targetCamera;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        targetCamera = Camera.main;
        anim = GetComponent<Animation>();
        CameraRigStartingRotation = transform.rotation;
    }
    private void Update()
    {
        if (winningAnimation)
        {
            //StartCoroutine(WinningCameraAnimation());
        }
        else
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
                transform.rotation = Quaternion.RotateTowards(transform.rotation, (Quaternion.AngleAxis(45, targetCamera.transform.right) * transform.rotation), inputAxisMouseY*2);
                currentMouseTilt += inputAxisMouseY*2;

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
    
    public IEnumerator WinningCameraAnimation()
    {
        
        winningAnimation = true;
        while (targetCamera.transform.localPosition.y<169.7f || targetCamera.transform.localPosition.y>170.3f)
        {
            targetCamera.transform.localPosition = Vector3.Lerp(targetCamera.transform.localPosition, new Vector3(70, 170, 0), .6f*Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, CameraRigStartingRotation, 0.7f*Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, new Vector3(57, -20, 57), 0.7f*Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        anim.Play();
        yield return new WaitUntil(() => anim.isPlaying == false);
        while (targetCamera.transform.localPosition.y < 169.3f || targetCamera.transform.localPosition.y > 170.7f)
        {
            targetCamera.transform.localPosition = Vector3.Lerp(targetCamera.transform.localPosition, new Vector3(77, 109.2712f, 0), .6f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, CameraRigStartingRotation, 0.6f * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, new Vector3(57, -20, 57), 0.6f * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        currentMouseTilt = 0;
        currentZoom = 0;
        winningAnimation = false;

    }
    
}
