using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawn : MonoBehaviour
{
    [SerializeField] GameObject objectiveCube;
    [SerializeField] GameObject objectiveRay;

    Vector3 spawnPosition;
    void Start()
    {
   


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bool hasSpawned = false;

            while (hasSpawned == false)
            {
                float x = Random.Range(0 + GridArray.Instance.cellSize, GridArray.Instance.arrayX * GridArray.Instance.cellSize - GridArray.Instance.cellSize);
                float z = Random.Range(0 + GridArray.Instance.cellSize, GridArray.Instance.arrayZ * GridArray.Instance.cellSize - GridArray.Instance.cellSize);
                float y = 100;

                spawnPosition = new Vector3(x, y, z);


                int layer_mask = LayerMask.GetMask("Ground");

                RaycastHit hit;


                if (Physics.Raycast(spawnPosition, new Vector3(0, -1, 0), out hit, Mathf.Infinity, layer_mask))
                {
                    Debug.Log("Did Hit");
                    spawnPosition.y = hit.point.y + 30;

                    hasSpawned = true;
                    Instantiate(objectiveCube, spawnPosition, Quaternion.identity);
                    Instantiate(objectiveRay, spawnPosition, Quaternion.identity);
                }


            }


         
        }
    }
}
