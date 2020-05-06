using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehavior : MonoBehaviour
{
    [SerializeField] Color colorBase;
    [SerializeField] Color colorSelect;
    [SerializeField] Color colorBridged;

    Renderer render;

    public bool isSelected = false;
    public bool isBridged = false;
    public bool isBase = true;

    void Start()
    {
        render = transform.GetChild(0).GetComponent<Renderer>();

        

    }

    private void Update()
    {
        if (isSelected == true && isBase == true)
        {
            ChangeColorSelect();
        }
        else if (isSelected == false && isBridged == false && isBase == false)
        {
            ChangeColorBase();
        }

        if (BridgeSpawn.Instance.build == false)
        {
            isSelected = false;
        }

        if (isBridged && isBase == false)
        {
            render.material.DisableKeyword("_EMISSION");

            render.material.color = colorBridged;
            isBase = true;
        }
    }

    public void ChangeColorSelect()
    {
        render.material.color = colorBase;

        render.material.EnableKeyword("_EMISSION");

        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0.25f);
        render.material.SetColor("_EmissionColor", finalColor);
        isBase = false;

    }
    public void ChangeColorBase()
    {
        render.material.color = colorBase;

        render.material.DisableKeyword("_EMISSION");
        isBase = true;

    }
}
