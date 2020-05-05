using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GridList
{

    public int x;
    public int z;

    public int structureAmount;
    public int pointAmount;
    public int foundationAmount;
    public List<GameObject> structureObjects;
    public int bridge;


    public GridList(int newStructureAmount, int newPointAmount, int newFoundationAmount, int newBridge)
    {
        //x = newX;
        //z = newZ;
        structureAmount = newStructureAmount;
        pointAmount = newPointAmount;
        foundationAmount = newFoundationAmount;
        structureObjects = new List<GameObject>();
        bridge = newBridge;

        //structureObjects = newStructureObjects;
    }


 
}