using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GridList
{

    public int x;
    public int z;
    public int y;

    public int structureAmount;
    public int pointAmount;
    public int foundationAmount;
    public List<GameObject> structureObjects;
    public GameObject foundationObject;
    public int bridge;

    public GridStructures[] gridStructures;


    public float towerWidth;

    public GridList(int newStructureAmount, int newPointAmount, int newFoundationAmount, int newBridge)
    {
        //x = newX;
        //z = newZ;
        //y = newY;
        structureAmount = newStructureAmount;
        pointAmount = newPointAmount;
        foundationAmount = newFoundationAmount;
        structureObjects = new List<GameObject>();
        foundationObject = new GameObject();
        bridge = newBridge;

        gridStructures = new GridStructures[GridArray.Instance.maxStructures];

        for (int i = 0; i < gridStructures.Length; i++)
        {
            gridStructures[i] = new GridStructures(0, 0, 0, null);
        }
                          

    }



}


