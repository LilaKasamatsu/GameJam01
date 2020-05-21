using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveFallingDetection : MonoBehaviour
{
    [SerializeField] ObjectiveCubeBehavior parent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ground") && parent.falling && !parent.spawned)
        {
            GroundBehaviour groundBehaviour = other.transform.GetComponent<GroundBehaviour>();
            if (groundBehaviour != null)
            {
                groundBehaviour.StartCoroutine(groundBehaviour.Fall());
                parent.spawned = true;
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
                Destroy(parent.gameObject,0.5f);
                Destroy(this.gameObject);
            }
        }
    }
}
