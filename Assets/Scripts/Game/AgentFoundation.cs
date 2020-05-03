using System.Collections;
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

    void Start()
    {
        cam = Camera.main;
        ground = GameObject.FindGameObjectWithTag("ground");
        
        cellSize = GridArray.Instance.cellSize;
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
        //Looping and delaying their walk cycle 
        while (isActive == true)
        {
            BuildFoundation();
            
            yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));


            int minX = Mathf.RoundToInt( (transform.position.x ) / cellSize - maxDestinationDistance) ;
            int maxX = Mathf.RoundToInt( (transform.position.x ) / cellSize + maxDestinationDistance)  ;

            int minZ = Mathf.RoundToInt( (transform.position.z ) / cellSize - maxDestinationDistance) ;
            int maxZ = Mathf.RoundToInt( (transform.position.z ) / cellSize + maxDestinationDistance) ;

            //Search Grid for structures/main points
            //The list orientPositions saves the Vector3 of all structures, that the agent has to orient on
            orientPositions = new List<Vector3>();
            for (int x = minX; x >= minX  && x <= maxX && x < GridArray.Instance.arrayX && x > 0; x++)
            {
                for (int z = minZ; z >= minZ && z <= maxZ && z < GridArray.Instance.arrayZ && z > 0; z++)
                {

                    if (gridArray[x, z].foundationAmount > 0)
                    {

                        //return that this position has a strucutre to orient on.
                        orientPositions.Add(new Vector3(x * cellSize, transform.position.y, z * cellSize));

                    }
                }
            }

            //"GetClosestTarget" then compares all of those Vector3 inside the maximum range "maxDestinationDistance" and finds the closest
            Vector3 closestMainPoint = GridArray.Instance.GetClosestTarget(orientPositions, transform.position);

            agentMoveLocation = new Vector3(closestMainPoint.x + Random.Range(-pointRadius, pointRadius), transform.position.y, closestMainPoint.z + Random.Range(-pointRadius, pointRadius));
                  
            canBuild = true;                        
            agent.SetDestination(agentMoveLocation);

        }

        

    }


    private void BuildFoundation()
    {
        gridArray = GridArray.Instance.gridArray;
        if (!agent.hasPath && canBuild == true)
        {
            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / cellSize) * cellSize, foundation.transform.localScale.y / 2, Mathf.Round(transform.position.z / cellSize) * cellSize);
            canBuild = false;

            int arrayPosX = Mathf.RoundToInt(buildLocation.x) / cellSize;
            int arrayPosZ = Mathf.RoundToInt(buildLocation.z) / cellSize;

            if (gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount <= 0)
            {

                Instantiate(foundation, buildLocation, Quaternion.identity);
                gridArray[arrayPosX, arrayPosZ].foundationAmount += 1;

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

            SpawnSettings.Instance.PlaceAgent(spawnAgent); 
        }
    }

}
