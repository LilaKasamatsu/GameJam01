﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AgentFoundation : MonoBehaviour
{
    [SerializeField] GameObject foundation;
    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] int maxBuildings;
    [SerializeField] GameObject spawnAgent;


    //Der Bereich wird um diesen Wert erhöt, wenn sie kein nahes leeres Feld finden.
    [SerializeField] float searchIncrease = 0.5f;

    //Maximaler vergrößerter Bereich
    [SerializeField] float maxSearch = 5f;


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
    AgentStack agentStack;

    void Start()
    {
        cam = Camera.main;
        ground = GameObject.FindGameObjectWithTag("ground");
        
        cellSize = GridArray.Instance.cellSize;
        gridArray = GridArray.Instance.gridArray;
        agentStack = GridArray.Instance.agentStack;
        
        // DIESER CODE IST DER NEUE

        //Create orientPosition List based on the grid
        //Adds all existing build-orientation points currently on the scene to the list
        // "state == true" means, that that location has a structure to orient on.

 
 
        StartCoroutine(MoveTimer());


    }

    void FixedUpdate()
    {
        grid = SpawnSettings.Instance.grid;

        if (isActive == true && transform.GetChild(0).gameObject.activeSelf)
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

            BuildFoundation();
            
            yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));


            //int minX = Mathf.RoundToInt( (transform.position.x ) / cellSize - maxDestinationDistance) ;
            //int maxX = Mathf.RoundToInt( (transform.position.x ) / cellSize + maxDestinationDistance) ;
            //int minZ = Mathf.RoundToInt( (transform.position.z ) / cellSize - maxDestinationDistance) ;
            //int maxZ = Mathf.RoundToInt( (transform.position.z ) / cellSize + maxDestinationDistance) ;

            
            orientPositions= GridArray.Instance.searchForFoundations(transform.position, pointRadius);
            

            //"GetClosestTarget" then compares all of those Vector3 inside the maximum range "maxDestinationDistance" and finds the closest
            Vector3 closestMainPoint = GridArray.Instance.GetClosestTarget(orientPositions, transform.position);

            float closestX = closestMainPoint.x + Random.Range(-pointRadius, pointRadius);
            float closestZ = closestMainPoint.z + Random.Range(-pointRadius, pointRadius);

            int gridX = GridArray.Instance.NumToGrid(closestX);
            int gridZ = GridArray.Instance.NumToGrid(closestZ);

            float counter = searchIncrease;


            while (counter < maxSearch && (GridArray.Instance.CheckArrayBounds(gridX, gridZ) == false || (GridArray.Instance.CheckArrayBounds(gridX, gridZ) && GridArray.Instance.gridArray[gridX, gridZ].foundationAmount != 0)))
            {


                closestX = closestMainPoint.x + Random.Range(-pointRadius - counter, pointRadius + counter);
                closestZ = closestMainPoint.z + Random.Range(-pointRadius - counter, pointRadius + counter);
                counter += searchIncrease;

                gridX = GridArray.Instance.NumToGrid(closestX);
                gridZ = GridArray.Instance.NumToGrid(closestZ);

        
            }

    


            agentMoveLocation = new Vector3(closestX, transform.position.y, closestZ);
                  
            canBuild = true;                        
            agent.SetDestination(agentMoveLocation);

        }

        

    }

    IEnumerator SearchDestination()
    {
        yield return new WaitForFixedUpdate();


    }
    private void BuildFoundation()
    {
        gridArray = GridArray.Instance.gridArray;
        if (!agent.hasPath && canBuild == true)
        {
            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / cellSize) * cellSize, foundation.transform.localScale.y / 2, Mathf.Round(transform.position.z / cellSize) * cellSize);
            canBuild = false;

            int arrayPosX = GridArray.Instance.NumToGrid(buildLocation.x);
            int arrayPosZ = GridArray.Instance.NumToGrid(buildLocation.z);

            if( GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
            {
                if (gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount <= 0)
                {
                    GameObject builtStructure = Instantiate(foundation, buildLocation, Quaternion.identity) as GameObject;
                    
                    gridArray[arrayPosX, arrayPosZ].foundationObject = builtStructure;


                    gridArray[arrayPosX, arrayPosZ].foundationAmount += 1;

                }
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

        if (Input.GetMouseButton(0) == true  && !IsOverUi() )
        {
            SpawnSettings.Instance.PlaceAgent(spawnAgent);
        }
        
        
    }

}
