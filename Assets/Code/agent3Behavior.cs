using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class agent3Behavior : MonoBehaviour
{
    [SerializeField] GameObject structure;
    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] int maxBuildings;
    [SerializeField] GameObject spawnAgent;



    private GameObject ground;

    public Grid grid;
    public Camera cam;
    public NavMeshAgent agent;

    public bool isActive = false;
    bool canBuild = false;
    bool canSpawn = false;
    private int gridSize;



    public float pointRadius;
    public float maxDestinationDistance;

    Vector3 agentMoveLocation;
    List<Vector3> orientPositions;

    GridList[,] gridArray;

    void Start()
    {
        cam = Camera.main;
        ground = GameObject.FindGameObjectWithTag("ground");
        
        gridSize = SpawnSettings.Instance.gridSize;
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

      
        

        if (isActive == false)
        {
            SpawnActiveAgent();

        }
    }

 

    IEnumerator MoveTimer()
    {
        //while loop is a bit more elegant in an coroutine than stopping and starting it over and over again-Philip
        while (isActive == true)
        {
            BuildFoundation();


            yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));

            //DIESER CODE IST DER NEUE
            // instead of working throw the same list twice, once to change the bool and then a second time to cope the true once you could just skipp the first steps-Philip

            //Search Grid for structures/main points
            //The list orientPositions saves the Vector3 of all structures, that the agent has to orient on


            orientPositions = new List<Vector3>();

            int minX = Mathf.RoundToInt( (transform.position.x ) / gridSize - maxDestinationDistance) ;
            int maxX = Mathf.RoundToInt( (transform.position.x ) / gridSize + maxDestinationDistance)  ;

            int minZ = Mathf.RoundToInt( (transform.position.z ) / gridSize - maxDestinationDistance) ;
            int maxZ = Mathf.RoundToInt( (transform.position.z ) / gridSize + maxDestinationDistance) ;


            orientPositions = new List<Vector3>();
            for (int x = minX; x >= minX  && x <= maxX && x < GridArray.Instance.arrayX && x > 0; x++)
            {
                for (int z = minZ; z >= minZ && z <= maxZ && z < GridArray.Instance.arrayZ && z > 0; z++)
                {

                    if (gridArray[x, z].foundationAmount > 0)
                    {

                        //return that this position has a strucutre to orient on.
                        orientPositions.Add(new Vector3(x * gridSize, transform.position.y, z * gridSize));

                    }
                }
            }

            //"GetClosestTarget" then compares all of those Vector3 and finds the closest
            Vector3 closestMainPoint = GetClosestTarget(orientPositions);

            agentMoveLocation = new Vector3(closestMainPoint.x + Random.Range(-pointRadius, pointRadius), transform.position.y, closestMainPoint.z + Random.Range(-pointRadius, pointRadius));
                  
            canBuild = true;                        
            agent.SetDestination(agentMoveLocation);

        }

        

    }

    Vector3 GetClosestTarget(List<Vector3> target)
    {
        Vector3 tMin = transform.position;
        Vector3 currentPos = transform.position;
        float minDist = 30f;
 
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
        gridArray = GridArray.Instance.gridArray;
        if (!agent.hasPath && canBuild == true)
        {
            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, structure.transform.localScale.y / 2, Mathf.Round(transform.position.z / gridSize) * gridSize);
            canBuild = false;

            int arrayPosX = Mathf.RoundToInt(buildLocation.x) / gridSize;
            int arrayPosZ = Mathf.RoundToInt(buildLocation.z) / gridSize;
            //To get rid off some outside of bounds errors -Philip
            if (arrayPosX >= 0 && arrayPosX < Mathf.Sqrt(gridArray.Length) && arrayPosZ >= 0 && arrayPosZ < Mathf.Sqrt(gridArray.Length))
            {
                if (gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount <= 0)
                {

                    Instantiate(structure, buildLocation, Quaternion.identity);
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


        if (Input.GetMouseButtonDown(1))
        {
            Destroy(this.gameObject);
        }

        if (Input.GetMouseButtonUp(0))
        {
            canSpawn = true;
        }

        if (Input.GetMouseButton(0) == true && canSpawn == true && !IsOverUi())
        {


            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layer_mask = LayerMask.GetMask("Ground");

            gridArray = GridArray.Instance.gridArray;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, Mathf.Round(hit.point.y / gridSize) * gridSize, Mathf.Round(hit.point.z / gridSize) * gridSize);

                int arrayPosX = Mathf.RoundToInt(hitGrid.x) / gridSize;
                int arrayPosZ = Mathf.RoundToInt(hitGrid.z) / gridSize;
                //To get rid off some outside of bounds errors -Philip
                if (arrayPosX >= 0 && arrayPosX < Mathf.Sqrt(gridArray.Length) && arrayPosZ >= 0 && arrayPosZ < Mathf.Sqrt(gridArray.Length))
                {
                    if (gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount <= 0)
                    {

                        Vector3 spawnLocation = hitGrid;
                        spawnLocation.y = structure.transform.localScale.y / 2;

                        GameObject newAgent = Instantiate(spawnAgent, spawnLocation + new Vector3(0, spawnAgent.transform.localScale.y / 2), Quaternion.identity);
                        newAgent.GetComponent<agent3Behavior>().isActive = true;


                        Instantiate(structure, spawnLocation, Quaternion.identity);
                        gridArray[arrayPosX, arrayPosZ].foundationAmount += 1;

                    }
                }
            }
        }
    }

}
