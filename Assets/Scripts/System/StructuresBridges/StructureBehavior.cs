using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehavior : MonoBehaviour
{
    [SerializeField] Color colorBase1;
    [SerializeField] Color colorBase2;
    [SerializeField] Color colorBase3;

    [SerializeField] Color colorGradient;

    private Color colorBaseFinal;

    [SerializeField] Color colorSelect;
    [SerializeField] Color colorBridged;

    //Renderer render;
    List<Renderer> renders = new List<Renderer>();

    public bool isSelected = false;
    public bool isBridged = false;
    public bool isBase = true;
    float randomGrowthMax;
    float randomLerp = 0.0001f;
    public float startY;

    List<GameObject> tag_targets;

    private void AddDescendants(Transform parent, List<GameObject> list)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            //!child.gameObject.GetComponent<StructureBehavior>() &&
            if ( (child.gameObject.CompareTag("structure") || child.gameObject.CompareTag("branch")))
            {
                list.Add(child.gameObject);
                if(child.gameObject.GetComponent<Renderer>() != null)
                {
                    renders.Add(child.gameObject.GetComponent<Renderer>());
                }
       
            }
            if (child.transform.childCount > 0)
            {
                AddDescendants(child, list);

            }
        }
    }

    void Start()
    {


        AddDescendants(transform, new List<GameObject>());

        //render = transform.GetChild(0).GetComponent<Renderer>();
        int randomColor = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(transform.position.x), GridArray.Instance.NumToGrid(transform.position.z)].color;
        //randomColor = 0;

        if (randomColor == 0)
        {
            colorBaseFinal = colorBase1;
        }
        else if (randomColor == 1)
        {
            colorBaseFinal = colorBase2;

        }
        else if (randomColor == 2)
        {
            colorBaseFinal = colorBase3;

        }
        //render.material.color = colorBaseFinal;
        //render.material.SetColor("_baseColor", colorBaseFinal);

        for (int i = 0; i < renders.Count; i++)
        {
            renders[i].material.SetColor("_baseColor", colorBaseFinal);
            renders[i].material.SetColor("_gradientColor", colorGradient);


        }

        StartCoroutine(growthDelay());
    }

    IEnumerator growthDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2));
            randomLerp = Random.Range(0.0001f, 0.05f);
            randomGrowthMax = Random.Range(-0.5f, 0.5f);
        }
    }

    private void Update()
    {
        int gridX = GridArray.Instance.NumToGrid(transform.position.x);
        int gridZ = GridArray.Instance.NumToGrid(transform.position.z);

        if (CompareTag("branch"))
        {
            float posY = -1 + startY / transform.parent.transform.localScale.y;
            //transform.position = new Vector3(transform.position.x, posY, transform.position.z);
        }


        if (!CompareTag("branch") && GridArray.Instance.gridArray[gridX, gridZ].sizeY > 0)
        {

            transform.localScale = new Vector3(transform.localScale.x,
            Mathf.Lerp(transform.localScale.y, GridArray.Instance.gridArray[gridX, gridZ].sizeY + randomGrowthMax, randomLerp),
            transform.localScale.z);
        }



        if (isBase == false && isSelected == false)
        {
            if (isBridged)
            {
                ChangeColorBridged();
 
            }
            else if (isBridged == false)
            {
                ChangeColorBase();
            }
        }

        if (BridgeSpawn.Instance.build == false)
        {
            isSelected = false;
        }


        if (isSelected == true)
        {
            ChangeColorSelect();
        }  
        
    }

    public void ChangeColorSelect()
    {
        isBase = false;


        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0.5f);
        //render.material.SetColor("_emissionColor", finalColor);

        //render.material.SetColor("_baseColor", colorSelect);
        for (int i = 0; i < renders.Count; i++)
        {
            renders[i].material.SetColor("_baseColor", colorSelect);
            renders[i].material.SetColor("_gradientColor", colorSelect);

        }



    }
    public void ChangeColorBridged()
    {
        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0.1f);
        //render.material.SetColor("_emissionColor", finalColor);

        //render.material.SetColor("_baseColor", colorBridged);
        for (int i = 0; i < renders.Count; i++)
        {
            renders[i].material.SetColor("_baseColor", colorBridged);
            renders[i].material.SetColor("_gradientColor", colorBridged);

        }

        isBase = true;
    }
    public void ChangeColorBase()
    {
        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0f);
        //render.material.SetColor("_baseColor", colorBaseFinal);

        for (int i = 0; i < renders.Count; i++)
        {
            renders[i].material.SetColor("_baseColor", colorBaseFinal);
            renders[i].material.SetColor("_gradientColor", colorGradient);

        }

        //render.material.SetColor("_emissionColor", Color.black);

        isBase = true;

    }
}
