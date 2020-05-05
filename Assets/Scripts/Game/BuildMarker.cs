using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMarker : MonoBehaviour
{

    Camera cam;


    private void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(SpawnSettings.Instance.spawnMode == true)
        {

          


            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layer_mask = LayerMask.GetMask("Ground");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                if (gameObject.GetComponent<MeshRenderer>().enabled == false)
                {
                    gameObject.GetComponent<MeshRenderer>().enabled = true;
                }

                Vector3 hitGrid = new Vector3(GridArray.Instance.RoundToGrid(hit.point.x), GridArray.Instance.RoundToGrid(hit.point.y), GridArray.Instance.RoundToGrid(hit.point.z));

                int arrayPosX = GridArray.Instance.NumToGrid(hitGrid.x);
                int arrayPosZ = GridArray.Instance.NumToGrid(hitGrid.z);

                Vector3 markerPosition = new Vector3(hitGrid.x, hit.point.y, hitGrid.z);

                transform.position = markerPosition;

            }
        }
    }
    private void LateUpdate()
    {
        if(SpawnSettings.Instance.spawnMode == false)
        {
            Destroy(this.gameObject);
        }
    }
}
