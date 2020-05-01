using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agent3Behavior : MonoBehaviour
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
    private List<GameObject> mainPoint;
    private GameObject[] tagStructure;
    private GameObject[] tagPoint;


    public Grid grid;

    public Camera cam;
    public NavMeshAgent agent;

    bool canBuild = false;

    Vector3 agentMoveLocation;


    private mainPoint pointScript;
    public float pointRadius;




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

        

        tagStructure = GameObject.FindGameObjectsWithTag("structure");
        tagPoint = GameObject.FindGameObjectsWithTag("point");

        mainPoint = new List<GameObject>();
        for (int i = 0; i <= tagStructure.Length-1; i++)
        {
            mainPoint.Add(tagStructure[i ]);
        }
        for (int i = 0; i <= tagPoint.Length-1; i++)
        {
            mainPoint.Add(tagPoint[i]);
        }

     
        

        StartCoroutine(MoveTimer());


    }



    void Update()
    {
        grid = spawnerScript.grid;


        BuildFoundation();


    }


    IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));


        //mainPoint = GameObject.FindGameObjectsWithTag("point");

        /*
        foreach (string tag in radiusTags)
        {
            mainPoint = GameObject.FindGameObjectsWithTag(tag);


        }
        */

        tagStructure = GameObject.FindGameObjectsWithTag("structure");
        tagPoint = GameObject.FindGameObjectsWithTag("point");

        mainPoint = new List<GameObject>();
        for (int i = 0; i <= tagStructure.Length-1; i++)
        {
            mainPoint.Add(tagStructure[i]);
        }
        for (int i = 0; i <= tagPoint.Length-1; i++)
        {
            mainPoint.Add(tagPoint[i]);
        }


        GameObject closestMainPoint = GetClosestTarget(mainPoint);

        pointScript = closestMainPoint.GetComponent<mainPoint>();

        

        agentMoveLocation = new Vector3(closestMainPoint.transform.position.x + Random.Range(-pointRadius, pointRadius), transform.position.y, closestMainPoint.transform.position.z + Random.Range(-pointRadius, pointRadius));


        canBuild = true;


        agent.SetDestination(agentMoveLocation);

        StartCoroutine(MoveTimer());

    }

    GameObject GetClosestTarget(List<GameObject> target)
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

    private void BuildFoundation()
    {

        if (!agent.hasPath && canBuild == true)
        {
            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, structure.transform.localScale.y / 2, Mathf.Round(transform.position.z / gridSize) * gridSize);
            canBuild = false;


            //GridList gridValue = grid.GetValue(buildLocation);


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
}
