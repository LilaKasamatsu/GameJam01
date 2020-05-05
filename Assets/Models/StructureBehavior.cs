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

    void Start()
    {
        render = GetComponent<Renderer>();

        

    }

    private void Update()
    {
        if (isSelected == true)
        {
            ChangeColorSelect();
        }
        else if (isSelected == false && isBridged == false)
        {
            ChangeColorBase();
        }

        if (BridgeSpawn.Instance.build == false)
        {
            isSelected = false;
        }

        if (isBridged)
        {
            render.material.color = colorBridged;
            render.material.DisableKeyword("_EMISSION");
        }
    }

    public void ChangeColorSelect()
    {
        render.material.color = colorBase;

        render.material.EnableKeyword("_EMISSION");

        Color finalColor = colorSelect * Mathf.LinearToGammaSpace(0.25f);
        render.material.SetColor("_EmissionColor", finalColor);

    }
    public void ChangeColorBase()
    {
        render.material.color = colorBase;

        render.material.DisableKeyword("_EMISSION");

    }
}
