using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapToGrid : MonoBehaviour
{

    private int gridSize;
    private int cellY;
    
    void Start()
    {

        

        gridSize = GridArray.Instance.cellSize;
        cellY = GridArray.Instance.cellY;


        transform.position = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, Mathf.Round(transform.position.y), Mathf.Round(transform.position.z / gridSize) * gridSize);
    }
    
}
