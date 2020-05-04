using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    float inputAxisMouse;
    [SerializeField] [Range(80,500)] float Rotationspeed;

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            inputAxisMouse = Input.GetAxis("Mouse X")*Rotationspeed;

            transform.rotation = Quaternion.AngleAxis(inputAxisMouse*Time.deltaTime, transform.up)*transform.rotation;
        }
    }

}
