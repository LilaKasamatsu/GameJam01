using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySignal : MonoBehaviour
{
    private bool canFade;
    private Color alphaColor;
    private MeshRenderer childRender0;
    private MeshRenderer childRender1;

    private float timeToFade = 2f;

    // Start is called before the first frame update
    void Start()
    {
        childRender0 = transform.GetChild(0).GetComponent<MeshRenderer>();
        childRender1 = transform.GetChild(1).GetComponent<MeshRenderer>();

        alphaColor = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
        alphaColor.a = 0;
        StartCoroutine(DestroySelf());
    }

    // Update is called once per frame
    void Update()
    {
        if (canFade)
        {
            childRender0.material.color = Color.Lerp(childRender0.material.color, alphaColor, timeToFade * Time.deltaTime);
            childRender1.material.color = Color.Lerp(childRender1.material.color, alphaColor, timeToFade * Time.deltaTime);

        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(timeToFade );
        canFade = true;


        yield return new WaitForSeconds(timeToFade*2);
        Destroy(this.gameObject);


    }
}
