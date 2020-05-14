using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BridgeSpawn : MonoBehaviour
{
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject previewBridge;
    [SerializeField] GameObject selecter;
    [SerializeField] GameObject vfxRay;
    [SerializeField] GameObject vfxRadius;

    GameObject previewObject = null;

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

    bool isPreview = false;


    private SignalBehavior sig;
    private float doubleClickTime = 0.22f;
    private float lastClickTime;


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
            int sizeY;
            //Vector3 currentGridPos = new Vector3(gridX, 0, gridZ);
            Vector3 gridOrigin = GridArray.Instance.gridArray[gridX, gridZ].bridgeObjects[i].GetComponent<BridgeStruct>().gridOrigin;
            Vector3 gridEnd = GridArray.Instance.gridArray[gridX, gridZ].bridgeObjects[i].GetComponent<BridgeStruct>().gridEnd;



            if (gridOrigin.x == gridX &&
                gridOrigin.z == gridZ)
            {
                spawnPosition = gridEnd;
                spawnPosition.x = gridEnd.x * cellSize;
                spawnPosition.z = gridEnd.z * cellSize;

                sigY = Mathf.RoundToInt( GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects[0].transform.position.y);
                //sigY += GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].sizeY;

                //spawnPosition.y = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects[sigY].transform.position.y - (sigY + 1)*GridArray.Instance.cellY;
                spawnPosition.y = sigY;

                Debug.Log("Position Grid End");

            }
            else
            {
                spawnPosition = gridOrigin;
                spawnPosition.x = gridOrigin.x * cellSize;
                spawnPosition.z = gridOrigin.z * cellSize;
                Debug.Log("Position Grid Origin");

                sigY = Mathf.RoundToInt(GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects[0].transform.position.y);
                //sigY += GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].sizeY;

                //spawnPosition.y = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(spawnPosition.x), GridArray.Instance.NumToGrid(spawnPosition.z)].structureObjects[sigY].transform.position.y - (sigY + 1) * GridArray.Instance.cellY;
                spawnPosition.y = sigY;


            }

            GameObject signalInstance = Instantiate(sig.signalObj, spawnPosition, Quaternion.identity) as GameObject;
            signalInstance.transform.GetChild(1).transform.localScale = new Vector3(sig.signalRadius * 2, sig.signalRadius * 2, sig.signalRadius * 2);
            Instantiate(vfxRadius, spawnPosition, Quaternion.identity);


            int layer_agent = LayerMask.GetMask("Agent");
            sig.FindAgents(spawnPosition, destinationPoint, sig.signalRadius, layer_agent);

        }    
    }

    void Update()
    {
    

        //Bridge Spawning
        if (SpawnSettings.Instance.spawnMode == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                lastClickTime = Time.time;

                SpawnBridgeOrigin();

                if (timeSinceLastClick <= doubleClickTime)
                {
                    Debug.Log("double");
                    SpawnSignal();
                }

                            
            }
        
            //Check while mouse down
            if (Input.GetMouseButton(0))
            {
                SpawnBridgeHold();
            }

            if (isPreview)
            {
                float previewBridgeSize = Vector3.Distance(startPoint, endPoint);

                previewObject.transform.localScale = new Vector3(previewObject.transform.localScale.x, previewObject.transform.localScale.y, previewBridgeSize);
                previewObject.transform.LookAt(endPoint);
            }
            else
            {
                Destroy(previewObject);
            }

            if (Input.GetMouseButtonUp(0) && build == true)
            {
                spawnBridgeUp();

            }
        }
    }

    private void SpawnSignal()
    {

        //Signal Spawning stuff
        Ray raySig = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitSig;
        Debug.Log("Spawn Signal1");


        if (Physics.Raycast(raySig, out hitSig) && hitSig.collider.CompareTag("structure"))
        {
            Debug.Log("Spawn Signal2");

            //Vector3 hitGridSig = new Vector3(Mathf.Round(hitSig.collider.gameObject.transform.position.x/ cellSize) * cellSize, Mathf.Round(hitSig.collider.gameObject.transform.position.y / cellY) * cellY, Mathf.Round(hitSig.collider.gameObject.transform.position.z / cellSize) * cellSize);

            int arrayPosX = GridArray.Instance.NumToGrid(hitSig.collider.transform.position.x);
            int arrayPosZ = GridArray.Instance.NumToGrid(hitSig.collider.transform.position.z);
            //int arrayPosY = Mathf.RoundToInt(GridArray.Instance.gridArray[arrayPosX, arrayPosZ].sizeY) * cellY;

            int arrayPosY = Mathf.RoundToInt(GridArray.Instance.gridArray[arrayPosX, arrayPosZ].structureObjects[0].transform.position.y);
            arrayPosY += GridArray.Instance.gridArray[arrayPosX, arrayPosZ].sizeY * cellY;


            //GridArray.Instance.gridArray[arrayPosX, arrayPosZ].bridge > 0
            if (selectedOrigin.isBridged)
            {
                Debug.Log("Spawn Signal FINAL");

                Vector3 spawnPosition = new Vector3(arrayPosX * cellSize, arrayPosY, arrayPosZ * cellSize);

                GameObject signalInstance = Instantiate(sig.signalObj, spawnPosition, Quaternion.identity) as GameObject;
                Instantiate(vfxRadius, spawnPosition, Quaternion.identity);
                //Instantiate(vfxRay, spawnPosition, Quaternion.identity);

                SearchConnections(arrayPosX, arrayPosZ, spawnPosition);

                //signalInstance.transform.GetChild(1).transform.localScale = new Vector3(sig.signalRadius * 2, sig.signalRadius * 2, sig.signalRadius * 2);

                //int layer_agent = LayerMask.GetMask("Agent");
                //sig.FindAgents(spawnPosition, sig.signalRadius, layer_agent);
            }
        }
   

    }

    private void SpawnBridgeOrigin()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
        {
            hitGridX = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.x);
            hitGridZ = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.z);
            hitGridY = Mathf.RoundToInt(GridArray.Instance.gridArray[hitGridX, hitGridZ].sizeY) * cellY;



            oldX = hitGridX;
            oldZ = hitGridZ;
            selectedOrigin = GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].GetComponent<StructureBehavior>();
                        
            GridArray.Instance.gridArray[hitGridX, hitGridZ].bridge = 1;

            startPoint = new Vector3(hitGridX * cellSize, GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].transform.position.y + hitGridY, hitGridZ * cellSize);
            build = true;

            //selectedStructures = new List<StructureBehavior>();


            if (selectedOrigin.GetComponent<StructureBehavior>() != null)
            {
                selectedOrigin.isSelected = true;

            }
        }
    }
    private void SpawnBridgeHold()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure"))
        {
            hitGridX = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.x);
            hitGridZ = GridArray.Instance.NumToGrid(hit.collider.gameObject.transform.position.z);
            hitGridY = Mathf.RoundToInt(GridArray.Instance.gridArray[hitGridX, hitGridZ].sizeY) * cellY;


            if (selectedDest != null && selectedDest.GetComponent<StructureBehavior>() != null)
            {
                selectedDest.isSelected = false;
                selectedOrigin.isSelected = true;

            }

            //selectedStructuresDest = new List<StructureBehavior>();


            if (GridArray.Instance.gridArray[hitGridX, hitGridZ].foundationAmount > 0)
            {

                endPoint = new Vector3(hitGridX * cellSize, GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].transform.position.y + hitGridY, hitGridZ * cellSize);


                selectedDest = GridArray.Instance.gridArray[hitGridX, hitGridZ].structureObjects[0].GetComponent<StructureBehavior>();


                if (!isPreview)
                {
                    isPreview = true;
                    previewObject = Instantiate(previewBridge, startPoint, Quaternion.identity) as GameObject;


                }

                if (selectedDest.GetComponent<StructureBehavior>() != null)
                {
                    selectedDest.isSelected = true;

                }

            }
        }
    }
    private void spawnBridgeUp()
    {
        isPreview = false;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("structure") && startPoint != endPoint)
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
