using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{

    //grid stuff

    private int height;
    private int width;
    private float cellSize;
    private int[,] gridArray;

    agent1Behavior agentScript;
    GameObject agent;

    SpawnSettings spawnerScript;
    GameObject spawnController;

    List<GridList> gridList;

    //public List<GridList> gridList = new List<GridList>();



    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        //agent = GameObject.FindGameObjectWithTag("agent1");
        //agentScript = agent.GetComponent<agent1Behavior>();

        spawnController = GameObject.Find("spawnController");
        spawnerScript = spawnController.GetComponent<SpawnSettings>();

        gridArray = new int[width, height];

        // #Debug Grid width + height
        //Debug.Log(width + " " + height);

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            
            

            for (int i = 0; i < gridArray.GetLength(1); i++)
            {
                // #Debug every grid width and height
                //Debug.Log(x + " , " + i);
                spawnerScript.CreateGridObject(x, i);

                //gridList.Add(new GridList (x, i, 0, 0));

                //spawnerScript.gridList.Add(new GridList(x, i, 0, 0, 0));

                //SetValue
            }
        }
    }




    private Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, z) * cellSize;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        z = Mathf.FloorToInt(worldPosition.z / cellSize);
    }

    public void SetValue(int x, int z, int valueStructure, int valuePoint, int valueFoundation)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            //gridArray[x, z] = value;


           

            for (int i = 0; i < spawnerScript.gridList.Count; i++)
            {
                if (spawnerScript.gridList[i].x == x && spawnerScript.gridList[i].z == z)
                {
                    spawnerScript.gridList[i] = new GridList(x, z, valueStructure, valuePoint, valueFoundation);
                 }
            }


        }
    }

    public void SetValue(Vector3 worldPosition, int valueStructure, int valuePoint, int valueFoundation)
    {
        int x, z;
        GetXY(worldPosition, out x, out z);
        SetValue(x, z, valueStructure, valuePoint, valueFoundation);
    }

    public GridList GetValue(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {



            for (int i = 0; i < spawnerScript.gridList.Count; i++)
            {
                if (spawnerScript.gridList[i].x == x && spawnerScript.gridList[i].z == z)
                {
                    return spawnerScript.gridList[i];
                }
            }

            return new GridList(x, z, 0, 0, 0);
            //return gridArray[x, z];


        }
        else
        {
            return new GridList(x, z, 0, 0, 0);
        }
    }

    public GridList GetValue(Vector3 worldPosition)
    {
        int x, z;
        GetXY(worldPosition, out x, out z);
        
        return GetValue(x, z);
    }

}
