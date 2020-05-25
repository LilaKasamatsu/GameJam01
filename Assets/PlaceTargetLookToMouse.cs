using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTargetLookToMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Input.mousePosition, Vector3.forward);
        transform.Rotate(new Vector3(90, -90, 0));

        float distanceSize = Vector3.Distance(transform.position, Input.mousePosition);
        GetComponent<RectTransform>().sizeDelta = new Vector2(distanceSize, 10f);
        Debug.Log(distanceSize);

        //transform.position = Input.mousePosition;

        /*
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        transform.position = myCanvas.transform.TransformPoint(pos);
        */
    }
}
