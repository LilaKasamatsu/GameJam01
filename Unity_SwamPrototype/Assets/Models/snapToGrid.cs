using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapToGrid : MonoBehaviour
{

    private int gridSize;

    GameObject spawnController;
    private SpawnSettings spawnerScript;

    void Start()
    {

        spawnController = GameObject.Find("spawnController");
        spawnerScript = spawnController.GetComponent<SpawnSettings>();

        gridSize = spawnerScript.gridSize;


        transform.position = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, Mathf.Round(transform.position.y / (gridSize/2)) * (gridSize/2), Mathf.Round(transform.position.z / gridSize) * gridSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
