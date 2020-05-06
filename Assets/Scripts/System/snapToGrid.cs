using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapToGrid : MonoBehaviour
{

    private int gridSize;
    
    void Start()
    {

        

        gridSize = GridArray.Instance.cellSize;


        transform.position = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, Mathf.Round(transform.position.y / (gridSize/2)) * (gridSize/2), Mathf.Round(transform.position.z / gridSize) * gridSize);
    }
    
}
