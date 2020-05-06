using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridStructures
{
    
    public int x;
    public int z;
    public int y;

    public int xOrigin;
    public int zOrigin;

    public bool isBranched = false;

    public GameObject strucObject;


    public GridStructures(int newX, int newY, int newZ, GameObject newStructure)
    {

        x = 0;
        z = 0;
        y = 0;
        //strucObject = new GameObject();


    }


}
