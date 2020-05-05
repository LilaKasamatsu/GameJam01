using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    [SerializeField] float minCoolDown;
    [SerializeField] float maxCoolDown;
    [SerializeField] float duration;
    [SerializeField] int heightLimit;
    [SerializeField] int remainedHeight;
    [SerializeField] int platformSearchDistance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("Destruction");
           
        }
    }

    IEnumerator Destruction()
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
                    Vector3 targetVector = new Vector3(i, 0, coloumscounter);
                   if( target.structureAmount >= heightLimit)
                    {
                        Explode(target,targetVector);
                    }
                   else if (target.structureAmount==0 && target.foundationAmount > 0)
                    {
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
        for(int i = targetList.Count - 1; i > remainedHeight - 1; i--)
        {
           
            Destroy(targetList[i]);
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
