﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AgentBranch : MonoBehaviour
{
    [SerializeField] GameObject structure;
    //[SerializeField] GameObject structureSquare;
    [SerializeField] GameObject branch;
    [SerializeField] GameObject branch2;
    [SerializeField] GameObject branch3;
    [SerializeField] GameObject branch4;
    [SerializeField] GameObject branch5;
    [SerializeField] GameObject branch6;

    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] GameObject spawnAgent;

    [SerializeField] int minBranchHeight = 4;
    [SerializeField] int branchChance = 40;





    public int structuresLifetime = 10;
    int structuresPlaced = 0;
    

    public Grid grid;
    Camera cam;
    public NavMeshAgent agent;

    public bool hasSignal = false;
    public bool isActive = false;
    bool canBuild = false;
    private int cellSize;
    private int cellY;
    Vector3 buildLocation;
    float buildRotation;
    GameObject builtStructure;
    int gridY;

    public float pointRadius;
    public float maxDestinationDistance;

    Vector3 agentMoveLocation;
    List<Vector3> orientPositions;

    GridList[,] gridArray;
    AgentStack agentStack;


    void Start()
    {
   

        cam = Camera.main;

        cellSize = GridArray.Instance.cellSize;
        cellY = GridArray.Instance.cellY;
        gridArray = GridArray.Instance.gridArray;
        agentStack = GridArray.Instance.agentStack;


        StartCoroutine(MoveTimer());
  
    }


    IEnumerator RetireAgent()
    {
        agentStack.agentAmount += 1;
        agentStack.agentBranch -= 1;
        yield return new WaitForSeconds(Random.Range(0, 3));
        Destroy(this.gameObject);


    }

    public void ReceiveSignal(Vector3 position, float destMin)
    {
        transform.GetChild(0).gameObject.GetComponent<SetAgentHeight>().GoToSignal(position, destMin);
    }

    void FixedUpdate()
    {
        grid = SpawnSettings.Instance.grid;

        if (hasSignal == true && agent.hasPath)
        {
            agent.ResetPath();
        }

        if (isActive == true)
        {
            transform.GetChild(0).gameObject.SetActive(true);

        }

        if (isActive == false)
        {
            SpawnActiveAgent();

        }

        if (structuresPlaced >= structuresLifetime)
        {
            StartCoroutine("RetireAgent");
        }
    }

          
    IEnumerator MoveTimer()
    {
        //Looping and delaying their walk cycle 
        while (isActive == true && hasSignal == false)
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

                    if (gridArray[x, z].sizeY > 0)
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
            int arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
            int arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);

            int newArrayPosX = arrayPosX;
            int newArrayPosZ = arrayPosZ;


            canBuild = false;


            if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
            {
                //buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].sizeY + 1, GridArray.Instance.RoundToGrid(transform.position.z));


                if (gridArray[arrayPosX, arrayPosZ] != null && gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].bridge <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                {

                    gridY = Mathf.RoundToInt(gridArray[arrayPosX, arrayPosZ].sizeY);

                    int randomValue = Random.Range(0, 100);
                    Vector3 size = structure.transform.localScale;
                    int selfChance = branchChance / 4;
                    bool hasBuilt = false;

                    //Check if can branch
                    if (gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched == false)
                    {
                        //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].y
                        if (gridY >= minBranchHeight)
                        {
                            if (randomValue >= 0 && randomValue < selfChance * 0.25f)
                            {
                                if (gridArray[newArrayPosX - 1, newArrayPosZ].gridStructures[gridY].y == 0 && gridArray[newArrayPosX - 1, newArrayPosZ].gridStructures[gridY - 1].y == 0)
                                {

                                    newArrayPosX = arrayPosX - 1;    
                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    GridList target = gridArray[arrayPosX, arrayPosZ];
                                    //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    target.branchedStructures = gridArray[arrayPosX, arrayPosZ].sizeY;
                                    isBranch = true;
                                    hasBuilt = true;

                                    buildRotation = 0;
                                    if (target.warningSystemEngaged && target.sizeY-target.branchedStructures>=DestructionManager.instance.heightLimit)
                                    {
                                        Transform targetTransform=target.windParticle.transform; 
                                        targetTransform.position = new Vector3(targetTransform.position.x, buildLocation.y + DestructionManager.instance.heightLimit, targetTransform.position.z);
                                    }
                                    else if(target.warningSystemEngaged)
                                    {
                                        Destroy(target.windParticle);
                                        target.warningSystemEngaged = false;
                                    }
                                }
                            }

                            if (randomValue >= selfChance * 0.25f && randomValue < selfChance * 0.5f)
                            {
                                if (gridArray[newArrayPosX + 1, newArrayPosZ].gridStructures[gridY].y == 0 && gridArray[newArrayPosX + 1, newArrayPosZ].gridStructures[gridY - 1].y == 0)
                                {
                                    newArrayPosX = arrayPosX + 1;       
                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    GridList target = gridArray[arrayPosX, arrayPosZ];
                                    //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    target.branchedStructures = gridArray[arrayPosX, arrayPosZ].sizeY;
                                    isBranch = true;

                                    hasBuilt = true;
                                    buildRotation = 180;
                                    if (target.warningSystemEngaged && target.sizeY - target.branchedStructures >= DestructionManager.instance.heightLimit)
                                    {
                                        Transform targetTransform = target.windParticle.transform;
                                        targetTransform.position = new Vector3(targetTransform.position.x, buildLocation.y + DestructionManager.instance.heightLimit, targetTransform.position.z);
                                    }
                                    else if (target.warningSystemEngaged)
                                    {
                                        Destroy(target.windParticle);
                                        target.warningSystemEngaged = false;
                                    }
                                }
                            }

                            if (randomValue >= selfChance * 0.5f && randomValue < selfChance * 0.75f)
                            {
                                if (gridArray[newArrayPosX, newArrayPosZ - 1].gridStructures[gridY].y == 0 && gridArray[newArrayPosX, newArrayPosZ - 1].gridStructures[gridY - 1].y == 0)
                                {
                                    newArrayPosZ = arrayPosZ - 1;                         
                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    GridList target = gridArray[arrayPosX, arrayPosZ];
                                    //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    target.branchedStructures = gridArray[arrayPosX, arrayPosZ].sizeY;
                                    isBranch = true;

                                    hasBuilt = true;
                                    buildRotation = 270;
                                    if (target.warningSystemEngaged && target.sizeY - target.branchedStructures >= DestructionManager.instance.heightLimit)
                                    {
                                        Transform targetTransform = target.windParticle.transform;
                                        targetTransform.position = new Vector3(targetTransform.position.x, buildLocation.y + DestructionManager.instance.heightLimit, targetTransform.position.z);
                                    }
                                    else if (target.warningSystemEngaged)
                                    {
                                        Destroy(target.windParticle);
                                        target.warningSystemEngaged = false;
                                    }
                                }
                            }
                            if (randomValue >= selfChance * 0.75f && randomValue < selfChance)
                            {
                                if (gridArray[newArrayPosX, newArrayPosZ + 1].gridStructures[gridY].y == 0 && gridArray[newArrayPosX, newArrayPosZ + 1].gridStructures[gridY - 1].y == 0)
                                {
                                    newArrayPosZ = arrayPosZ + 1;         
                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                      
                                    GridList target = gridArray[arrayPosX, arrayPosZ];
                                    //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    target.branchedStructures = gridArray[arrayPosX, arrayPosZ].sizeY;
                                    isBranch = true;

                                    hasBuilt = true;
                                    buildRotation = 90;
                                    if (target.warningSystemEngaged && target.sizeY - target.branchedStructures >= DestructionManager.instance.heightLimit)
                                    {
                                        Transform targetTransform = target.windParticle.transform;
                                        targetTransform.position = new Vector3(targetTransform.position.x, buildLocation.y + DestructionManager.instance.heightLimit, targetTransform.position.z);
                                    }
                                    else if (target.warningSystemEngaged)
                                    {
                                        Destroy(target.windParticle);
                                        target.warningSystemEngaged = false;
                                    }
                                }
                            }

                            size.z = cellSize * 2f;
                            gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].xOrigin = arrayPosX;
                            gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].zOrigin = arrayPosZ;

                            //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                            gridArray[arrayPosX, arrayPosZ].branchedStructures = gridArray[arrayPosX, arrayPosZ].structureAmount;
                            hasBuilt = true;


                        }


                    }


                    //Instantiate structure.
                    // hasBuild == if the previous operators were true and the agent can build on this position. Else, do nothing.
                    if (hasBuilt)
                    {
                        GameObject finalStructure = new GameObject();
                        
                        int randomBranch = Random.Range(0, 6);

                        if (randomBranch == 0)
                        {
                            finalStructure = branch;
                        }
                        else if (randomBranch == 1)
                        {
                            finalStructure = branch2;
                        }
                        else if (randomBranch == 2)
                        {
                            finalStructure = branch3;
                        }
                        else if (randomBranch == 3)
                        {
                            finalStructure = branch4;
                        }
                        else if (randomBranch == 4)
                        {
                            finalStructure = branch5;
                        }
                        else if (randomBranch == 5)
                        {
                            finalStructure = branch6;
                        }

                        builtStructure = Instantiate(finalStructure, buildLocation, Quaternion.identity) as GameObject;
                        builtStructure.transform.Rotate(new Vector3(0, buildRotation, 0));

                        builtStructure.transform.SetParent(gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform);
                                                                     

                        gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].y = gridY;

                        gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].strucObject = builtStructure;
                        gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].x = newArrayPosX;
                        gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].z = newArrayPosZ;
                    }
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
            //canSpawn = true;
        }

        if (Input.GetMouseButton(0) == true && !IsOverUi())
        {

            SpawnSettings.Instance.PlaceAgent(spawnAgent);
        }
    }

}

