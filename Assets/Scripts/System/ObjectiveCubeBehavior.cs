using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCubeBehavior : MonoBehaviour
{
    
    [HideInInspector] public int amountOfAgents;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bridge"))
        {
            Destroy(this.gameObject);
            GridArray.Instance.agentStack.agentAmount += amountOfAgents;
            ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(2));
        }
  
    }
}
