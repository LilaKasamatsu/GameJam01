﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSettings : MonoBehaviour
{

    [SerializeField] GameObject agentStruc;
    [SerializeField] GameObject agentPoint;
    [SerializeField] GameObject agentFound;

    [SerializeField] GameObject foundation;
    [SerializeField] GameObject structure;
    [SerializeField] GameObject mainPoint;
    [SerializeField] GameObject marker;
    [SerializeField] GameObject buildMarker;

    private GameObject ground;

    Camera cam;

    public Grid grid;
    public int cellSize = 3;
    public List<GridList> gridList = new List<GridList>();
    public bool spawnMode = false;

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

    AgentStack agentStack;

    private void Start()
    {
        cam = Camera.main;
        ground = GameObject.FindGameObjectWithTag("ground");
        //grid = new Grid(Mathf.RoundToInt(ground.transform.localScale.x / cellSize) + 1, Mathf.RoundToInt(ground.transform.localScale.z / cellSize) + 1, cellSize);
        
        cellSize = GridArray.Instance.cellSize;
        agentStack = GridArray.Instance.agentStack;


        //CreateStartPoint();

    }

    public void CreateStartPoint()
    {
        int randomX = Random.Range(0, GridArray.Instance.arrayX * cellSize);
        int randomZ = Random.Range(0, GridArray.Instance.arrayZ * cellSize);


        int maxTries = 0;
        while (maxTries < 12 && GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(randomX), GridArray.Instance.NumToGrid(randomZ)].foundationAmount > 0)
        {
            maxTries++;
            randomX = Random.Range(0, GridArray.Instance.arrayX * cellSize);
            randomZ = Random.Range(0, GridArray.Instance.arrayZ * cellSize);
        }

        if (maxTries >= 12)
        {
            Debug.Log("More than 12 spawn tries");
        }

        Instantiate(mainPoint, new Vector3(randomX, 3f, randomZ), Quaternion.identity);
        GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(randomX), GridArray.Instance.NumToGrid(randomZ)].pointAmount += 1;

        int minX = GridArray.Instance.NumToGrid(randomX) - 1;
        int maxX = GridArray.Instance.NumToGrid(randomX) + 1;
        int minZ = GridArray.Instance.NumToGrid(randomZ) - 1;
        int maxZ = GridArray.Instance.NumToGrid(randomZ) + 1;


        for (int x = minX; x >= minX && x <= maxX && x < GridArray.Instance.arrayX && x > 0; x++)
        {
            for (int z = minZ; z >= minZ && z <= maxZ && z < GridArray.Instance.arrayZ && z > 0; z++)
            {
                GridArray.Instance.gridArray[x, z].foundationAmount += 1;

            }
        }
    }


    public void PlaceAgent(GameObject agent)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layer_mask = LayerMask.GetMask("Ground");

        GridList[,] gridArray = GridArray.Instance.gridArray;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
        {
            Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / cellSize) * cellSize, Mathf.Round(hit.point.y / cellSize) * cellSize, Mathf.Round(hit.point.z / cellSize) * cellSize);

            int arrayPosX = GridArray.Instance.NumToGrid(hitGrid.x); 
            int arrayPosZ = GridArray.Instance.NumToGrid(hitGrid.z);

            //Foundation Agent
            if (agent.GetComponent<AgentFoundation>() != null && agentStack.agentFoundation > 0)
            {
                if ( GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ) && gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 )
                {
                    agentStack.agentFoundation -= 1;

                    Vector3 spawnLocation = hitGrid;
                    spawnLocation.y = foundation.transform.localScale.y / 2;

                    GameObject newAgent = Instantiate(agent, spawnLocation + new Vector3(0, agent.transform.localScale.y ), Quaternion.identity);
                    newAgent.GetComponent<AgentFoundation>().isActive = true;


                    //Instantiate(foundation, spawnLocation, Quaternion.identity);
                    //GridArray.Instance.gridArray[arrayPosX, arrayPosZ].foundationAmount += 1;

                }
            }
            
            //Structure Agent
            if (agent.GetComponent<AgentStructure>() != null && agentStack.agentStructure > 0)
            {
                if (gridArray[arrayPosX, arrayPosZ].structureAmount <= 0 && gridArray[arrayPosX, arrayPosZ].pointAmount <= 0 && gridArray[arrayPosX, arrayPosZ].foundationAmount > 0)
                {
                    agentStack.agentStructure -= 1;

                    Vector3 spawnLocation = hitGrid;
                    spawnLocation.y = structure.transform.localScale.y / 2;

                    GameObject newAgent = Instantiate(agent, spawnLocation + new Vector3(0, agent.transform.localScale.y / 2), Quaternion.identity);
                    newAgent.GetComponent<AgentStructure>().isActive = true;
                                                    

                    GameObject builtStructure = Instantiate(structure, spawnLocation, Quaternion.identity) as GameObject;
                    GridArray.Instance.gridArray[arrayPosX, arrayPosZ].structureObjects.Add(builtStructure);


                    GridArray.Instance.gridArray[arrayPosX, arrayPosZ].structureAmount += 1;

                }
            }



        }


    }

    public void SpawnAgent(GameObject spawnAgent, Vector3 position)
    {
        

        //Instantiate(spawnAgent, spwnLocation, cam.transform.rotation);

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f;  // Preview Agent is 10 units in front of the camera
        mousePos.y = 125.0f;  // Preview Agent is a bit higher than the mouse cursor

        Vector3 objectPos = cam.ScreenToWorldPoint(mousePos);
        Instantiate(spawnAgent, objectPos, cam.transform.rotation);
        CreateBuildMarker();

    }

    private void CreateBuildMarker()
    {
        Instantiate(buildMarker, new Vector3(0, 0, 0), Quaternion.identity);
              
    

    }
    void Update()
    {
        NumberSpawning();
        GetGridValue();

    }
    private void NumberSpawning()
    {
        //Mouse Left
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("ground"))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / cellSize) * cellSize, Mathf.Round(hit.point.y / cellSize) * cellSize, Mathf.Round(hit.point.z / cellSize) * cellSize);


                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].foundationAmount <= 0)
                    {
                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / cellSize) * cellSize, foundation.transform.localScale.y / 2, Mathf.Round(hit.point.z / cellSize) * cellSize);
                        Instantiate(agentFound, spawnLocation, Quaternion.identity);
                        Instantiate(foundation, spawnLocation, Quaternion.identity);

                        gridList[i].foundationAmount = gridList[i].foundationAmount + 1;
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
            {

                PlaceAgent(agentStruc);

                
            }
        }


        //Main Point Spawner
        if (Input.GetKey(KeyCode.Alpha1))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;



            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("ground"))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / cellSize) * cellSize, Mathf.Round(hit.point.y / cellSize) * cellSize, Mathf.Round(hit.point.z / cellSize) * cellSize);

                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].structureAmount <= 0 && gridList[i].pointAmount <= 0)
                    {
                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / cellSize) * cellSize, mainPoint.transform.localScale.y / 2, Mathf.Round(hit.point.z / cellSize) * cellSize);
                        Instantiate(agentPoint, spawnLocation, Quaternion.identity);
                        Instantiate(mainPoint, spawnLocation, Quaternion.identity);

                        gridList[i].pointAmount = gridList[i].pointAmount + 1;
                    }
                }
            }
        }
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

                Vector3 hitPosition = hit.point;

                hitPosition.x = Mathf.Round(hit.point.x / cellSize) * cellSize;
                hitPosition.y = Mathf.Round(hit.point.y / cellSize) * cellSize;
                hitPosition.z = Mathf.Round(hit.point.z / cellSize) * cellSize;

                int hitX = GridArray.Instance.NumToGrid(hitPosition.x);
                int hitZ = GridArray.Instance.NumToGrid(hitPosition.z);

                if (GridArray.Instance.CheckArrayBounds(hitX, hitZ))
                {
                    Debug.Log("; Foundation: " + GridArray.Instance.gridArray[hitX, hitZ].foundationAmount + "; Structures: " + GridArray.Instance.gridArray[hitX, hitZ].structureAmount + "; Point: " + GridArray.Instance.gridArray[hitX, hitZ].pointAmount);

                }

            }
        }
    }

}
