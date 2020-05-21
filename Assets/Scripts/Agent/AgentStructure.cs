using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AgentStructure : MonoBehaviour
{
    [SerializeField] GameObject structure;
    [SerializeField] GameObject structureSquare;

    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] GameObject spawnAgent;

    [SerializeField] int minBranchHeight = 4;
    [SerializeField] int branchChance = 40;





    public int structuresLifetime = 10;
    int structuresPlaced = 0;
    [SerializeField] int destructionTimer = 10;
    [SerializeField] GameObject destructionAnim;

    private GameObject ground;

    public Grid grid;
    Camera cam;
    public NavMeshAgent agent;

    public bool hasSignal = false;
    public bool isActive = false;
    bool canBuild = false;
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



        StartCoroutine(RetireTimer());

        if (GetComponent<NavMeshAgent>())
        {
            StartCoroutine(MoveTimer());
        }

        if (structuresPlaced >= structuresLifetime)
        {
            //StartCoroutine("RetireAgent");
        }
    }

    IEnumerator RetireTimer()
    {
        yield return new WaitForSeconds(destructionTimer);
        Instantiate(destructionAnim, this.transform.position, Quaternion.identity);
        agentStack.agentAmountStructure += 1;
        agentStack.agentStructure -= 1;
        Destroy(this.gameObject);

    }
    IEnumerator RetireAgent()
    {
        agentStack.agentAmountStructure += 1;
        agentStack.agentStructure -= 1;
        yield return new WaitForSeconds(Random.Range(0, 3));
        Destroy(this.gameObject);


    }

    public void ReceiveSignal(Vector3 position, float destMin)
    {
        transform.GetChild(0).gameObject.GetComponent<SetAgentHeight>().GoToSignal(position, destMin);
    }

    void Update()
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

    StructureBehavior targetStructure = new StructureBehavior();

    private void BuildStructure()
    {
        gridArray = GridArray.Instance.gridArray;
        if (!agent.hasPath && canBuild == true)
        {
            int arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
            int arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);
            canBuild = false;

            if (gridArray[arrayPosX, arrayPosZ].sizeY >= 1)
            {
                targetStructure = gridArray[arrayPosX, arrayPosZ].structureObjects[0].GetComponent<StructureBehavior>();

            }
            if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
            {
                if (gridArray[arrayPosX,arrayPosZ]!=null && gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                {                                       
                    gridY = Mathf.RoundToInt(gridArray[arrayPosX, arrayPosZ].sizeY);                   
                    Vector3 size = structure.transform.localScale;
                    bool hasBuilt = false;             
                    //Normal Building
                    if (gridY < GridArray.Instance.maxStructures - 2 || gridY < minBranchHeight && gridY < GridArray.Instance.maxStructures-2)
                    {                        if (gridArray[arrayPosX, arrayPosZ].branchAtY.Count > 0)
                        {
                            for (int i = 0; i < gridArray[arrayPosX, arrayPosZ].branchAtY.Count; i++)
                            {
                                if (gridArray[arrayPosX, arrayPosZ].branchAtY[i] < gridY + 1 )
                                {
                                    //buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].sizeY, GridArray.Instance.RoundToGrid(transform.position.z));

                                    buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), 0, GridArray.Instance.RoundToGrid(transform.position.z));
                                    buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].sizeY;

                                    hasBuilt = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), 0, GridArray.Instance.RoundToGrid(transform.position.z));
                            buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].sizeY;

                            hasBuilt = true;
                        }
          

                        /*
                        if (gridArray[arrayPosX, arrayPosZ].gridStructures[gridY + 1].y == 0 && gridArray[arrayPosX, arrayPosZ].gridStructures[gridY + 2].y == 0)
                        {
                            //buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), cellY * gridArray[arrayPosX, arrayPosZ].sizeY, GridArray.Instance.RoundToGrid(transform.position.z));
                            buildLocation = new Vector3(GridArray.Instance.RoundToGrid(transform.position.x), 0, GridArray.Instance.RoundToGrid(transform.position.z));
                            buildLocation.y = transform.position.y - transform.localScale.y + cellY * gridArray[arrayPosX, arrayPosZ].sizeY;
                            hasBuilt = true;
                        }                        */
                    }

                    //Instantiate structure.
                    // hasBuild == if the previous operators were true and the agent can build on this position. Else, do nothing.
                    if (hasBuilt)
                    {
                        GameObject finalStructure = null;
                                               
                        if (gridArray[arrayPosX, arrayPosZ].structureShape == "squ")
                        {
                            finalStructure = structureSquare;
                        }
                        else if (gridArray[arrayPosX, arrayPosZ].structureShape == "tri")
                        {
                            finalStructure = structure;
                        }
                        else
                        {
                            finalStructure = structure;
                        }

                        if (gridArray[arrayPosX, arrayPosZ].sizeY == 0)
                        {
                            builtStructure = Instantiate(finalStructure, buildLocation, Quaternion.identity) as GameObject;
                            //builtStructure.transform.Rotate(new Vector3(0, buildRotation, 0));                            gridArray[arrayPosX, arrayPosZ].sizeY += 1;
                            gridArray[arrayPosX, arrayPosZ].structureObjects.Add(builtStructure);                          
                            builtStructure.transform.localScale = new Vector3(builtStructure.transform.localScale.x - gridArray[arrayPosX, arrayPosZ].towerWidth,
                                0.1f,
                                builtStructure.transform.localScale.z - gridArray[arrayPosX, arrayPosZ].towerWidth);
                        }
                        else if(gridArray[arrayPosX, arrayPosZ].sizeY >= 1 && !targetStructure.isBridged)
                        {
                            gridArray[arrayPosX, arrayPosZ].sizeY += 1;
                            //gridArray[arrayPosX, arrayPosZ].CreateWindParticles();
                            /*
                            gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform.localScale 
                                = new Vector3 
                                (gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform.localScale.x, 
                                gridArray[arrayPosX, arrayPosZ].sizeY * cellY,
                                gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform.localScale.z);
                            */
                        }





                        gridArray[arrayPosX, arrayPosZ].gridStructures[0].y = gridY;

                        gridArray[arrayPosX, arrayPosZ].gridStructures[0].strucObject = builtStructure;
                        gridArray[arrayPosX, arrayPosZ].gridStructures[0].x = arrayPosX;
                        gridArray[arrayPosX, arrayPosZ].gridStructures[0].z = arrayPosZ;
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
        mousePos.z = 40.0f;
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

}

