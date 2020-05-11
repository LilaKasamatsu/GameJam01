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
    public List<GameObject> bridgeObjects;

    public GameObject foundationObject;
    public int bridge;

    public GridStructures[] gridStructures;
    public GridStructures[] gridBridges;

    public float towerWidth;
    public int bridgedStructures;
    public string structureShape;

    public int color;

    public GridList(int newStructureAmount, int newPointAmount, int newFoundationAmount, int newBridge, int newBridgedStructures, float newTowerWidth, string newShape, int newColor)
    {
        //x = newX;
        //z = newZ;
        //y = newY;
        structureAmount = newStructureAmount;
        pointAmount = newPointAmount;
        foundationAmount = newFoundationAmount;
        structureObjects = new List<GameObject>();
        bridgeObjects = new List<GameObject>();

        foundationObject = new GameObject();
        bridge = newBridge;

        gridStructures = new GridStructures[GridArray.Instance.maxStructures];
        //gridBridges = new GridStructures[1];

        for (int i = 0; i < gridStructures.Length; i++)
        {
            gridStructures[i] = new GridStructures(0, 0, 0, null);
        }

        //gridBridges[0] = new GridStructures(0, 0, 0, null);

        towerWidth = newTowerWidth;
        bridgedStructures = newBridgedStructures;
        structureShape = newShape;

        color = newColor;

    }



}


