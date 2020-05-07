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

    int oldX;
    int oldZ;

    int modY;
    int modEndY;

    List<StructureBehavior> selectedStructures = new List<StructureBehavior>();
    List<StructureBehavior> selectedStructuresDest = new List<StructureBehavior>();


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
                    hitGridY = GridArray.Instance.cellY * GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount;

                    modY = Mathf.RoundToInt(GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount - 1].transform.position.y);

                    oldX = hitGridX;
                    oldZ = hitGridZ;

                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {
                        GridArray.Instance.gridArray[hitGridX, hitGridZ].bridge = 1;

                        startPoint = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);
                        build = true;

                        selectedStructures = new List<StructureBehavior>();
                        
                        for (int i = 0; i < GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects.Count; i++)
                        {
                            selectedStructures.Add(GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[i].GetComponent<StructureBehavior>());
                            
                            if(selectedStructures[i].GetComponent<StructureBehavior>() != null)
                            {
                                selectedStructures[i].isSelected = true;

                            }

                        }
                    }
                }
            }

            //Check while mouse down
            if (Input.GetMouseButton(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
                {
                    hitGridX = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.x);
                    hitGridZ = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.z);
                    hitGridY = GridArray.Instance.cellY * GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount;

                    modEndY = Mathf.RoundToInt(GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[GridArray.Instance.gridArray[hitGridX, hitGridZ].structureAmount - 1].transform.position.y);



                    for (int i = 0; i < selectedStructuresDest.Count; i++)
                    {
                        if(selectedStructuresDest[i].GetComponent<StructureBehavior>() != null && (selectedStructuresDest.Count > selectedStructures.Count || selectedStructuresDest.Count < selectedStructures.Count))
                        {
                            selectedStructuresDest[i].isSelected = false;

                        }

                        else if (selectedStructuresDest[i].GetComponent<StructureBehavior>() != null && selectedStructuresDest[i] != selectedStructures[i])
                        {
                            selectedStructuresDest[i].isSelected = false;
                            
                        }

                    }
                    
                    selectedStructuresDest = new List<StructureBehavior>();

                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {

                        endPoint = new Vector3(hitGridX * cellSize, hitGridY, hitGridZ * cellSize);

                 
                        
                        for (int i = 0; i < GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects.Count; i++)
                        {
                            selectedStructuresDest.Add(GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[i].GetComponent<StructureBehavior>());

                            if (selectedStructuresDest[i].GetComponent<StructureBehavior>() != null)
                            {
                                selectedStructuresDest[i].isSelected = true;

                            }

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
                  
                 
                    bridgeSize = Vector3.Distance(startPoint + new Vector3(0, modY, 0), endPoint + new Vector3(0, modEndY, 0)) ;
                    GridArray.Instance.gridArray[hitGridX, hitGridZ].bridge = 1;


                }
                else
                {
                    build = false;
                    GridArray.Instance.gridArray[oldX, oldZ].bridge = 0;
                }

                if ( build == true)
                {
                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {

                        //GameObject bridgeNew = Instantiate(bridge, startPoint, Quaternion.identity) as GameObject;
                        //bridgeNew.transform.localScale = new Vector3(bridgeNew.transform.localScale.x, bridgeNew.transform.localScale.y, bridgeSize);
                        //bridgeNew.transform.LookAt(endPoint);

                        Vector3 spawnStart = new Vector3(startPoint.x, 0, startPoint.z);
                        Vector3 spawnEnd = new Vector3(endPoint.x, 0, endPoint.z);


                        GameObject bridgeNew = Instantiate(bridge, spawnStart + new Vector3(0, modY, 0), Quaternion.identity) as GameObject;
                        bridgeNew.transform.localScale = new Vector3(bridgeNew.transform.localScale.x, bridgeNew.transform.localScale.y, bridgeSize);
                        bridgeNew.transform.LookAt(spawnEnd + new Vector3(0, modEndY, 0));


                        build = false;


                        for (int i = 0; i < selectedStructuresDest.Count; i++)
                        {
                            if (selectedStructuresDest[i].GetComponent<StructureBehavior>() != null)
                            {
                                selectedStructuresDest[i].isBridged = true;

                            }

                        }
                        for (int i = 0; i < selectedStructures.Count; i++)
                        {
                            if (selectedStructures[i].GetComponent<StructureBehavior>() != null)
                            {
                                selectedStructures[i].isBridged = true;

                            }

                        }

                    }

                }


            }

        }
    }
           
}
