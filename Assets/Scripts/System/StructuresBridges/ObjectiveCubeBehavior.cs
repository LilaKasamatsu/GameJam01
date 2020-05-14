using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCubeBehavior : MonoBehaviour
{
    
    [HideInInspector] public int amountOfAgents;
    [SerializeField] GameObject effect1;
    [SerializeField] GameObject effect2;
    [SerializeField] GameObject effect3;


    float cooldown;
    private void Update()
    {
        cooldown += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bridge"))
        {
            Instantiate(effect1, this.transform.position, Quaternion.identity);
            //Instantiate(effect2, this.transform.position, Quaternion.identity);
            //Instantiate(effect3, this.transform.position, Quaternion.identity);


            Destroy(this.gameObject);

            if (cooldown >= .25)
            {
                GridArray.Instance.agentStack.agentAmountStructure += amountOfAgents;
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
            }
            else
            {
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(2));
            }
        }
  
    }
}
