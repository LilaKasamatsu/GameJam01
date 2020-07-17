using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class ObjectiveSpawn : MonoBehaviour
{
    [SerializeField] GameObject objectiveCube;
    [SerializeField] GameObject MegaCube;
    [SerializeField] GameObject objectiveRay;
    [SerializeField] int trysPerFrame;
    [SerializeField] float minDistanceBetweenTwoSpawns;
    [SerializeField] float startingHeight;
    [SerializeField] float Heightincrease;
    [SerializeField] float maximumHeight;
    [SerializeField] int AgentsPerCube;
    [Range(0, 1)] public float objectiveFallingSpeed;
    public float objectiveCountdown;

    public int collectedCubes = 0;

    [SerializeField] Image cube1;
    [SerializeField] Image cube2;
    [SerializeField] Image cube3;
    [SerializeField] Image cube4;
    [SerializeField] Image cube5;
    [SerializeField] GameObject particles;


    public static ObjectiveSpawn instance;
    float currentHeight;
    Vector3 lastPosition;
    Vector3 spawnPosition;

    Camera cam;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        cam = Camera.main;
        currentHeight = startingHeight;
        StartCoroutine(SpawnCube(1));

    }
    


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //StartCoroutine( SpawnCube(2));
            collectedCubes += 1;

        }

        if (collectedCubes == 1)
        {
            cube1.GetComponent<Image>().color = new Color(255, 255, 255, 100);
            cube1.transform.position = new Vector3(cube1.transform.position.x, Mathf.Lerp(cube1.transform.position.y, cube1.transform.parent.transform.position.y, 0.05f), cube1.transform.position.z);
               
        }
        if (collectedCubes == 2)
        {
            cube2.GetComponent<Image>().color = new Color(255, 255, 255, 100);
            cube2.transform.position = new Vector3(cube2.transform.position.x, Mathf.Lerp(cube2.transform.position.y, cube2.transform.parent.transform.position.y, 0.05f), cube2.transform.position.z);

        }
        if (collectedCubes == 3)
        {
            cube3.GetComponent<Image>().color = new Color(255, 255, 255, 100);
            cube3.transform.position = new Vector3(cube3.transform.position.x, Mathf.Lerp(cube3.transform.position.y, cube3.transform.parent.transform.position.y, 0.05f), cube3.transform.position.z);

        }
        if (collectedCubes == 4)
        {
            cube4.GetComponent<Image>().color = new Color(255, 255, 255, 100);
            cube4.transform.position = new Vector3(cube4.transform.position.x, Mathf.Lerp(cube4.transform.position.y, cube4.transform.parent.transform.position.y, 0.05f), cube4.transform.position.z);

        }
        if (collectedCubes == 5)
        {
            cube5.GetComponent<Image>().color = new Color(255, 255, 255, 100);
            cube5.transform.position = new Vector3(cube5.transform.position.x, Mathf.Lerp(cube5.transform.position.y, cube5.transform.parent.transform.position.y, 0.05f), cube5.transform.position.z);

        }

    }

    public IEnumerator SpawnCube(int amountOfCubes)
    {
        int hasSpawned = amountOfCubes;
        int tryCounter = 0;

        while (hasSpawned >0)
        {
            float x = Random.Range(5* GridArray.Instance.cellSize, GridArray.Instance.arrayX * GridArray.Instance.cellSize - GridArray.Instance.cellSize*5);
            float z = Random.Range(5 * GridArray.Instance.cellSize, GridArray.Instance.arrayZ * GridArray.Instance.cellSize - GridArray.Instance.cellSize*5);
            float y = 100;

            spawnPosition = new Vector3(x, y, z);


            int layer_mask = LayerMask.GetMask("ObjectiveSpawn");

            RaycastHit hit;


            if ( Vector3.Distance( new Vector3(spawnPosition.x,0,spawnPosition.z),new Vector3(lastPosition.x,0,lastPosition.z))>minDistanceBetweenTwoSpawns && Physics.Raycast(spawnPosition, new Vector3(0, -1, 0), out hit, Mathf.Infinity, layer_mask) && hit.transform.GetComponent<GroundBehaviour>().falling==false)
            {
                
                spawnPosition.y =currentHeight;
                spawnPosition.y= Mathf.Clamp(spawnPosition.y, hit.point.y + 20, Mathf.Infinity);
                currentHeight += Heightincrease;
                currentHeight= Mathf.Clamp(currentHeight, startingHeight, maximumHeight);

                hasSpawned -= 1;
                if (collectedCubes < 4)
                {
                    Instantiate(objectiveCube, spawnPosition, Quaternion.identity).GetComponent<ObjectiveCubeBehavior>().amountOfAgents = AgentsPerCube;
                }
                else
                    Instantiate(MegaCube, spawnPosition, Quaternion.identity);
                lastPosition = spawnPosition;
            }
            tryCounter += 1;

            if (tryCounter >= trysPerFrame)
            {
                tryCounter = 0;
                yield return new WaitForEndOfFrame();
            }

        }
    }


}
