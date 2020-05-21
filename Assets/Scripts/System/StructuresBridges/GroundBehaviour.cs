using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBehaviour : MonoBehaviour
{
    public bool falling;
    
    public IEnumerator  Fall()
    {
        falling = true;
        transform.position += 0.5f*Vector3.up;
        Debug.Log("Ground is falling");
        yield return new WaitForEndOfFrame();

        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.useGravity = true;

    }

    private void OnTriggerStay(Collider other)
    {
        
        if (falling)
        {
            if (other.CompareTag("structure"))
            {
                Destroy(other.gameObject);
            }

            if (other.CompareTag("agent1"))
            {
                Debug.Log("Agent");
                Destroy(other.gameObject);
            }

        }


    }
}
