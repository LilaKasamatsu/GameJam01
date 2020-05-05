using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SetAgentHeight : MonoBehaviour
{
    GridList[,] gridArray;
    int cellSize;
    NavMeshAgent parentNav;

    private void Update()
    {


        int arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
        int arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);

        float parentY = transform.parent.position.y;
        //float lerpY = Mathf.Lerp(transform.position.y, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.1f);

        gridArray = GridArray.Instance.gridArray;

        
    

        if(GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ) && parentNav.isOnNavMesh)
        {

            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.07f), transform.position.z);

        }

    }

    private void Start()
    {
        cellSize =  GridArray.Instance.cellSize;
        //SetHeight();
    }
    private void Awake()
    {
        //gameObject.SetActive(false);
        parentNav = transform.parent.GetComponent<NavMeshAgent>();


    }

}
