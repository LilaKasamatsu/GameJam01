using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructionManager : MonoBehaviour
{
    [SerializeField] float minCoolDown;
    [SerializeField] float maxCoolDown;
    [SerializeField] float duration;
    [SerializeField] int heightLimit;
    [SerializeField] int remainedHeight;
    [SerializeField] int platformSearchDistance;
    [SerializeField] DestructionMode destructionMode;
    [SerializeField] int maxLocalHeightLimit;
    enum DestructionMode
    {
        global,
        local
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (destructionMode)
            {
                case DestructionMode.global:
                   StartCoroutine(GlobalDestruction());
                    break;
                case DestructionMode.local:
                    StartCoroutine(LocalDestruction());
                    break;


            }
           
        }
    }
    IEnumerator LocalDestruction()
    {
        while (true)
        {
            Debug.Log("SANDSTORM on Level:"+heightLimit);
            int coloumscounter = 0;
            int GridLengthX = GridArray.Instance.arrayX;
            int GridLengthZ = GridArray.Instance.arrayZ;
            
            while (coloumscounter < GridLengthX)
            {
                for (int i = 0; i < GridLengthZ; i++)
                {
                    GridList target = GridArray.Instance.gridArray[coloumscounter, i];

                    if (target.structureAmount - target.bridgedStructures >= heightLimit)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        Explode(target, targetVector);

                    }
                    else if (heightLimit==0 && target.structureAmount == 0 && target.foundationAmount > 0)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        LookForStructures(target, targetVector);
                    }

                }


                coloumscounter += 1;
                yield return new WaitForEndOfFrame();
            }
            coloumscounter = 0;
            heightLimit =Mathf.FloorToInt( Random.Range(0, maxLocalHeightLimit)/3 + Random.Range(0, maxLocalHeightLimit) / 3 + Random.Range(0, maxLocalHeightLimit) /3);
            remainedHeight = heightLimit;

            yield return new WaitForSeconds(Random.Range(minCoolDown, maxCoolDown));
        }
    }

    IEnumerator GlobalDestruction()
    {
        while (true)
        {
            Debug.Log("SANDSTORM");
            int coloumscounter = 0;
            int GridLengthX = GridArray.Instance.arrayX;
            int GridLengthZ = GridArray.Instance.arrayZ;
            while (coloumscounter<GridLengthX)
            {
                for (int i = 0; i < GridLengthZ; i++)
                {
                    GridList target = GridArray.Instance.gridArray[coloumscounter, i];

                   if( target.structureAmount-target.bridgedStructures >= heightLimit)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        Explode(target,targetVector);

                    }
                   else if (target.structureAmount==0 && target.foundationAmount > 0)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        LookForStructures(target,targetVector);
                    }

                }


                coloumscounter +=1;
                yield return new WaitForEndOfFrame();
            }
            coloumscounter = 0;

            yield return new WaitForSeconds(Random.Range(minCoolDown, maxCoolDown));
        }
    }

    void Explode(GridList target, Vector3 targetVector)
    {
        List<GameObject> targetList = target.structureObjects;
        for(int i=target.bridgeObjects.Count-1;i>=0;i--)
        {
            GameObject targetBridgeObject = target.bridgeObjects[i];
            BridgeStruct targetscript = targetBridgeObject.GetComponent<BridgeStruct>();
            Vector3 targetBridgeOrigin =targetscript.gridOrigin;
            Vector3 targetBridgeEnd = targetscript.gridEnd;
            GridList targetGridOrigin = GridArray.Instance.gridArray[Mathf.RoundToInt(targetBridgeOrigin.x), Mathf.RoundToInt(targetBridgeOrigin.z)];
            GridList targetGridEnd = GridArray.Instance.gridArray[Mathf.RoundToInt(targetBridgeEnd.x), Mathf.RoundToInt(targetBridgeEnd.z)];
            targetGridOrigin.bridgeObjects.Remove(targetBridgeObject);
            targetGridEnd.bridgeObjects.Remove(targetBridgeObject);
            if (targetGridOrigin.bridgeObjects.Count == 0)
            {
                targetGridOrigin.structureObjects[targetGridOrigin.structureObjects.Count - 1].GetComponent<StructureBehavior>().isBridged = false;
                targetGridOrigin.bridge = 0;
            }
            if (targetGridEnd.bridgeObjects.Count == 0)
            {
                targetGridEnd.structureObjects[targetGridEnd.structureObjects.Count - 1].GetComponent<StructureBehavior>().isBridged = false;
                targetGridEnd.bridge = 0;
            }


            Destroy(targetBridgeObject);
            
            
        }
        for(int i = targetList.Count - 1; i > remainedHeight + target.bridgedStructures- 1; i--)
        {
           
            Destroy(targetList[i]);

            targetList.RemoveAt(i);
            target.structureAmount -= 1;

        }
    }

    void LookForStructures(GridList target, Vector3 targetVector)
    {
        if (GridArray.Instance.searchForStructure(targetVector, platformSearchDistance).Count == 0)
        {
            Destroy(target.foundationObject);
            target.foundationAmount = 0;
        }
    }

    
}
