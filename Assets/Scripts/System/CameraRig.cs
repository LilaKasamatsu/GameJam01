using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    float inputAxisMouse;

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            inputAxisMouse = Input.GetAxis("Mouse X");

            transform.rotation = Quaternion.AngleAxis(inputAxisMouse*Time.deltaTime, transform.up)*transform.rotation;
        }
    }

}
