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

   

        /*
         * DEBUGGING COLOR
        if (Input.GetKey(KeyCode.Alpha1))
        {

            transform.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_Emission");
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColor);

            Color color = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.05f);




        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            transform.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColorSelect);

            Color color = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.35f);

        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bridge"))
        {
            Instantiate(effect1, this.transform.position, Quaternion.identity);
            //Instantiate(effect2, this.transform.position, Quaternion.identity);
            //Instantiate(effect3, this.transform.position, Quaternion.identity);


            Destroy(this.gameObject);
            ObjectiveSpawn.instance.collectedCubes += 1;

            if (cooldown >= .025)
            {
                GridArray.Instance.agentStack.agentAmountStructure += amountOfAgents;
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
            }
            else
            {
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
            }
        }

        if (other.gameObject.CompareTag("previewBridge"))
        {
            transform.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_Emission");
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColorSelect);

            Color color = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.35f);

        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("previewBridge"))
        {
            transform.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_Emission");
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColor);
  
            Color color = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.05f);

        }



    }
}
