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



        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.1f), transform.position.z);
        


    }

    private void Start()
    {
        cellSize =  GridArray.Instance.cellSize;
        //SetHeight();
    }

    IEnumerator SetHeight()
    {
        while (1 == 1)
        {
            yield return new WaitForSeconds(0.001f);

            int arrayPosX = Mathf.RoundToInt(transform.position.x / cellSize);
            int arrayPosZ = Mathf.RoundToInt(transform.position.z / cellSize);
            float parentY = transform.parent.position.y;

            gridArray = GridArray.Instance.gridArray;


            transform.position = new Vector3(transform.position.x, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, transform.position.z);


        }
    }
}
