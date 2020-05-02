﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSettings : MonoBehaviour
{

    [SerializeField] GameObject agent;
    [SerializeField] GameObject agent2;
    [SerializeField] GameObject agent3;

    [SerializeField] GameObject foundation;
    [SerializeField] GameObject structure;
    [SerializeField] GameObject mainPoint;


    public int gridSize = 3;
    [SerializeField] GameObject marker;

    //Debug Right mouse
    int rightPressed = 0;

    public Camera cam;

    public Grid grid;

    private GameObject ground;

    public List<GridList> gridList = new List<GridList>();




    public void CreateGridObject(float x, float z)
    {
        //Visual Grid
        //Instantiate(marker, new Vector3(x * gridSize, 0, z * gridSize), Quaternion.identity);

        gridList.Add(new GridList(Mathf.RoundToInt(x * gridSize), Mathf.RoundToInt(z * gridSize), 0, 0, 0));

    }

    private void Start()
    {
        ground = GameObject.FindGameObjectWithTag("ground");
        grid = new Grid(Mathf.RoundToInt(ground.transform.localScale.x / gridSize) + 1, Mathf.RoundToInt(ground.transform.localScale.z / gridSize) + 1, gridSize);

    }




    public void SpawnAgent(GameObject spawnAgent, Vector3 position)
    {
        

        //Instantiate(spawnAgent, spwnLocation, cam.transform.rotation);

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f;  // Preview Agent is 10 units in front of the camera
        mousePos.y = 125.0f;  // Preview Agent is a bit higher than the mouse cursor

        Vector3 objectPos = cam.ScreenToWorldPoint(mousePos);
        Instantiate(spawnAgent, objectPos, cam.transform.rotation);
    }

    void Update()
    {
        //Mouse Left
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("ground"))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, Mathf.Round(hit.point.y / gridSize) * gridSize, Mathf.Round(hit.point.z / gridSize) * gridSize);


                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].foundationAmount <= 0)
                    {
                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, foundation.transform.localScale.y / 2, Mathf.Round(hit.point.z / gridSize) * gridSize);
                        Instantiate(agent3, spawnLocation, Quaternion.identity);
                        Instantiate(foundation, spawnLocation, Quaternion.identity);

                        gridList[i].foundationAmount = gridList[i].foundationAmount + 1;
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
            {
                Vector3 hitGrid = new Vector3( Mathf.Round(hit.point.x / gridSize) * gridSize, Mathf.Round(hit.point.y / gridSize) * gridSize, Mathf.Round(hit.point.z / gridSize) * gridSize);



                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].structureAmount <= 0 && gridList[i].foundationAmount > 0 && gridList[i].pointAmount <= 0)
                    {
                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, structure.transform.localScale.y / 2, Mathf.Round(hit.point.z / gridSize) * gridSize);
                        Instantiate(agent, spawnLocation, Quaternion.identity);
                        Instantiate(structure, spawnLocation, Quaternion.identity);

                        gridList[i].structureAmount = gridList[i].structureAmount + 1;


                    }
                }
            }
        }


        //Main Point Spawner
        if (Input.GetKey(KeyCode.Alpha1))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;



            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("ground"))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, Mathf.Round(hit.point.y / gridSize) * gridSize, Mathf.Round(hit.point.z / gridSize) * gridSize);

                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitGrid.x && gridList[i].z == hitGrid.z && gridList[i].structureAmount <= 0 && gridList[i].pointAmount <= 0)
                    {
                        Vector3 spawnLocation = new Vector3(Mathf.Round(hit.point.x / gridSize) * gridSize, mainPoint.transform.localScale.y / 2, Mathf.Round(hit.point.z / gridSize) * gridSize);
                        Instantiate(agent2, spawnLocation, Quaternion.identity);
                        Instantiate(mainPoint, spawnLocation, Quaternion.identity);

                        gridList[i].pointAmount = gridList[i].pointAmount + 1;
                    }
                }
            }
        }

        //Mouse Right
        if (Input.GetMouseButtonDown(1))
        {
           

            rightPressed += 1;

            //Debug.Log("click: " + rightPressed);

            
            Ray ray1 = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // && hit.collider.CompareTag("structure") || hit.collider.CompareTag("ground") ||  hit.collider.CompareTag("marker")

            if (Physics.Raycast(ray1, out hit))
            {
                //Debug.Log(grid.GetValue(hit.point));
                //Debug.Log(hit.collider.gameObject.name);
                //Debug.Log(grid.GetValue(hit.collider.gameObject.transform.position));

                Vector3 hitPosition = hit.collider.gameObject.transform.position;

                for (int i = 0; i < gridList.Count; i++)
                {
                    if (gridList[i].x == hitPosition.x && gridList[i].z == hitPosition.z)
                    {
                        Debug.Log(gridList[i].x + " " + gridList[i].z + "; Foundation: "+ gridList[i].foundationAmount +"; Structures: " + gridList[i].structureAmount + "; Point: " + gridList[i].pointAmount);
                    }
                }
            }
        }
    }
}
