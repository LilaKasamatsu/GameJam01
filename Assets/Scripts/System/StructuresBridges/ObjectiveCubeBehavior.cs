using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCubeBehavior : MonoBehaviour
{
    
    [HideInInspector] public int amountOfAgents;
    [SerializeField] GameObject effect1;
    [SerializeField] GameObject effect2;
    [SerializeField] GameObject effect3;
    [SerializeField] GameObject selecterBridge;
    [SerializeField] Color sphereColor;
    [SerializeField] Color sphereColorSelect;

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

        if (other.gameObject.CompareTag("previewBridge"))
        {
            //transform.GetChild(1).GetComponent<Renderer>().material.color = sphereColorSelect;
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColorSelect);

            Color color = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);

        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("previewBridge"))
        {
            //transform.GetChild(1).GetComponent<Renderer>().material.color = sphereColor;
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColor);
  
            Color color = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.05f);

        }



    }
}
