using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehavior : MonoBehaviour
{
    [SerializeField] Color colorBase1;
    [SerializeField] Color colorBase2;
    [SerializeField] Color colorBase3;

    private Color colorBaseFinal;

    [SerializeField] Color colorSelect;
    [SerializeField] Color colorBridged;

    Renderer render;

    public bool isSelected = false;
    public bool isBridged = false;
    public bool isBase = true;

    void Start()
    {
        render = transform.GetChild(0).GetComponent<Renderer>();
        int randomColor = GridArray.Instance.gridArray[GridArray.Instance.NumToGrid(transform.position.x), GridArray.Instance.NumToGrid(transform.position.z)].color;
        randomColor = 0;

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
        render.material.SetColor("_baseColor", colorBaseFinal);
    }

    private void Update()
    {
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

        render.material.color = colorBaseFinal;

        //render.material.EnableKeyword("_EMISSION");


        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0.5f);
        render.material.SetColor("_EmissionColor", finalColor);

        render.material.SetColor("_baseColor", colorSelect);               


    }
    public void ChangeColorBridged()
    {
        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0.1f);
        render.material.SetColor("_EmissionColor", finalColor);

        //render.material.DisableKeyword("_EMISSION");
        render.material.SetColor("_baseColor", colorBridged);

        isBase = true;
    }
    public void ChangeColorBase()
    {
        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0f);
        render.material.SetColor("_baseColor", colorBaseFinal);

        render.material.SetColor("_EmissionColor", Color.black);
        //render.material.DisableKeyword("_EMISSION");
        
        isBase = true;

    }
}
