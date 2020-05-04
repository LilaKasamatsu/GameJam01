using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridArray : MonoBehaviour
{
    [SerializeField] GameObject marker;
    public GridList[,] gridArray;
    private GameObject ground;
    public int arrayX;
    public int arrayZ;

    public int gridSize = 3;

    public List<GridList> gridList = new List<GridList>();

    private int[,] gridDimensions;
    public int cellSize = 3;


    public void CreateGrid()
    {
        gridDimensions = new int[Mathf.RoundToInt(ground.transform.localScale.x / cellSize) + 1, Mathf.RoundToInt(ground.transform.localScale.z / cellSize) + 1];
        int setSquare = Random.Range(0, 100);

        for (int x = 0; x < gridDimensions.GetLength(0); x++)
        {

            for (int z = 0; z < gridDimensions.GetLength(1); z++)
            {

                //Visual Grid
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

    void GenerateGridSquares()
    {

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
        CreateGrid();

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
        i = Mathf.RoundToInt(i / SpawnSettings.Instance.cellSize) * SpawnSettings.Instance.cellSize;
        return (int) i;
    }

    public Vector3 VectorToGrid(Vector3 i)
    {
        i.x = Mathf.RoundToInt(i.x / SpawnSettings.Instance.cellSize) * SpawnSettings.Instance.cellSize;
        i.y = Mathf.RoundToInt(i.y / SpawnSettings.Instance.cellSize) * SpawnSettings.Instance.cellSize;
        i.z = Mathf.RoundToInt(i.z / SpawnSettings.Instance.cellSize) * SpawnSettings.Instance.cellSize;
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
