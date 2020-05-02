using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class agent3Behavior : MonoBehaviour
{
    [SerializeField] GameObject structure;
    [SerializeField] GameObject marker;
    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] int maxBuildings;
    [SerializeField] GameObject spawnAgent;

    string[] radiusTags =
     {
                 "structure",
                 "point",

         };

    private int gridSize;

    private GameObject ground;
    private List<GameObject> mainPoint;
    private GameObject[] tagStructure;
    private GameObject[] tagPoint;

    private List<float> pointPositions;

    public Grid grid;
    public Camera cam;
    public NavMeshAgent agent;

    bool canBuild = false;
    public bool isActive = false;
    bool canSpawn = false;

    Vector3 agentMoveLocation;


    private mainPoint pointScript;
    public float pointRadius;


    GameObject spawnController;
    private SpawnSettings spawnerScript;


    List<GridList> gridList;
    List<OrientPositions> orientPositions = new List<OrientPositions>();
    List<Vector3> buildPoints;

    void Start()
    {
        cam = Camera.main;

        ground = GameObject.FindGameObjectWithTag("ground");



        spawnController = GameObject.Find("spawnController");
        spawnerScript = spawnController.GetComponent<SpawnSettings>();

        gridList = spawnerScript.gridList;
        gridSize = spawnerScript.gridSize;



        // DIESER CODE IST DER NEUE

        //Create orientPosition List based on the grid
        //Adds all existing build-orientation points currently on the scene to the list
        // "state == true" means, that that location has a structure to orient on.
        for (int i = 0; i < gridList.Count; i++)
        {
            orientPositions.Add(new OrientPositions( gridList[i].x, gridList[i].z, false ));

        }
        
        //

        StartCoroutine(MoveTimer());


    }



    void Update()
    {
        grid = spawnerScript.grid;

      
        

        if (isActive == false)
        {
            SpawnActiveAgent();

        }
    }


    IEnumerator MoveTimer()
    {
        //while loop is a bit more elegant in an coroutine then stopping and starting it over and over again-Philip
        while (isActive == true)
        {
            BuildFoundation();


            yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));

            //DIESER CODE IST DER NEUE
            // instead of workingthrew the same list twice, once to change the bool and then a second time to cope the true once you could just skipp the first steps-Philip

            //Search Grid for structures/main points
            //The list buildPoints saves the Vector3 of all structures, that the agent has to orient on
            List<Vector3> buildPoints = new List<Vector3>();
            for (int i = 0; i < gridList.Count; i++)
            {

                if (gridList[i].pointAmount > 0 || gridList[i].foundationAmount > 0)
                {

                    //return that this position has a strucutre to orient on.
                    //orientPositions[i].state = true;
                    buildPoints.Add(new Vector3(gridList[i].x, transform.position.y, gridList[i].z));

                }
            }

            //"GetClosestTarget" then compares all of those Vector3 and finds the closest
            
            Vector3 closestMainPoint = GetClosestTarget(buildPoints);
            agentMoveLocation = new Vector3(closestMainPoint.x + Random.Range(-pointRadius, pointRadius), transform.position.y, closestMainPoint.z + Random.Range(-pointRadius, pointRadius));

            //

            //agentMoveLocation = new Vector3(transform.position.x + Random.Range(-pointRadius, pointRadius), transform.position.y, transform.position.z + Random.Range(-pointRadius, pointRadius));

            canBuild = true;

            agent.SetDestination(agentMoveLocation);

        }

        

    }

    Vector3 GetClosestTarget(List<Vector3> target)
    {
        Vector3 tMin = transform.position;
        Vector3 currentPos = transform.position;
        float minDist = 10f;
 
        foreach (Vector3 t in target)
        {
            float dist = Vector3.Distance(t, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }



    private void BuildFoundation()
    {

        if (!agent.hasPath && canBuild == true)
        {
            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, structure.transform.localScale.y / 2, Mathf.Round(transform.position.z / gridSize) * gridSize);
            canBuild = false;



            for (int i = 0; i < gridList.Count; i++)
            {
                if (gridList[i].x == buildLocation.x && gridList[i].z == buildLocation.z && gridList[i].pointAmount <= 0)
                {
                    if (gridList[i].foundationAmount <= 0)
                    {
                        Instantiate(structure, buildLocation, Quaternion.identity);


                        gridList[i].foundationAmount = gridList[i].foundationAmount + 1;
                    }
                }
            }
        }
    }

    private bool isOverUi()
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


        if (Input.GetMouseButtonDown(1))
        {
            Destroy(this.gameObject);
        }

        if (Input.GetMouseButtonUp(0))
        {
            canSpawn = true;
        }

        if (Input.GetMouseButton(0) == true && canSpawn == true && !isOverUi())
        {


            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layer_mask = LayerMask.GetMask("Ground");


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, Mathf.Round(hit.point.y / gridSize) * gridSize, Mathf.Round(hit.point.z / gridSize) * gridSize);



                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].structureAmount <= 0  && gridList[i].pointAmount <= 0 && gridList[i].foundationAmount <= 0)
                    {

                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, structure.transform.localScale.y / 2, Mathf.Round(hit.point.z / gridSize) * gridSize);

                        GameObject newAgent = Instantiate(spawnAgent, spawnLocation, Quaternion.identity);
                        newAgent.GetComponent<agent3Behavior>().isActive = true;
                        Instantiate(structure, spawnLocation, Quaternion.identity);


                        gridList[i].foundationAmount = gridList[i].foundationAmount + 1;

                    }
                }
            }
        }
    }

}
