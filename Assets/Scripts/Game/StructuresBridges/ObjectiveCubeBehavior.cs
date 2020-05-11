using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCubeBehavior : MonoBehaviour
{
    AgentStack agentStack;

    // Start is called before the first frame update
    void Start()
    {
        agentStack = GridArray.Instance.agentStack;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bridge"))
        {
            Destroy(this.gameObject);
            agentStack.agentAmount += 5;
        }
    }
}
