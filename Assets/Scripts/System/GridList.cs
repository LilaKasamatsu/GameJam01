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


    public GridList(int newX, int newZ, int newStructureAmount, int newPointAmount, int newFoundationAmount)
    {
        x = newX;
        z = newZ;
        structureAmount = newStructureAmount;
        pointAmount = newPointAmount;
        foundationAmount = newFoundationAmount;
    }


 
}