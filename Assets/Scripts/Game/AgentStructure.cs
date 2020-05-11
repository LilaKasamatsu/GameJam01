using System.Collections;
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

    [SerializeField] int minBranchHeight = 4;
    [SerializeField] int branchChance = 40;

    public int structuresLifetime = 10;
    int structuresPlaced = 0;


    private GameObject ground;

    public Grid grid;
    Camera cam;
    public NavMeshAgent agent;

    public bool hasSignal = false;
    public bool isActive = false;
    bool canBuild = false;
    bool canSpawn = false;
    private int cellSize;
    private int cellY;
    Vector3 buildLocation;
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
        ground = GameObject.FindGameObjectWithTag("ground");

        cellSize = GridArray.Instance.cellSize;
        cellY = GridArray.Instance.cellY;
        gridArray = GridArray.Instance.gridArray;
        agentStack = GridArray.Instance.agentStack;

        // DIESER CODE IST DER NEUE

        //Create orientPosition List based on the grid
        //Adds all existing build-orientation points currently on the scene to the list
        // "state == true" means, that that location has a structure to orient on.



        StartCoroutine(MoveTimer());
        if (structuresPlaced >= structuresLifetime)
        {
            //StartCoroutine("RetireAgent");
        }
    }


    IEnumerator RetireAgent()
    {
        agentStack.agentAmount += 1;
        agentStack.agentStructure -= 1;
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

                    if (gridArray[x, z].structureAmount > 0)
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
                //buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount + 1, GridArray.Instance.RoundToGrid(transform.position.z));


                if (gridArray[arrayPosX,arrayPosZ]!=null && gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].bridge <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                {

                    gridY = Mathf.RoundToInt(gridArray[arrayPosX, arrayPosZ].structureAmount);
                    
                    int randomValue = Random.Range(0, 100);
                    Vector3 size = structure.transform.localScale;
                    int selfChance = branchChance / 4;
                    bool hasBuilt = false;
                
                    //Set building to branch
                    if (gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched == false)
                    {
                        //Check if branch is possible
                        //gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].y
                        if (gridY >= minBranchHeight)
                        {
                            if (randomValue >= 0 && randomValue < selfChance * 0.25f)
                            {
                                if (gridArray[newArrayPosX - 1, newArrayPosZ].gridStructures[gridY].y == 0 && gridArray[newArrayPosX - 1, newArrayPosZ].gridStructures[gridY - 1].y == 0)
                                {
                                    size.x = cellSize * 2f;
                                    newArrayPosX = arrayPosX - 1;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].xOrigin = arrayPosX;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].zOrigin = arrayPosZ;

                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x) - cellSize, cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    gridArray[arrayPosX, arrayPosZ].bridgedStructures = gridArray[arrayPosX, arrayPosZ].structureAmount;
                                    hasBuilt = true;

                                }
                            }
                            
                            if (randomValue >= selfChance * 0.25f && randomValue < selfChance * 0.5f)
                            {
                                if (gridArray[newArrayPosX + 1, newArrayPosZ].gridStructures[gridY].y == 0 && gridArray[newArrayPosX + 1, newArrayPosZ].gridStructures[gridY - 1].y == 0)
                                {
                                    size.x = cellSize * 2f;
                                    newArrayPosX = arrayPosX + 1;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].xOrigin = arrayPosX;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].zOrigin = arrayPosZ;

                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x) + cellSize, cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    gridArray[arrayPosX, arrayPosZ].bridgedStructures = gridArray[arrayPosX, arrayPosZ].structureAmount;

                                    hasBuilt = true;
                                }
                            }
                            
                            if (randomValue >= selfChance * 0.5f && randomValue < selfChance * 0.75f)
                            {
                                if (gridArray[newArrayPosX, newArrayPosZ - 1].gridStructures[gridY].y == 0 && gridArray[newArrayPosX, newArrayPosZ - 1].gridStructures[gridY - 1].y == 0)
                                {
                                    size.z = cellSize * 2f;
                                    newArrayPosZ = arrayPosZ - 1;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].xOrigin = arrayPosX;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].zOrigin = arrayPosZ;

                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z) - cellSize);
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    gridArray[arrayPosX, arrayPosZ].bridgedStructures = gridArray[arrayPosX, arrayPosZ].structureAmount;

                                    hasBuilt = true;
                                }
                            }
                            if (randomValue >= selfChance * 0.75f && randomValue < selfChance)
                            {
                                if (gridArray[newArrayPosX, newArrayPosZ + 1].gridStructures[gridY].y == 0 && gridArray[newArrayPosX, newArrayPosZ + 1].gridStructures[gridY - 1].y == 0)
                                {
                                    size.z = cellSize * 2f;
                                    newArrayPosZ = arrayPosZ + 1;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].xOrigin = arrayPosX;
                                    gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY].zOrigin = arrayPosZ;

                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY, GridArray.Instance.RoundToGrid(transform.position.z) + cellSize);
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount - cellY;

                                    gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].isBranched = true;
                                    gridArray[arrayPosX, arrayPosZ].bridgedStructures = gridArray[arrayPosX, arrayPosZ].structureAmount;

                                    hasBuilt = true;
                                }
                            }

                        }                       
                                            

                    }


                    //Normal Building
                    // || gridY == 0 || gridArray[arrayPosX, arrayPosZ].gridStructures[gridY].y < minBranchHeight
                    if (randomValue >= branchChance || gridY < minBranchHeight)
                    {
                        if (gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY + 1].y == 0 && gridArray[newArrayPosX, newArrayPosZ].gridStructures[gridY + 2].y == 0)
                        {
                            //buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].structureAmount, GridArray.Instance.RoundToGrid(transform.position.z));
                            buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), 0, GridArray.Instance.RoundToGrid(transform.position.z));
                            buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].structureAmount;

                            gridArray[arrayPosX, arrayPosZ].structureAmount += 1;
                            hasBuilt = true;
                        }
                    }

                    //Instantiate structure.
                    // hasBuild == if the previous operators were true and the agent can build on this position. Else, do nothing.
                    if (hasBuilt)
                    {
                        builtStructure = Instantiate(structure, buildLocation, Quaternion.identity) as GameObject;
                        builtStructure.transform.localScale = size;

                        builtStructure.transform.localScale = new Vector3(builtStructure.transform.localScale.x - gridArray[newArrayPosX, newArrayPosZ].towerWidth,
                            builtStructure.transform.localScale.y,
                            builtStructure.transform.localScale.z - gridArray[newArrayPosX, newArrayPosZ].towerWidth);

                        gridArray[arrayPosX, arrayPosZ].structureObjects.Add(builtStructure);

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
            canSpawn = true;
        }

        if (Input.GetMouseButton(0) == true && !IsOverUi())
        {

            SpawnSettings.Instance.PlaceAgent(spawnAgent);
        }
    }

}

