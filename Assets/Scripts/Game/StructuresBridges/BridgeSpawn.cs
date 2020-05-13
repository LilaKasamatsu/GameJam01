using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject selecter;

    private int cellSize;
    private int cellY;
    private Camera cam;

    private Vector3 startPoint;
    private Vector3 endPoint;


    private GameObject[] structureSelects;

    public bool build = false;
    private float bridgeSize;

    int hitGridX;
    int hitGridZ;
    int hitGridY;
       
    int oldX;
    int oldZ;


    private SignalBehavior sig;

    List<StructureBehavior> selectedStructures = new List<StructureBehavior>();
    List<StructureBehavior> selectedStructuresDest = new List<StructureBehavior>();
    StructureBehavior selectedDest = new StructureBehavior();
    StructureBehavior selectedOrigin = new StructureBehavior();


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
        sig = SignalBehavior.Instance;
        cellSize = GridArray.Instance.cellSize;
        cellY = GridArray.Instance.cellY;
        cam = Camera.main;
    }


    public void SearchConnections(int gridX, int gridZ, Vector3 destinationPoint)
    {

        for (int i = 0; i < GridArray.Instance.gridArray[gridX, gridZ].bridgeObjects.Count; i++)
        {
            Vector3 spawnPosition = new Vector3(0, 0, 0);

            int sigY;
            int structureAmount;
            //Vector3 currentGridPos = new Vector3(gridX, 0, gridZ);
            Vector3 gridOrigin = GridArray.Instance.gridArray[gridX, gridZ].bridgeObjects[i].GetComponent<BridgeStruct>().gridOrigin;
            Vector3 gridEnd = GridArray.Instance.gridArray[gridX, gridZ].bridgeObjects[i].GetComponent<BridgeStruct>().gridEnd;



            if (gridOrigin.x == gridX &&
                gridOrigin.z == gridZ)
            {
                spawnPosition = gridEnd;
                spawnPosition.x = gridEnd.x * cellSize;
                spawnPosition.z = gridEnd.z * cellSize;

                sigY = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects.Count - 1;
                spawnPosition.y = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects[sigY].transform.position.y - (sigY + 1)*GridArray.Instance.cellY;
                Debug.Log("Position Grid End");

            }
            else
            {
                spawnPosition = gridOrigin;
                spawnPosition.x = gridOrigin.x * cellSize;
                spawnPosition.z = gridOrigin.z * cellSize;
                Debug.Log("Position Grid Origin");

                sigY = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects.Count - 1;

                spawnPosition.y = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects[sigY].transform.position.y - (sigY + 1) * GridArray.Instance.cellY;


            }

            GameObject signalInstance = Instantiate(sig.signalObj, spawnPosition, Quaternion.identity) as GameObject;
            signalInstance.transform.GetChild(1).transform.localScale = new Vector3(sig.signalRadius * 2, sig.signalRadius * 2, sig.signalRadius * 2);
            
            int layer_agent = LayerMask.GetMask("Agent");
            sig.FindAgents(spawnPosition, destinationPoint, sig.signalRadius, layer_agent);

        }


    
    }

    void Update()
    {
        //Signal Spawning stuff
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Ray raySig = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitSig;
            
            if (Physics.Raycast(raySig, out hitSig) && hitSig.collider.CompareTag("structure"))
            {
                Vector3 hitGrid = new Vector3(Mathf.Round(hitSig.point.x / cellSize) * cellSize, Mathf.Round(hitSig.point.y / cellY) * cellY, Mathf.Round(hitSig.point.z / cellSize) * cellSize);

                int arrayPosX = GridArray.Instance.NumToGrid(hitGrid.x);
                int arrayPosZ = GridArray.Instance.NumToGrid(hitGrid.z);
                int arrayPosY = Mathf.RoundToInt(GridArray.Instance.gridArray[arrayPosX, arrayPosZ].sizeY) * cellY;


                if (GridArray.Instance.gridArray[arrayPosX, arrayPosZ].bridge > 0)
                {
                    Vector3 spawnPosition = new Vector3(arrayPosX * cellSize, arrayPosY, arrayPosZ * cellSize);

                    GameObject signalInstance = Instantiate(sig.signalObj, spawnPosition, Quaternion.identity) as GameObject;

                    SearchConnections(arrayPosX, arrayPosZ, spawnPosition);

                    //signalInstance.transform.GetChild(1).transform.localScale = new Vector3(sig.signalRadius * 2, sig.signalRadius * 2, sig.signalRadius * 2);


                    //int layer_agent = LayerMask.GetMask("Agent");
                    //sig.FindAgents(spawnPosition, sig.signalRadius, layer_agent);
                }
            }            
        }


        //Bridge Spawning
        if (SpawnSettings.Instance.spawnMode == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
                {
                    hitGridX = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.x);
                    hitGridZ = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.z);
                    hitGridY = Mathf.RoundToInt(GridArray.Instance.gridArray[hitGridX, hitGridZ].sizeY) * cellY * 2;



                    oldX = hitGridX;
                    oldZ = hitGridZ;


                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {
                        GridArray.Instance.gridArray[hitGridX, hitGridZ].bridge = 1;

                        startPoint = new Vector3(hitGridX * cellSize, GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].transform.position.y + hitGridY, hitGridZ * cellSize);
                        build = true;

                        //selectedStructures = new List<StructureBehavior>();

                        selectedOrigin = GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].GetComponent<StructureBehavior>();

                        if (selectedOrigin.GetComponent<StructureBehavior>() != null)
                        {
                            selectedOrigin.isSelected = true;

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

                    hitGridY = Mathf.RoundToInt(GridArray.Instance.gridArray[hitGridX, hitGridZ].sizeY) * cellY * 2;


                    if (selectedDest != null && selectedDest.GetComponent<StructureBehavior>() != null)
                    {
                        selectedDest.isSelected = false;
                        selectedOrigin.isSelected = true;

                    }

                    //selectedStructuresDest = new List<StructureBehavior>();

                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {

                        endPoint = new Vector3(hitGridX * cellSize, GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].transform.position.y + hitGridY, hitGridZ * cellSize);


                        //selectedStructuresDest.Add(GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].GetComponent<StructureBehavior>());
                        selectedDest = GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].GetComponent<StructureBehavior>();
                        

                        if (selectedDest.GetComponent<StructureBehavior>() != null)
                        {
                            selectedDest.isSelected = true;

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


                    bridgeSize = Vector3.Distance(startPoint, endPoint);
                    GridArray.Instance.gridArray[hitGridX, hitGridZ].bridge = 1;


                }
                else
                {
                    build = false;
                    GridArray.Instance.gridArray[oldX, oldZ].bridge = 0;
                }

                if (build == true)
                {
                    if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
                    {


                        //Vector3 spawnStart = new Vector3(startPoint.x, startPoint.y, startPoint.z);
                        //Vector3 spawnEnd = new Vector3(endPoint.x, endPoint.y, endPoint.z);


                        GameObject bridgeNew = Instantiate(bridge, startPoint, Quaternion.identity) as GameObject;
                        bridgeNew.transform.localScale = new Vector3(bridgeNew.transform.localScale.x, bridgeNew.transform.localScale.y, bridgeSize);
                        bridgeNew.transform.LookAt(endPoint);

                        bridgeNew.GetComponent<BridgeStruct>().gridOrigin = new Vector3(startPoint.x / cellSize, startPoint.y / GridArray.Instance.cellY, startPoint.z / cellSize);
                        bridgeNew.GetComponent<BridgeStruct>().gridEnd = new Vector3(endPoint.x / cellSize, endPoint.y / GridArray.Instance.cellY, endPoint.z / cellSize);

                        GridArray.Instance.gridArray[oldX, oldZ].bridgeObjects.Add(bridgeNew);
                        GridArray.Instance.gridArray[hitGridX, hitGridZ].bridgeObjects.Add(bridgeNew);



                        build = false;


                        //Selected Destination sets to "isBridged"
                        if (selectedDest != null && selectedDest.GetComponent<StructureBehavior>() != null)
                        {
                            selectedDest.isBridged = true;

                        }

                        //Selected Origin sets to "isBridged"
                        if (selectedOrigin.GetComponent<StructureBehavior>() != null)
                        {
                            selectedOrigin.isBridged = true;

                        }

                    }

                }


            }

        }

    }
}
