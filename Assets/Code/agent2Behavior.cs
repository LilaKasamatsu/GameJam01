using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agent2Behavior : MonoBehaviour
{


    [SerializeField] float minBuildDelay;
    [SerializeField] float maxBuildDelay;
    [SerializeField] GameObject mainPoint;
    [SerializeField] float moveRadius;

    private GameObject ground;

    public Grid grid;
    private int gridSize;


    public NavMeshAgent agent;
    private bool canBuild;
    Vector3 agentMoveLocation;
    Vector3 originPoint;
   


    GameObject spawnController;
    private SpawnSettings spawnerScript;

    List<GridList> gridList;


    // Start is called before the first frame update
    void Start()
    {
        spawnController = GameObject.Find("spawnController");
        spawnerScript = spawnController.GetComponent<SpawnSettings>();

        gridList = spawnerScript.gridList;
        gridSize = spawnerScript.gridSize;

        originPoint = transform.position;
        StartCoroutine(MoveTimer());

    }

    // Update is called once per frame
    void Update()
    {
        BuildStructure();


    }

    IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(Random.Range(minBuildDelay, maxBuildDelay));

        originPoint = transform.position;

        agentMoveLocation = new Vector3(originPoint.x + Random.Range(-moveRadius, moveRadius), originPoint.y, originPoint.z + Random.Range(-moveRadius, moveRadius));
        agent.SetDestination(agentMoveLocation);
        canBuild = true;
        StartCoroutine(MoveTimer());
    }

    private void BuildStructure()
    {

        if (!agent.hasPath && canBuild == true )
        {



            Vector3 buildLocation = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, mainPoint.transform.localScale.y / 2, Mathf.Round(transform.position.z / gridSize) * gridSize);
            canBuild = false;





            for (int i = 0; i < gridList.Count; i++)
            {
                if (gridList[i].x == buildLocation.x && gridList[i].z == buildLocation.z)
                {
                    if (gridList[i].structureAmount <= 0  && gridList[i].pointAmount <= 0)
                    {

                        Instantiate(mainPoint, buildLocation, Quaternion.identity);


                        gridList[i].pointAmount = gridList[i].pointAmount + 1;
                    }
                }
            }

        }
    }
}
