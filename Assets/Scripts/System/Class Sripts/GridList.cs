﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;



public class GridList 
{

    public int x;
    public int z;
    public int y;

    
    public int pointAmount;
    public int foundationAmount;
    public List<GameObject> structureObjects;
    public List<GameObject> bridgeObjects;    public List<GameObject> branchObjects = new List<GameObject>();

    public GameObject foundationObject;
    public int bridge;

    public GridStructures[] gridStructures;
    public GridStructures[] gridBridges;

    public float towerWidth;
    public int branchedStructures;
    public string structureShape;

    public int sizeY;

    public int color;
    
    public GameObject windParticle;

    public List<int> branchAtY = new List<int>();

    public GridList( int newPointAmount, int newFoundationAmount, int newBridge, int newBranched, float newTowerWidth, string newShape, int newColor)
    {
        //x = newX;
        //z = newZ;
        //y = newY;
       
        pointAmount = newPointAmount;
        foundationAmount = newFoundationAmount;
        structureObjects = new List<GameObject>();
        bridgeObjects = new List<GameObject>();

        foundationObject = null;
        bridge = newBridge;

        gridStructures = new GridStructures[1];
        //gridBridges = new GridStructures[1];

        for (int i = 0; i < gridStructures.Length; i++)
        {
            gridStructures[i] = new GridStructures(0, 0, 0, null);
        }

        //gridBridges[0] = new GridStructures(0, 0, 0, null);

        towerWidth = newTowerWidth;
        branchedStructures = newBranched;
        structureShape = newShape;
        color = newColor;
    }

    public void CreateWindParticles()
    {
        if (DestructionManager.instance.heightLimit <= sizeY - branchedStructures && windParticle!=null)
        {
            GameObject WindPrefab = DestructionManager.instance.localWindPrefab;
            float cellSize = GridArray.Instance.cellSize;
            Vector3 target = structureObjects[0].transform.position;
            windParticle= DestructionManager.instance.LocalParticleInstantiate(target.x,target.y+(DestructionManager.instance.heightLimit+branchedStructures)*2 ,target.z);
        }

    }


}


