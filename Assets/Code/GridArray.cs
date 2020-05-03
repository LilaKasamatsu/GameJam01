using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridArray : MonoBehaviour
{
    public GridList[,] gridArray;
    private GameObject ground;
    public int arrayX;
    public int arrayZ;


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
           
        int gridSize = SpawnSettings.Instance.gridSize;
        ground = GameObject.FindGameObjectWithTag("ground");

        arrayX = Mathf.RoundToInt(ground.transform.localScale.x / gridSize);
        arrayZ = Mathf.RoundToInt(ground.transform.localScale.z / gridSize);

        gridArray = new GridList[arrayX, arrayZ];


        for (int x = 0; x < arrayX; x++)
        {
            for (int z = 0; z < arrayZ; z++)
            {
                //Debug.Log(x + " , " + i);

                gridArray[x, z] = new GridList( 0, 0, 0, 0, 0 );

            }
        }
    }

    public int NumToGrid(float i)
    {
        i = Mathf.RoundToInt(i / SpawnSettings.Instance.gridSize) * SpawnSettings.Instance.gridSize;
        return (int) i;
    }

    public Vector3 VectorToGrid(Vector3 i)
    {
        i.x = Mathf.RoundToInt(i.x / SpawnSettings.Instance.gridSize) * SpawnSettings.Instance.gridSize;
        i.y = Mathf.RoundToInt(i.y / SpawnSettings.Instance.gridSize) * SpawnSettings.Instance.gridSize;
        i.z = Mathf.RoundToInt(i.z / SpawnSettings.Instance.gridSize) * SpawnSettings.Instance.gridSize;
        return i;
    }

}
