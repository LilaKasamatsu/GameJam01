using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    [SerializeField] float minCoolDown;
    [SerializeField] float maxCoolDown;
    [SerializeField] float duration;
    [SerializeField] int heightLimit;
    [SerializeField] int platformSearchDistance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("Destruction");
            Debug.Log("startCoroutine");
        }
    }

    IEnumerator Destruction()
    {
        while (true)
        {
            int coloumscounter = 0;
            int GridLengthX = GridArray.Instance.arrayX;
            int GridLengthZ = GridArray.Instance.arrayZ;
            while (coloumscounter<GridLengthX)
            {
                for (int i = 0; i < GridLengthZ; i++)
                {
                    GridList target = GridArray.Instance.gridArray[coloumscounter, i];
                    Vector3 targetVector = new Vector3(coloumscounter, 0, i);
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
            

            yield return new WaitForSeconds(Random.Range(minCoolDown, maxCoolDown));
        }
    }

    void Explode(GridList target, Vector3 targetVector)
    {
        
    }

    void LookForStructures(GridList target, Vector3 targetVector)
    {
        if (GridArray.Instance.searchForStructure(targetVector, platformSearchDistance).Count == 0)
        {
            
        }
    }

    
}
