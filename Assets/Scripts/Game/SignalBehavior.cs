using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalBehavior : MonoBehaviour
{

    [SerializeField] GameObject signalObj;
    public float signalRadius;
    public float destMin = 3;

    Camera cam;
    private GridArray gridArray;
    private int cellSize;
    private int cellY;

    void Start()
    {
        cam = Camera.main;
        gridArray = GridArray.Instance;

        cellSize = gridArray.cellSize;
        cellY = gridArray.cellY;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layer_mask = LayerMask.GetMask("Ground");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hit.point.x / cellSize) * cellSize, Mathf.Round(hit.point.y / cellY) * cellY, Mathf.Round(hit.point.z / cellSize) * cellSize);

                int arrayPosX = GridArray.Instance.NumToGrid(hitGrid.x);
                int arrayPosZ = GridArray.Instance.NumToGrid(hitGrid.z);
                float arrayPosY = hit.point.y;

                Vector3 spawnPosition = new Vector3(arrayPosX * cellSize, arrayPosY, arrayPosZ * cellSize);

                Instantiate(signalObj, spawnPosition, Quaternion.identity);


                int layer_agent = LayerMask.GetMask("Agent");
                FindAgents(spawnPosition + new Vector3(0, 1, 0), signalRadius, layer_agent);
            }

        
        }

        void FindAgents(Vector3 center, float radius, int layer)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius, layer);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].GetComponent<AgentStructure>() != false)
                {
                    hitColliders[i].GetComponent<AgentStructure>().ReceiveSignal(center, destMin);               
                }
                i++;
            }
        }
    }
}
