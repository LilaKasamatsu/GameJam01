using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalBehavior : MonoBehaviour
{

    public GameObject signalObj;
    public float signalRadius;
    public float destMin = 3;

    Camera cam;
    private GridArray gridArray;
    private int cellSize;
    private int cellY;
    
    //Singleton
    public static SignalBehavior Instance { get; private set; }
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

    void Start()
    {
        cam = Camera.main;
        gridArray = GridArray.Instance;

        cellSize = gridArray.cellSize;
        cellY = gridArray.cellY;
        
    }
    void Update()
    {
       
    }


    public void FindAgents(Vector3 center, Vector3 destination, float radius, int layer)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, layer);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].GetComponent<AgentStructure>() != false)
            {
                hitColliders[i].GetComponent<AgentStructure>().ReceiveSignal(destination, destMin);
            }
            i++;
        }
    }



}
