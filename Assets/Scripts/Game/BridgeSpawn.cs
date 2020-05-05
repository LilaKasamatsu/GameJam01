using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [SerializeField] GameObject bridge;

    private int cellSize;
    private Camera cam;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 spawnLocation;

    private bool build = false;
    private float bridgeSize;

    // Start is called before the first frame update
    void Start()
    {
        cellSize = SpawnSettings.Instance.cellSize;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnSettings.Instance.spawnMode == false)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
                {
                    build = true;

                    int hitGridX = GridArray.Instance.NumToGrid(hit.point.x);
                    int hitGridZ = GridArray.Instance.NumToGrid(hit.point.z);
                    int hitGridY = GridArray.Instance.RoundToGrid(-1f + 2 * GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount);

                    startPoint = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);
                                

                    spawnLocation = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);
                }
            }

            if (Input.GetMouseButtonUp(0) && build == true)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
                {
                    int hitGridX = GridArray.Instance.NumToGrid(hit.point.x);
                    int hitGridZ = GridArray.Instance.NumToGrid(hit.point.z);
                    int hitGridY = GridArray.Instance.RoundToGrid(-1f + 2 * GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount);

                    endPoint = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);

                    bridgeSize = Vector3.Distance(startPoint, endPoint ) + 1;
                                    
                }
                else
                {
                    build = false;
                }

                if ( build == true)
                {
                    GameObject bridgeNew = Instantiate(bridge, spawnLocation, Quaternion.identity) as GameObject;
                    bridgeNew.transform.localScale = new Vector3(bridgeNew.transform.localScale.x, bridgeNew.transform.localScale.y, bridgeSize);

                    //bridgeNew.transform.LookAt(new Vector3(endPoint.x * cellSize, endPoint.y, startPoint.z * cellSize));
                    bridgeNew.transform.LookAt(endPoint);
                    //bridgeNew.transform.localPosition += new Vector3(0, 0, bridgeSize / 2);
                }


            }

        }
    }
}
