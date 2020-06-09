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
    [SerializeField] int destructionTimer = 10;
    [SerializeField] GameObject destructionAnim;

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
    bool hasBuilt = false;

    GameObject builtStructure;
    int gridY;

    public float pointRadius;
    public float maxDestinationDistance;

    Vector3 agentMoveLocation;
    List<Vector3> orientPositions;

    GridList[,] gridArray;
    AgentStack agentStack;


    int arrayPosX;
    int arrayPosZ;

    int newArrayPosX;
    int newArrayPosZ;

    void Start()
    {

        arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
        arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);



        cam = Camera.main;

        cellSize = GridArray.Instance.cellSize;
        cellY = GridArray.Instance.cellY;
        gridArray = GridArray.Instance.gridArray;
        agentStack = GridArray.Instance.agentStack;

        StartCoroutine(RetireTimer());

        if (GetComponent<NavMeshAgent>())
        {
            StartCoroutine(MoveTimer());

        }

  
    }


    IEnumerator RetireTimer()
    {
        yield return new WaitForSeconds(destructionTimer);
        Instantiate(destructionAnim, this.transform.position, Quaternion.identity);
        agentStack.agentAmountBranch += 1;
       
        Destroy(this.gameObject);

    }
    private void RetireAgent()
    {
        agentStack.agentAmountBranch += 1;
        Destroy(this.gameObject);


    }

    public void ReceiveSignal(Vector3 position, float destMin)
    {
        transform.GetChild(0).gameObject.GetComponent<SetAgentHeight>().GoToSignal(position, destMin);
    }

    void Update()
    {

        if (isActive)
        {
            transform.Rotate(0, 360 * Time.deltaTime, 0); //rotates 50 degrees per second around z axis

            //transform.GetChild(0).transform.position = new Vector3(-1.3f, 0, 0);
            transform.GetChild(0).transform.GetChild(1).localPosition = new Vector3(-0.035f, 0, -0.02f);


        }
        else 
        {
            transform.Rotate(0, 360 * Time.deltaTime, 0); //rotates 50 degrees per second around z axis
            transform.GetChild(0).transform.GetChild(1).localPosition = new Vector3(-0.003f, 0, -0.02f);
            transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).localScale = new Vector3(4f, 4f, 4f);
            transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<ParticleSystem>().playbackSpeed = 2.5f;
            transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<ParticleSystem>().maxParticles = 300;





        }

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
            RetireAgent();
        }
    }

          
    IEnumerator MoveTimer()
    {
        //Looping and delaying their walk cycle 
        while (isActive == true && hasSignal == false)
        {

            gridY = Mathf.RoundToInt(gridArray[arrayPosX, arrayPosZ].sizeY);

            
            if (gridArray[arrayPosX, arrayPosZ].branchAtY.Count > 0)
            {

                if (gridArray[arrayPosX, arrayPosZ].branchAtY[gridArray[arrayPosX, arrayPosZ].branchAtY.Count - 1] != gridY)
                {
                    BuildStructureNew();
                }
          

                /*
                for (int i = 0; i < gridArray[arrayPosX, arrayPosZ].branchAtY.Count; i++)
                {
                    if (gridArray[arrayPosX, arrayPosZ].branchAtY[i] != gridY)
                    {
                        BuildStructureNew();
                        break;
                    }
                }
                */
            }
            
            
            if (gridArray[arrayPosX, arrayPosZ].branchAtY.Count == 0)
            {
                BuildStructureNew();
            }
            


            yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));
            canBuild = true;

        }
    }

    private void SetBuildLocation()
    {
        float buildY = transform.position.y + cellY * gridArray[arrayPosX, arrayPosZ].sizeY ;
        buildLocation = new Vector3(arrayPosX * cellSize, buildY, arrayPosZ * cellSize);
        
        
        hasBuilt = true;

        GridList target = gridArray[arrayPosX, arrayPosZ];

        target.branchedStructures = gridArray[arrayPosX, arrayPosZ].sizeY;


        
        if (target.windParticle!=null)
        {
            
            Destroy(target.windParticle);
            target.windParticle = null;
        }
    }

    private void BuildStructureNew()
    {
        gridArray = GridArray.Instance.gridArray;
        if (canBuild == true)
        {

            gridY = Mathf.RoundToInt(gridArray[arrayPosX, arrayPosZ].sizeY);


            newArrayPosX = arrayPosX;
            newArrayPosZ = arrayPosZ;


            canBuild = false;
            hasBuilt = false;


            if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
            {
                if (gridArray[arrayPosX, arrayPosZ] != null && gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].bridge <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                {

                    int randomValue = Random.Range(0, 100);
                    Vector3 size = structure.transform.localScale;
                    int selfChance = branchChance / 4;

                    //Check if can branch

                    if (gridY >= minBranchHeight)
                    {
                        if (randomValue >= 0 && randomValue < selfChance * 0.25f)
                        {
                            newArrayPosX = arrayPosX - 1;
                            buildRotation = 0;
                        }

                        if (randomValue >= selfChance * 0.25f && randomValue < selfChance * 0.5f)
                        {
                            newArrayPosX = arrayPosX + 1;
                            buildRotation = 180;
                        }

                        if (randomValue >= selfChance * 0.5f && randomValue < selfChance * 0.75f)
                        {
                            newArrayPosZ = arrayPosZ - 1;
                            buildRotation = 270;
                        }
                        if (randomValue >= selfChance * 0.75f && randomValue < selfChance)
                        {
                            newArrayPosZ = arrayPosZ + 1;
                            buildRotation = 90;
                        }
                        //FINAL Check                            
                        if (gridArray[newArrayPosX, newArrayPosZ].sizeY < gridY && (newArrayPosX != arrayPosX || newArrayPosZ != arrayPosZ))
                        {
                            if (gridArray[newArrayPosX, newArrayPosZ].branchAtY.Count > 0)
                            {
                                if (gridArray[newArrayPosX, newArrayPosZ].branchAtY[gridArray[newArrayPosX, newArrayPosZ].branchAtY.Count-1] != gridY)
                                {
                                    SetBuildLocation();
                                }

                                /*
                                for (int i = 0; i < gridArray[newArrayPosX, newArrayPosZ].branchAtY.Count; i++)
                                {
                                    if (gridArray[newArrayPosX, newArrayPosZ].branchAtY[i] != gridY)
                                    {
                                        SetBuildLocation();

                                        break;
                                    }
                                }
                                */
                            }
                            else
                            {
                                SetBuildLocation();
                            }

                        }

                    }


                    //Instantiate structure.
                    // hasBuild == if the previous operators were true and the agent can build on this position. Else, do nothing.
                    if (hasBuilt)
                    {
                        GameObject finalStructure = null;

                        int randomBranch = Random.Range(2, 6);

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
                        if (builtStructure != null)
                        {
                            builtStructure.transform.Rotate(new Vector3(0, buildRotation, 0));
                            //builtStructure.transform.SetParent(gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform);
                            builtStructure.GetComponent<StructureBehavior>().startY = buildLocation.y;

                            builtStructure.transform.localScale = new Vector3(builtStructure.transform.localScale.x, builtStructure.transform.localScale.y, builtStructure.transform.localScale.z  - gridArray[arrayPosX, arrayPosZ].towerWidth);

                            structuresPlaced += 1;
                            gridArray[newArrayPosX, newArrayPosZ].branchAtY.Add(gridY);
                            gridArray[newArrayPosX, newArrayPosZ].branchObjects.Add(builtStructure);
                            Instantiate(destructionAnim, this.transform.position, Quaternion.identity);
                            agentStack.agentAmountBranch += 1;
                            Destroy(this.gameObject);
                        }            
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
        mousePos.z = 50.0f;
        mousePos.y += 50.0f;

        Vector3 objectPos = cam.ScreenToWorldPoint(mousePos);
        transform.position = objectPos;

        if (SpawnSettings.Instance.spawnMode == false && isActive == false)
        {
            Destroy(this.gameObject);
        }

        if (Input.GetMouseButton(1))
        {
            SpawnSettings.Instance.spawnMode = false;
            //Destroy(this.gameObject);

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
    private void OnDestroy()
    {
        if (isActive)
        {
            agentStack.agentBranch -= 1;
        }
    }

}

