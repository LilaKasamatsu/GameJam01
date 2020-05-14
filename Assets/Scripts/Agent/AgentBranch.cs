using System.Collections;
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


    void Start()
    {
   

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
        agentStack.agentBranch -= 1;
        Destroy(this.gameObject);

    }
    private void RetireAgent()
    {
        agentStack.agentAmountBranch += 1;
        agentStack.agentBranch -= 1;
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

            arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
            arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);
            gridY = Mathf.RoundToInt(gridArray[arrayPosX, arrayPosZ].sizeY);

            
            if (gridArray[arrayPosX, arrayPosZ].branchAtY.Count >= 0)
            {
                for (int i = 0; i < gridArray[arrayPosX, arrayPosZ].branchAtY.Count; i++)
                {
                    if (gridArray[arrayPosX, arrayPosZ].branchAtY[i] != gridY)
                    {
                        BuildStructureNew();
                        break;
                    }
                }
            }
            
            
            if (gridArray[arrayPosX, arrayPosZ].branchAtY.Count == 0)
            {
                BuildStructureNew();
            }
            


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


    int arrayPosX;
    int arrayPosZ;

    int newArrayPosX;
    int newArrayPosZ;

    private void SetBuildLocation()
    {
        buildLocation = new Vector3(arrayPosX * cellSize, 0, arrayPosZ * cellSize);
        buildLocation.y = transform.position.y + cellY * gridArray[arrayPosX, arrayPosZ].sizeY - 3;
        gridArray[arrayPosX, arrayPosZ].branchedStructures = gridArray[arrayPosX, arrayPosZ].sizeY;
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
        if (!agent.hasPath && canBuild == true)
        {
            arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
            arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);
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
                    if (true)
                    {
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
                                    for (int i = 0; i < gridArray[newArrayPosX, newArrayPosZ].branchAtY.Count; i++)
                                    {
                                        if (gridArray[newArrayPosX, newArrayPosZ].branchAtY[i] != gridY)
                                        {
                                            SetBuildLocation();
                         
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    SetBuildLocation();
                                }
                                
                            }                          
           
                        }
                    }


                    //Instantiate structure.
                    // hasBuild == if the previous operators were true and the agent can build on this position. Else, do nothing.
                    if (hasBuilt)
                    {
                        GameObject finalStructure = null;

                        int randomBranch = Random.Range(1, 6);

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
                        //builtStructure.transform.SetParent(gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform);
                        builtStructure.GetComponent<StructureBehavior>().startY = buildLocation.y;

                        structuresPlaced += 1;
                        gridArray[newArrayPosX, newArrayPosZ].branchAtY.Add(gridY);
            
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

