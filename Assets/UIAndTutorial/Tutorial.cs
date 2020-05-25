using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button buttonFoundation;
    [SerializeField] Button buttonStructure;
    [SerializeField] Button buttonBranch;

    private SpawnSettings spawnSettings;


    // Start is called before the first frame update
    void Start()
    {
        spawnSettings = SpawnSettings.Instance;

        buttonStructure.gameObject.SetActive(false);
        buttonBranch.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnSettings.firstFoundation)
        {
            buttonFoundation.transform.parent.GetComponent<Animation>().Play("ButtonWaitingForClick");
            Debug.Log("HI");
        }else if (!spawnSettings.firstStructure)
        {
            buttonStructure.gameObject.SetActive(true);
            buttonStructure.transform.parent.GetComponent<Animation>().Play("ButtonWaitingForClick");
        }
        else
        {
            buttonBranch.gameObject.SetActive(true);
        }
    }


}
