﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AgentStructure : MonoBehaviour
{
    [SerializeField] GameObject structure;
    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] int maxBuildings;
    [SerializeField] GameObject spawnAgent;



    private GameObject ground;

    public Grid grid;
    Camera cam;
    public NavMeshAgent agent;

    public bool isActive = false;
    bool canBuild = false;
    bool canSpawn = false;
    private int cellSize;



    public float pointRadius;
    public float maxDestinationDistance;

    Vector3 agentMoveLocation;
    List<Vector3> orientPositions;

    GridList[,] gridArray;

  
    void Start()
    {

        cam = Camera.main;
        ground = GameObject.FindGameObjectWithTag("ground");

        cellSize = GridArray.Instance.cellSize;
        gridArray = GridArray.Instance.gridArray;
        
        // DIESER CODE IST DER NEUE

        //Create orientPosition List based on the grid
        //Adds all existing build-orientation points currently on the scene to the list
        // "state == true" means, that that location has a structure to orient on.



        StartCoroutine(MoveTimer());


    }



    void FixedUpdate()
    {
        grid = SpawnSettings.Instance.grid;

        
        if (isActive == true)
        {
            transform.GetChild(0).gameObject.SetActive(true);

        }

        if (isActive == false)
        {
            SpawnActiveAgent();


        }
    }



    IEnumerator MoveTimer()
    {
        //Looping and delaying their walk cycle 
        while (isActive == true)
        {
            
            BuildStructure();

            yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));


            int minX = Mathf.RoundToInt((transform.position.x) / cellSize - maxDestinationDistance);
            int maxX = Mathf.RoundToInt((transform.position.x) / cellSize + maxDestinationDistance);

            int minZ = Mathf.RoundToInt((transform.position.z) / cellSize - maxDestinationDistance);
            int maxZ = Mathf.RoundToInt((transform.position.z) / cellSize + maxDestinationDistance);

            //Search Grid for structures/main points
            //The list orientPositions saves the Vector3 of all structures, that the agent has to orient on
            orientPositions = new List<Vector3>();
            for (int x = minX; x >= minX && x <= maxX && x < GridArray.Instance.arrayX && x > 0; x++)
            {
                for (int z = minZ; z >= minZ && z <= maxZ && z < GridArray.Instance.arrayZ && z > 0; z++)
                {

                    if (gridArray[x, z].structureAmount  > 0)
                    {

                        //return that this position has a strucutre to orient on.
                        orientPositions.Add(new Vector3(x * cellSize, transform.position.y, z * cellSize));

                    }
                }
            }

            //"GetClosestTarget" then compares all of those Vector3 inside the maximum range "maxDestinationDistance" and finds the closest
            Vector3 closestMainPoint = GridArray.Instance.GetClosestTarget(orientPositions, transform.position);

            float closestX = closestMainPoint.x + Random.Range(-pointRadius, pointRadius);
            float closestZ = closestMainPoint.z + Random.Range(-pointRadius, pointRadius);

         

            agentMoveLocation = new Vector3(closestX, transform.position.y, closestZ);

            canBuild = true;
            agent.SetDestination(agentMoveLocation);

        }



    }

    private void BuildStructure()
    {
        gridArray = GridArray.Instance.gridArray;
        if (!agent.hasPath && canBuild == true)
        {
            int arrayPosX = Mathf.RoundToInt(transform.position.x / cellSize) ;
            int arrayPosZ = Mathf.RoundToInt(transform.position.z / cellSize);


            Vector3 buildLocation = new Vector3(arrayPosX * cellSize, structure.transform.localScale.y/2 + 2* gridArray[arrayPosX, arrayPosZ].structureAmount, arrayPosZ * cellSize);
            canBuild = false;

          
     
            if (gridArray[arrayPosX, arrayPosZ].pointAmount <= 0  && gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
            {

                GameObject builtStructure = Instantiate(structure, buildLocation, Quaternion.identity) as GameObject;
                gridArray[arrayPosX, arrayPosZ].structureObjects.Add(builtStructure);
                           

                gridArray[arrayPosX, arrayPosZ].structureAmount += 1;

            }
        }
    }

    private bool IsOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void SpawnActiveAgent()
    {

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 20.0f;
        mousePos.y += 50.0f;

        Vector3 objectPos = cam.ScreenToWorldPoint(mousePos);
        transform.position = objectPos;


        if (Input.GetMouseButton(1))
        {
            SpawnSettings.Instance.spawnMode = false;
            Destroy(this.gameObject);

        }

        if (Input.GetMouseButtonUp(0))
        {
            canSpawn = true;
        }

        if (Input.GetMouseButton(0) == true && !IsOverUi())
        {

            SpawnSettings.Instance.PlaceAgent(spawnAgent);
        }
    }

}

