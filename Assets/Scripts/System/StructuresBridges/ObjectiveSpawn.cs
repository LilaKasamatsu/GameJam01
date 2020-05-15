using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class ObjectiveSpawn : MonoBehaviour
{
    [SerializeField] GameObject objectiveCube;
    [SerializeField] GameObject objectiveRay;
    [SerializeField] int trysPerFrame;
    [SerializeField] float minDistanceBetweenTwoSpawns;
    [SerializeField] float startingHeight;
    [SerializeField] float Heightincrease;
    [SerializeField] float maximumHeight;
    [SerializeField] int AgentsPerCube;
    [Range(0, 1)] public float objectiveFallingSpeed;
    public float objectiveCountdown;

    public static ObjectiveSpawn instance;
    float currentHeight;
    Vector3 lastPosition;
    Vector3 spawnPosition;
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

        currentHeight = startingHeight;
        StartCoroutine(SpawnCube(1));

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
                
                Debug.Log("Did Hit");
                spawnPosition.y =currentHeight;
                spawnPosition.y= Mathf.Clamp(spawnPosition.y, hit.point.y + 10, Mathf.Infinity);
                currentHeight += Heightincrease;
                currentHeight= Mathf.Clamp(currentHeight, startingHeight, maximumHeight);

                hasSpawned -= 1;
                Instantiate(objectiveCube, spawnPosition, Quaternion.identity).GetComponent<ObjectiveCubeBehavior>().amountOfAgents = AgentsPerCube;
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
