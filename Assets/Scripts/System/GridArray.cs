﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridArray : MonoBehaviour
{
    [SerializeField] GameObject marker;
    [SerializeField] int startingAgentStruc;
    [SerializeField] int startingAgentFound;
    [SerializeField] int startingAgentPoint;

    [SerializeField] float minNewAgentDelay = 2;
    [SerializeField] float maxNewAgentDelay = 6;
    public int newAgentAmount;


    public GridList[,] gridArray;
    public AgentStack agentStack;
    private GameObject ground;
    public int arrayX;
    public int arrayZ;

    public int gridSize = 3;

    public List<GridList> gridList = new List<GridList>();

    private int[,] gridDimensions;
    public int cellSize = 3;


    public void CreateGrid()
    {
        gridDimensions = new int[arrayX, arrayZ];
        int setSquare = Random.Range(0, 100);

        for (int x = 0; x < gridDimensions.GetLength(0); x++)
        {

            for (int z = 0; z < gridDimensions.GetLength(1); z++)
            {

                //Visual Grid

                //GameObject gridSquare = Instantiate(marker, new Vector3(x * cellSize, 0 - marker.transform.localScale.y / 2, z * cellSize), Quaternion.identity) as GameObject;

                /*
                if ( setSquare > 40)
                {
                    GameObject gridSquare = Instantiate(marker, new Vector3(x * cellSize, 0 -  marker.transform.localScale.y/2, z * cellSize), Quaternion.identity) as GameObject;
                    

                }
                else
                {
                    setSquare = Random.Range(30, 50);

                }
                */

                gridList.Add(new GridList(Mathf.RoundToInt(x * cellSize), Mathf.RoundToInt(z * cellSize), 0, 0, 0));
            }
        }
    }


    //Singleton
    public static GridArray Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
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


        ground = GameObject.FindGameObjectWithTag("ground");

        arrayX = Mathf.RoundToInt(ground.transform.localScale.x / cellSize) - 1;
        arrayZ = Mathf.RoundToInt(ground.transform.localScale.z / cellSize) - 1;

        CreateGrid();


        gridArray = new GridList[arrayX, arrayZ];

        //Saving available agents
        agentStack = new AgentStack(startingAgentStruc, startingAgentFound, startingAgentPoint);

        

        for (int x = 0; x < arrayX; x++)
        {
            for (int z = 0; z < arrayZ; z++)
            {
                //Debug.Log(x + " , " + i);

                gridArray[x, z] = new GridList( 0, 0, 0, 0, 0 );

            }
        }
        StartCoroutine(AddAgentStack());

    }

  


    public bool CheckArrayBounds(int gridX, int gridZ)
    {
        if (gridX >= 0 && gridX < arrayX && gridZ >= 0 && gridZ < arrayZ)
        {
            return true;
        }

        
        return false;
    }

    IEnumerator AddAgentStack()
    {
        yield return new WaitForSeconds(Random.Range(minNewAgentDelay, maxNewAgentDelay));

        int randomAgentDrop = Random.Range(0, 100);

        if (randomAgentDrop < 50)
        {
            agentStack.agentStructure += newAgentAmount;

        }
        else if (randomAgentDrop > 50)
        {
            agentStack.agentFoundation += newAgentAmount;
            
        }
        StartCoroutine(AddAgentStack());


    }
    public int NumToGrid(float i)
    {
        i = Mathf.RoundToInt(i / cellSize);
        return (int) i;
    }

    public int RoundToGrid(float i)
    {
        i = Mathf.RoundToInt(i / cellSize) * cellSize;
        return (int)i;
    }

    public Vector3 VectorToGrid(Vector3 i)
    {
        i.x = Mathf.RoundToInt(i.x / cellSize) * cellSize;
        i.y = Mathf.RoundToInt(i.y / cellSize) * cellSize;
        i.z = Mathf.RoundToInt(i.z / cellSize) * cellSize;
        return i;
    }

 

    public Vector3 GetClosestTarget(List<Vector3> target, Vector3 position)
    {
        Vector3 tMin = position;
        Vector3 currentPos = position;
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
}
