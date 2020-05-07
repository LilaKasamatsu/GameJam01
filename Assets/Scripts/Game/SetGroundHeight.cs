using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGroundHeight : MonoBehaviour
{
   
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

 
}
