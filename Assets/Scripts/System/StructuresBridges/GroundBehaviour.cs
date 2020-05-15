using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Scripting.Pipeline;

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

                Debug.Log("Destruction incoming");

                StructureBehavior structBehavior;
                if (other.TryGetComponent<StructureBehavior>(out structBehavior))
                {
                    GridList targetGridList = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(structBehavior.transform.position.x), GridArray.Instance.NumToGrid(structBehavior.transform.position.z)];
                    List<GameObject> targetList = targetGridList.bridgeObjects;
                    for (int i = targetList.Count - 1; i >= 0; i--)
                    {
                        BridgeStruct target = targetList[i].GetComponent<BridgeStruct>();
                        GridArray.Instance.gridArray[Mathf.RoundToInt(target.gridOrigin.x), Mathf.RoundToInt(target.gridOrigin.z)].bridgeObjects.Remove(target.gameObject);
                        GridArray.Instance.gridArray[Mathf.RoundToInt(target.gridEnd.x), Mathf.RoundToInt(target.gridEnd.z)].bridgeObjects.Remove(target.gameObject);
                        Destroy(targetList[i]);

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
