using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agent1Behavior : MonoBehaviour
{
    [SerializeField] GameObject structure;
    [SerializeField] GameObject marker;
    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] int maxBuildings;


    string[] radiusTags =
         {
                 "structure",
                 "point",
          
         };


    private int gridSize;

    private GameObject ground;
    private GameObject[] mainPoint;

    public Grid grid;

    public Camera cam;
    public NavMeshAgent agent;

    bool canBuild = false;

    Vector3 agentMoveLocation;


    private mainPoint pointScript;
    float pointRadius;

   


    GameObject spawnController;
    private SpawnSettings spawnerScript;

    private Grid gridScript;

    List<GridList> gridList;


    void Start()
    {

        ground = GameObject.FindGameObjectWithTag("ground");



        spawnController = GameObject.Find("spawnController");
        spawnerScript = spawnController.GetComponent<SpawnSettings>();

        gridList = spawnerScript.gridList; 
        gridSize = spawnerScript.gridSize;

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


        CheckBuild();


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

        pointScript = closestMainPoint.GetComponent<mainPoint>();

        pointRadius = pointScript.radius;

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
}
