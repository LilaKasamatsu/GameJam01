using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAgentHeight : MonoBehaviour
{
    GridList[,] gridArray;
    int cellSize;

    private void Update()
    {


        int arrayPosX = Mathf.RoundToInt(transform.position.x / cellSize);
        int arrayPosZ = Mathf.RoundToInt(transform.position.z / cellSize);
        float parentY = transform.parent.position.y;
        //float lerpY = Mathf.Lerp(transform.position.y, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.1f);

        gridArray = GridArray.Instance.gridArray;


        if(gridArray[arrayPosX, arrayPosZ].structureAmount > 0 && arrayPosX >= 0 && arrayPosX <= GridArray.Instance.arrayX && arrayPosZ >= 0 && arrayPosZ <= GridArray.Instance.arrayZ)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.05f), transform.position.z);

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

    }

}
