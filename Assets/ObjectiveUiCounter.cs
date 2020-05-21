using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUiCounter : MonoBehaviour
{
    int collectedCubes = 0;

    [SerializeField] Image cube1;
    [SerializeField] Image cube2;
    [SerializeField] Image cube3;
    [SerializeField] Image cube4;
    [SerializeField] Image cube5;

    // Start is called before the first frame update
    private void Update()
    {
        if(collectedCubes >= 1)
        {
            cube1.GetComponent<Image>().color = new Color(255, 255, 255, 100);
        }
    }




    public void AddCubeCounter()
    {
        collectedCubes += 1;
    }
}
