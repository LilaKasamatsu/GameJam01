﻿using System.Collections;
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
    bool spawned;
    bool falling;

    [SerializeField] AudioClip collected;
    [SerializeField] AudioClip dropping;
    private AudioSource audioSource;

    float cooldown;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        cooldown += Time.deltaTime;
        transform.localScale = Vector3.Lerp(new Vector3(6,6,6), new Vector3(0f, 0f, 0f), cooldown/ObjectiveSpawn.instance.objectiveCountdown);
        if (cooldown >= ObjectiveSpawn.instance.objectiveCountdown && falling==false)
        {
            StartCoroutine(Falling());
        }
        
    }

    IEnumerator Falling()
    {
        while (true)
        {
            falling = true;
            Debug.Log("Cube is falling");
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down * 10, ObjectiveSpawn.instance.objectiveFallingSpeed);
            yield return new WaitForEndOfFrame();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bridge") || other.gameObject.CompareTag("ground") );
        {
            if (other.gameObject.CompareTag("ground") && falling == true)
            {
                GroundBehaviour groundBehaviour = other.transform.GetComponentInChildren<GroundBehaviour>();
                
                    groundBehaviour.StartCoroutine(groundBehaviour.Fall());
                
            }
            //Instantiate(effect2, this.transform.position, Quaternion.identity);
            //Instantiate(effect3, this.transform.position, Quaternion.identity);
            

            if (!audioSource.isPlaying)
            {
                audioSource.clip = collected;
                audioSource.Play();
            }
            Destroy(this.gameObject);
            ObjectiveSpawn.instance.collectedCubes += 1;

            if (cooldown >= .025)
            {
                spawned = true;
                Instantiate(effect1, this.transform.position, Quaternion.identity);
                GridArray.Instance.agentStack.agentAmountStructure += amountOfAgents;
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
                // hier collectable eingesammelt sound einfügen (neues objekt instantiaten oder die brücke other abspielen lassen)
            }
            else if(spawned==false)
            {
                spawned = true;
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
            }
            Destroy(this.gameObject);
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
