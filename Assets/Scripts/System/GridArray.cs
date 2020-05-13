using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridArray : MonoBehaviour
{
    [SerializeField] GameObject marker;
    [SerializeField] int agentPlaceAmount;
    [SerializeField] int startingAgentFound;
    [SerializeField] int startingAgentPoint;
    [SerializeField] LevelGenerator levelGenerator;


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
    public int cellY = 2;
    public int maxStructures;



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
                float width = Random.Range(0f, 1f);
                int color = Random.Range(0, 3);


                gridList.Add(new GridList( 0, 0, 0, 0, width, "nope", color));
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
        levelGenerator.GenerateMap();
        levelGenerator.surface.BuildNavMesh();


        ground = levelGenerator.Groundbounds;

        arrayX = Mathf.RoundToInt(ground.transform.localScale.x / cellSize) - 1;
        arrayZ = Mathf.RoundToInt(ground.transform.localScale.z / cellSize) - 1;

        CreateGrid();


        gridArray = new GridList[arrayX, arrayZ];

        //Saving available agents
        agentStack = new AgentStack(agentPlaceAmount, 0, 0, 0);

        

        for (int x = 0; x < arrayX; x++)
        {
            for (int z = 0; z < arrayZ; z++)
            {
                //Debug.Log(x + " , " + i);

                float width = Random.Range(0f, 1f);
                int range = Random.Range(0, 3);
                int color = Random.Range(0, 3);
                
                string shape = "circ";
                if (range == 0)
                {
                    shape = "squ";
                }
                if (range == 1)
                {
                    shape = "tri";
                }
                if (range == 2)
                {
                    shape = "circ";
                }

                gridArray[x, z] = new GridList( 0, 0, 0, 0, width, shape, color);

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
        while ( 1 == 1)
        {
            yield return new WaitForSeconds(Random.Range(minNewAgentDelay, maxNewAgentDelay));

            agentStack.agentAmount += newAgentAmount;
        }
                     
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

    float minDist = Mathf.Infinity;
    public Vector3 GetClosestTarget(List<Vector3> target, Vector3 position)
    {
        Vector3 tMin = position;
        float minDist = Mathf.Infinity;

        foreach (Vector3 t in target)
        {
            float dist = Vector3.Distance(t, position);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }
    public List<Vector3> searchForStructure(Vector3 target,float searchDistance)
    {
        int minX = NumToGrid(target.x - searchDistance);
        minX = Mathf.Clamp(minX, 0, arrayX-1);
        int minZ = NumToGrid(target.z - searchDistance);
        minZ = Mathf.Clamp(minZ, 0, arrayZ-1);

        int maxX = NumToGrid(target.x + searchDistance);
        maxX =Mathf.Clamp(maxX, 0, arrayX-1);
        int maxZ = NumToGrid(target.z + searchDistance);
        maxZ=Mathf.Clamp(maxZ, 0, arrayZ-1);
        



        //Search Grid for structures/main points
        //The list orientPositions saves the Vector3 of all structures, that the agent has to orient on
        List<Vector3> orientPositions = new List<Vector3>();
        for (int x = minX; x >= minX && x <= maxX; x++)
        {
            for (int z = minZ; z >= minZ && z <= maxZ ; z++)
            {
                if (gridArray[x, z].sizeY > 0 && target.y == gridArray[x,z].foundationObject.transform.position.y)
                {
                    //return that this position has a strucutre to orient on.
                    orientPositions.Add(new Vector3(x * cellSize, 0, z * cellSize));
                }
            }
        }
        return orientPositions;
    }
    public List<Vector3> searchForFoundations(Vector3 target, float searchDistance)
    {
        int minX = NumToGrid(target.x - searchDistance);
        int minZ = NumToGrid(target.z - searchDistance);

        int maxX = NumToGrid(target.x + searchDistance);
        int maxZ = NumToGrid(target.z + searchDistance);



        //Search Grid for structures/main points
        //The list orientPositions saves the Vector3 of all structures, that the agent has to orient on
        List<Vector3> orientPositions = new List<Vector3>();
        for (int x = minX; x >= minX && x <= maxX; x++)
        {
            for (int z = minZ; z >= minZ && z <= maxZ; z++)
            {
                if (x < arrayX && x >= 0 && z < arrayZ && z > 0 && gridArray[x, z].foundationAmount > 0)
                {
                    //return that this position has a strucutre to orient on.
                    orientPositions.Add(new Vector3(x * cellSize, 0, z * cellSize));
                }
            }
        }
        return orientPositions;
    }

}
