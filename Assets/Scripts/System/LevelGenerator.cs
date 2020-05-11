﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] int AmountOfGround;
    [SerializeField] [Range(5, 300)] int minBlockScale;
    [SerializeField] [Range(5, 300)] int maxBlockScale;

    [SerializeField] [Range(10,100)] int totalScale;
    [SerializeField] [Range(5, 20)] int maxElevation;
    [SerializeField] GameObject BaseBlockPrefab;
    [SerializeField] public GameObject Groundbounds;
    [SerializeField] GameObject cameraRig;
    [SerializeField] public NavMeshSurface surface;

    public static LevelGenerator instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        
        
    }
    

    public void GenerateMap()
    {
        int cellsize = GridArray.Instance.cellSize;
        totalScale *= cellsize;
        Groundbounds.transform.localScale = new Vector3(totalScale, 40, totalScale);
        Groundbounds.transform.position = new Vector3(totalScale / 2-cellsize, -20, totalScale / 2 - cellsize);
        cameraRig.transform.position = Groundbounds.transform.position + Vector3.up * 23;
        List<int> posList=new List<int>();
        for(int i = (-maxElevation-1)*2; i <= (1+maxElevation)*2;i+=2)
        {
            posList.Add(i);

        }

        for(int i = 1; i < AmountOfGround-1; i++)
        {
            for (int j = 1; j < AmountOfGround-1; j++)
            {
                int ElevationInt = Random.Range(1, posList.Count-1);
                int elevation = posList[ElevationInt];
                posList.RemoveAt(ElevationInt - 1);
                posList.RemoveAt(ElevationInt - 1);
                posList.RemoveAt(ElevationInt - 1);

                int upperEndX = Mathf.RoundToInt(totalScale / AmountOfGround * (i+1)/3+2);
                int lowerEndX = Mathf.RoundToInt(totalScale / AmountOfGround * i/3-4);
                int upperEndZ = Mathf.RoundToInt(totalScale / AmountOfGround * (j+1)/3+2);
                int lowerEndZ = Mathf.RoundToInt(totalScale / AmountOfGround * j/3-4);

                int posX = Random.Range(lowerEndX, upperEndX)*3;
                int posZ = Random.Range(lowerEndZ, upperEndZ)*3;
                int posY = elevation;
                int maxScaleX = Mathf.Min(totalScale-posX, posX - 3)/2;
                int MaxScaleZ = Mathf.Min(totalScale-posZ, posZ - 3)/2;
                    
                int ScaleX = Mathf.Clamp(Random.Range(minBlockScale, maxBlockScale)*3, 0, maxScaleX);
                int ScaleZ = Mathf.Clamp(Random.Range(minBlockScale, maxBlockScale)*3, 0, MaxScaleZ);
                int ScaleY = Mathf.Clamp(Mathf.Abs(posY*12),maxElevation*2,maxElevation*24);

                GameObject BaseBlock= Instantiate(BaseBlockPrefab, new Vector3(posX, posY, posZ), Quaternion.identity,this.transform);
                BaseBlock.transform.localScale = new Vector3(ScaleX, ScaleY, ScaleZ);


            }

        }


    }



}
