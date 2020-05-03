using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class agent1Behavior : MonoBehaviour
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
          
         };


    private int gridSize;

    private GameObject ground;
    private GameObject[] mainPoint;

    public Grid grid;

    public Camera cam;
    public NavMeshAgent agent;

    bool canBuild = false;
    public bool isActive = false;
    bool canSpawn;

    Vector3 agentMoveLocation;


    private mainPoint pointScript;
    public float pointRadius;

   


    GameObject spawnController;
    private SpawnSettings spawnerScript;

    private Grid gridScript;

    List<GridList> gridList;


    void Start()
    {

        cam = Camera.main;

        ground = GameObject.FindGameObjectWithTag("ground");



        spawnController = GameObject.Find("spawnController");
        spawnerScript = spawnController.GetComponent<SpawnSettings>();

        gridList = spawnerScript.gridList; 
        gridSize = spawnerScript.cellSize;

        //mainPoint = GameObject.FindGameObjectsWithTag("structure");


        foreach (string tag in radiusTags)
        {
            mainPoint = GameObject.FindGameObjectsWithTag(tag);
          

        }

        StartCoroutine(MoveTimer());


    }



    void Update()
    {
        grid = spawnerScript.grid;




        if (isActive == true)
        {
            CheckBuild();
        }


        if (isActive == false)
        {
            SpawnActiveAgent();

        }


    }


    IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));


        //mainPoint = GameObject.FindGameObjectsWithTag("structure");

        foreach (string tag in radiusTags)
        {
            mainPoint = GameObject.FindGameObjectsWithTag(tag);

        }


        GameObject closestMainPoint = GetClosestTarget(mainPoint);

        //pointScript = closestMainPoint.GetComponent<mainPoint>();

        //pointRadius = pointScript.radius;

        agentMoveLocation = new Vector3(closestMainPoint.transform.position.x + Random.Range(-pointRadius, pointRadius), transform.position.y, closestMainPoint.transform.position.z + Random.Range(-pointRadius, pointRadius));
        canBuild = true;
        agent.SetDestination(agentMoveLocation);

        StartCoroutine(MoveTimer());

    }

    GameObject GetClosestTarget(GameObject[] target)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in target)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    private void CheckBuild()
    {

        if (!agent.hasPath && canBuild == true)
        {
            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, structure.transform.localScale.y/2 , Mathf.Round(transform.position.z / gridSize) * gridSize);
            canBuild = false;


     


            for (int i = 0; i < gridList.Count; i++)
            {
                if (gridList[i].x == buildLocation.x && gridList[i].z == buildLocation.z)
                {
                    if (gridList[i].structureAmount < maxBuildings && gridList[i].foundationAmount > 0 && gridList[i].pointAmount <= 0)
                    {

                        BuildStructure(buildLocation, i) ;

                        

                        //grid.SetValue(buildLocation, spawnerScript.gridList[i].structureAmount + 1);

                        gridList[i].structureAmount = gridList[i].structureAmount + 1;
                    }
                }
            }





        }
    }


    public void BuildStructure(Vector3 location, int height)
    {
        Instantiate(structure, location + new Vector3(0, 2 * gridList[height].structureAmount, 0), Quaternion.identity);
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
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].structureAmount <= 0 && gridList[i].foundationAmount > 0 && gridList[i].pointAmount <= 0)
                    {

                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, structure.transform.localScale.y / 2, Mathf.Round(hit.point.z / gridSize) * gridSize);



                        GameObject newAgent = Instantiate(spawnAgent, spawnLocation, Quaternion.identity);
                        newAgent.GetComponent<agent1Behavior>().isActive = true;
                        Instantiate(structure, spawnLocation, Quaternion.identity);


                        gridList[i].structureAmount = gridList[i].structureAmount + 1;

                    }
                }
            }
        }
    }
}
