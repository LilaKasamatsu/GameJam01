using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject selecter;

    private int cellSize;
    private Camera cam;

    private Vector3 startPoint;
    private Vector3 endPoint;


    private GameObject[] structureSelects;

    public bool build = false;
    private float bridgeSize;

    int hitGridX;
    int hitGridZ;
    float hitGridY;


    //Singleton
    public static BridgeSpawn Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
                    hitGridX = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.x);
                    hitGridZ = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.z);
                    hitGridY = -1f + 2 * GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount;

                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {

                        startPoint = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);
                        build = true;

                        List<StructureBehavior> selectedStructures = new List<StructureBehavior>();

                        for (int i = 0; i < GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects.Count; i++)
                        {
                            selectedStructures.Add(GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[i].GetComponent<StructureBehavior>());
                            selectedStructures[i].isSelected = true;

                        }


                    }

                }
            }

            if (Input.GetMouseButtonUp(0) && build == true)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
                {
                  
                    hitGridX = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.x);
                    hitGridZ = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.z);
                    hitGridY = -1.5f + 2 * GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount;

             

                    endPoint = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);
                    bridgeSize = Vector3.Distance(startPoint, endPoint ) + 1;
                                    
                }
                else
                {
                    build = false;
                }

                if ( build == true)
                {
                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {

                        GameObject bridgeNew = Instantiate(bridge, startPoint, Quaternion.identity) as GameObject;
                        bridgeNew.transform.localScale = new Vector3(bridgeNew.transform.localScale.x, bridgeNew.transform.localScale.y, bridgeSize);
                        bridgeNew.transform.LookAt(endPoint);

                        build = false;

                    }

                }


            }

        }
    }
           
}
