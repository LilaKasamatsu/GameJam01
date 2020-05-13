using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnSettings : MonoBehaviour
{

    [SerializeField] GameObject agentStruc;
    [SerializeField] GameObject agentPoint;
    [SerializeField] GameObject agentFound;

    [SerializeField] GameObject foundation;
    [SerializeField] GameObject mainPoint;
    [SerializeField] GameObject buildMarker;

    private GameObject ground;

    Camera cam;

    public Grid grid;
    public int cellSize = 3;
    int cellY;
    public List<GridList> gridList = new List<GridList>();
    public bool spawnMode = false;
    private Vector3 lastPlacedTile;
    private bool tileTimer = true;
    AgentStack agentStack;

    //Singleton
    public static SpawnSettings Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        cam = Camera.main;
        ground = GameObject.FindGameObjectWithTag("ground");
        //grid = new Grid(Mathf.RoundToInt(ground.transform.localScale.x / cellSize) + 1, Mathf.RoundToInt(ground.transform.localScale.z / cellSize) + 1, cellSize);
        
        cellSize = GridArray.Instance.cellSize;
        cellY = GridArray.Instance.cellY;


        agentStack = GridArray.Instance.agentStack;


        //CreateStartPoint();

    }

    IEnumerator SetSpawnDelay()
    {
        tileTimer = false;
        yield return new WaitForSeconds(0.1f);
        tileTimer = true;
    }

    void Update()
    {
        //NumberSpawning();
        GetGridValue();

        if (spawnMode == false && lastPlacedTile != new Vector3 (-1, -1, -1))
        {
            lastPlacedTile = new Vector3(-1, -1, -1);
        }
    }



    public void PlaceAgent(GameObject agent)
    {
        bool hitGround = false;
        RaycastHit hitAll = new RaycastHit();

        RaycastHit[] hits;
        hits = Physics.RaycastAll(cam.ScreenPointToRay(Input.mousePosition));


        for (int i = 0; i < hits.Length; i++)
        {
            hitAll = hits[i];

            if (hitAll.collider.gameObject.CompareTag("ground"))
            {
                hitGround = true;
                break;               
            }
        }

        /*
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layer_mask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
        {
               
            
        }
        */
        GridList[,] gridArray = GridArray.Instance.gridArray;

        if (hitGround == true)
        {
            Vector3 hitGrid = new Vector3(Mathf.Round(hitAll.point.x / cellSize) * cellSize, Mathf.Round(hitAll.point.y / cellY) * cellY, Mathf.Round(hitAll.point.z / cellSize) * cellSize);

            int arrayPosX = GridArray.Instance.NumToGrid(hitGrid.x);
            int arrayPosZ = GridArray.Instance.NumToGrid(hitGrid.z);
            float arrayPosY = hitAll.point.y;

            //Vector3 currentTile = new Vector3(arrayPosX, arrayPosY, arrayPosX);

            if (tileTimer)
            {

                if (hitAll.normal.normalized == new Vector3(0f, 1f, 0f))
                {

                    //Foundation Agent
                    if (agent.GetComponent<AgentFoundation>() != null && agentStack.agentAmount > 0)
                    {

                        if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
                        {
                            if (gridArray[arrayPosX, arrayPosZ].pointAmount <= 0)
                            {
                                agentStack.agentAmount -= 1;
                                agentStack.agentFoundation += 1;

                                Vector3 spawnLocation = hitGrid;
                                spawnLocation.y = hitAll.point.y;

                                //+ new Vector3(0, agent.transform.localScale.y)

                                GameObject newAgent = Instantiate(agent, spawnLocation, Quaternion.identity);
                                newAgent.GetComponent<NavMeshAgent>().enabled = true;

                                newAgent.GetComponent<AgentFoundation>().isActive = true;

                                //lastPlacedTile = currentTile;
                                StartCoroutine(SetSpawnDelay());
                            }
                        }
                    }

                    //Structure Agent
                    if (agent.GetComponent<AgentStructure>() != null && agentStack.agentAmount > 0)
                    {
                        if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
                        {
                            if (gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                            {
                                agentStack.agentAmount -= 1;
                                agentStack.agentStructure += 1;


                                Vector3 spawnLocation = hitGrid;
                                spawnLocation.y = hitAll.point.y + agent.transform.localScale.y * 0.5f;

                                GameObject newAgent = Instantiate(agent, spawnLocation, Quaternion.identity);
                                newAgent.GetComponent<NavMeshAgent>().enabled = true;

                                newAgent.GetComponent<AgentStructure>().isActive = true;

                                //lastPlacedTile = currentTile;
                                StartCoroutine(SetSpawnDelay());


                            }
                        }
                    }
                    //Branch Agent
                    if (agent.GetComponent<AgentBranch>() != null && agentStack.agentAmount > 0)
                    {
                        if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ))
                        {
                            if (gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                            {
                                agentStack.agentAmount -= 1;
                                agentStack.agentBranch += 1;


                                Vector3 spawnLocation = hitGrid;
                                spawnLocation.y = hitAll.point.y + agent.transform.localScale.y * 0.5f;

                                GameObject newAgent = Instantiate(agent, spawnLocation, Quaternion.identity);
                                newAgent.GetComponent<NavMeshAgent>().enabled = true;

                                newAgent.GetComponent<AgentBranch>().isActive = true;

                                //lastPlacedTile = currentTile;
                                StartCoroutine(SetSpawnDelay());

                            }
                        }
                    }
                }

            }
        }

     


    }

    public void SpawnAgent(GameObject spawnAgent, Vector3 position)
    {
        

        //Instantiate(spawnAgent, spwnLocation, cam.transform.rotation);

        Vector3 mousePos = Input.mousePosition;
        //mousePos.z = 1.0f;  // Preview Agent is 10 units in front of the camera
        mousePos.y = 125.0f;  // Preview Agent is a bit higher than the mouse cursor

        Vector3 objectPos = cam.ScreenToWorldPoint(mousePos);
        GameObject newAgent = Instantiate(spawnAgent, objectPos, cam.transform.rotation) as GameObject;
        newAgent.GetComponent<NavMeshAgent>().enabled = false;

        CreateBuildMarker();

    }

    private void CreateBuildMarker()
    {
        Instantiate(buildMarker, new Vector3(0, 0, 0), Quaternion.identity);
              
    

    }


    private void GetGridValue()
    {
        //Mouse Right
        if (Input.GetMouseButtonDown(1))
        {



            Ray ray1 = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // && hit.collider.CompareTag("structure") || hit.collider.CompareTag("ground") ||  hit.collider.CompareTag("marker")

            if (Physics.Raycast(ray1, out hit))
            {

                Vector3 hitPosition = hit.collider.gameObject.transform.parent.position;

                hitPosition.x = GridArray.Instance.RoundToGrid(hit.collider.gameObject.transform.parent.position.x );
                hitPosition.z = GridArray.Instance.RoundToGrid(hit.collider.gameObject.transform.parent.position.z );

                hitPosition.y = Mathf.RoundToInt(hit.collider.gameObject.transform.parent.position.y /cellY ) * cellY;


                int hitX = GridArray.Instance.NumToGrid(hitPosition.x);
                int hitZ = GridArray.Instance.NumToGrid(hitPosition.z);


                if (GridArray.Instance.CheckArrayBounds(hitX, hitZ) && !hit.collider.gameObject.CompareTag("ground"))
                {
                    //Debug on structure amount, foundation etc
                    //Debug.Log("; Foundation: " + GridArray.Instance.gridArray[hitX, hitZ].foundationAmount + "; Structures: " + GridArray.Instance.gridArray[hitX, hitZ].structureAmount + "; Point: " + GridArray.Instance.gridArray[hitX, hitZ].pointAmount);

                    Debug.Log(" x: " + hitX + " z: " + hitZ + " posY: " + GridArray.Instance.gridArray[hitX, hitZ].gridStructures[Mathf.RoundToInt(hitPosition.y / cellY) + 1].y);
                    Debug.Log(" xOrigin: " + GridArray.Instance.gridArray[hitX, hitZ].gridStructures[Mathf.RoundToInt(hitPosition.y / cellY) + 1].xOrigin + " zOrigin: " + GridArray.Instance.gridArray[hitX, hitZ].gridStructures[Mathf.RoundToInt(hitPosition.y / cellY) + 1].zOrigin);

                    /*
                    for (int i = 0; i < GridArray.Instance.gridArray[hitX, hitZ].gridStructures.Length; i++)
                    {
                        //GridArray.Instance.gridArray[hitX, hitZ].gridStructures[i].y == Mathf.RoundToInt(hit.collider.transform.position.y / GridArray.Instance.cellY) * GridArray.Instance.cellY

                        if (1 == 1)
                        {
                            Debug.Log(" x: " + hitX + " z: " + hitZ + " posY: " + GridArray.Instance.gridArray[hitX, hitZ].gridStructures[Mathf.RoundToInt(hit.collider.transform.position.y / cellY)].y);
                            break;

                        }
                        else
                        {
                            Debug.Log("ERROR " + "x: " + hitX + " z: " + hitZ + "posY: " + hit.collider.transform.position.y);
                            break;

                        }                        
                    }
                    */

                }

            }
        }
    }

}
