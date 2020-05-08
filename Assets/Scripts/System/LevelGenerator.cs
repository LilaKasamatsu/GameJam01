using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] int AmountOfGround;
    [SerializeField] [Range(5, 300)] int minBlockScale;
    [SerializeField] [Range(5, 300)] int maxBlockScale;

    [SerializeField] [Range(10,100)] int totalScale;
    [SerializeField] [Range(5, 20)] int maxElevation;
    [SerializeField] GameObject BaseBlockPrefab;
    [SerializeField] GameObject Groundbounds;

    private void Start()
    {
        GenerateMap();


    }

    void GenerateMap()
    {
        int cellsize = GridArray.Instance.cellSize;
        totalScale *= cellsize;
        Groundbounds.transform.localScale = new Vector3(totalScale, 40, totalScale);
        Groundbounds.transform.position = new Vector3(totalScale / 2-cellsize, 0, totalScale / 2 - cellsize);

        for(int i = 0; i < AmountOfGround; i++)
        {
            for (int j = 0; j < AmountOfGround; j++)
            {
                int elevation = Random.Range(-maxElevation, maxElevation) * 2;
                int upperEndX = Mathf.RoundToInt(totalScale / AmountOfGround * (i+1)/3);
                int lowerEndX = Mathf.RoundToInt(totalScale / AmountOfGround * i/3);
                int upperEndZ = Mathf.RoundToInt(totalScale / AmountOfGround * (j+1)/3);
                int lowerEndZ = Mathf.RoundToInt(totalScale / AmountOfGround * j/3);

                int posX = Random.Range(lowerEndX, upperEndX)*3;
                int posZ = Random.Range(lowerEndZ, upperEndZ)*3;
                int posY = elevation;
                int maxScaleX = Mathf.Min(totalScale-posX, posX - 3)/2;
                int MaxScaleZ = Mathf.Min(totalScale-posZ, posZ - 3)/2;
                    
                int ScaleX = Mathf.Clamp(Random.Range(minBlockScale, maxBlockScale), 0, maxScaleX)*3;
                int ScaleZ = Mathf.Clamp(Random.Range(minBlockScale, maxBlockScale), 0, MaxScaleZ)*3;
                int ScaleY = posY*2;

                GameObject BaseBlock= Instantiate(BaseBlockPrefab, new Vector3(posX, posY, posZ), Quaternion.identity,this.transform);
                BaseBlock.transform.localScale = new Vector3(ScaleX, ScaleY, ScaleZ);


            }

        }


    }



}
