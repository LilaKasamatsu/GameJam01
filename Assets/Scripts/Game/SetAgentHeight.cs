using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SetAgentHeight : MonoBehaviour
{
    GridList[,] gridArray;
    int cellSize;
    NavMeshAgent parentNav;

    bool hasSignal;
    Vector3 signalPos;
    float destMin;
    public float signalWalkSpeed = 0.05f;

    private void Update()
    {


        int arrayPosX = GridArray.Instance.NumToGrid(transform.position.x);
        int arrayPosZ = GridArray.Instance.NumToGrid(transform.position.z);

        Vector3 parentPos = transform.parent.position;
        //float lerpY = Mathf.Lerp(transform.position.y, parentY + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.1f);

        gridArray = GridArray.Instance.gridArray;

        if (hasSignal)
        {

            if ((transform.position.x < signalPos.x - destMin || transform.position.x > signalPos.x + destMin) &&
                (transform.position.z < signalPos.z - destMin || transform.position.z > signalPos.z + destMin))
            {
                transform.position = Vector3.Lerp(transform.position, signalPos, signalWalkSpeed);

            }
            else
            {
                Debug.Log("child finished path");
                hasSignal = false;
                transform.parent.gameObject.GetComponent<NavMeshAgent>().Warp(signalPos);
                transform.parent.gameObject.GetComponent<AgentStructure>().hasSignal = false;
                transform.parent.gameObject.GetComponent<AgentStructure>().StartCoroutine("MoveTimer");
            }
        }

        if (GridArray.Instance.CheckArrayBounds(arrayPosX, arrayPosZ) && parentNav.isOnNavMesh && hasSignal == false)
        {

            transform.position = new Vector3(parentPos.x, Mathf.Lerp(transform.position.y, parentPos.y + 2 * gridArray[arrayPosX, arrayPosZ].structureAmount, 0.07f), parentPos.z);

        }    
    }

    private void Start()
    {
        cellSize =  GridArray.Instance.cellSize;
    }
    private void Awake()
    {
        //gameObject.SetActive(false);
        parentNav = transform.parent.GetComponent<NavMeshAgent>();
    }

    public void GoToSignal(Vector3 position, float newDestMin)
    { 
        Debug.Log("child received signal");
        destMin = newDestMin;

        transform.parent.gameObject.GetComponent<AgentStructure>().hasSignal = true;
        hasSignal = true;
        signalPos = position;


    }
}
