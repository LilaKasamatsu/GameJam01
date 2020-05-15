using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GroundBehaviour : MonoBehaviour
{
    public bool falling;
    
    public IEnumerator  Fall()
    {
        falling = true;
        Debug.Log("Ground is falling");
        yield return new WaitForEndOfFrame();

        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.useGravity = true;

    }

    private void OnTriggerStay(Collider other)
    {
        
        if (falling)
        {
            if (other.CompareTag("structure") || other.CompareTag("agent1"))
            {


                StructureBehavior structBehavior = other.transform.parent.GetComponent<StructureBehavior>();
                if (structBehavior!=null)
                {
                    GridList targetGridList = structBehavior.GridPosition;
                    foreach(GameObject branch in targetGridList.branchObjects)
                    {
                        if (branch != null)
                        {
                            Destroy(branch);
                        }
                    }
                    List<GameObject> targetList = targetGridList.bridgeObjects;
                    for (int i = targetList.Count - 1; i >= 0; i--)
                    {
                        Destroy(targetList[i]);
                        BridgeStruct target = targetList[i].GetComponent<BridgeStruct>();
                        GridArray.Instance.gridArray[Mathf.RoundToInt(target.gridOrigin.x), Mathf.RoundToInt(target.gridOrigin.z)].bridgeObjects.Remove(target.gameObject);
                        GridArray.Instance.gridArray[Mathf.RoundToInt(target.gridEnd.x), Mathf.RoundToInt(target.gridEnd.z)].bridgeObjects.Remove(target.gameObject);
                        

                    }


                }
                if (other.transform.parent != null)
                {
                    Destroy(other.transform.parent.gameObject);
                }
                else
                {
                    Destroy(other.gameObject);
                }
            }

        }


    }
}
