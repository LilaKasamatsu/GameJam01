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

    [Header("Panels")]
    [SerializeField] GameObject panel01;
    [SerializeField] GameObject panel02;
    [SerializeField] List<GameObject> panels;

    private SpawnSettings spawnSettings;

    private bool isCoroutineRunning;
    private bool isTextToShow;
    private bool allAreActive;

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
            if (!isCoroutineRunning)
            {
                StartCoroutine(WaitForText(2f));
            }
            if (isTextToShow)
            {
                panel01.GetComponent<Fade>().FadeMeIn();
                isTextToShow = false;
            }
            
        }else if (!spawnSettings.firstStructure)
        {
            panel01.GetComponent<Fade>().FadeMeOut();
            if (!isCoroutineRunning)
            {
                StartCoroutine(WaitForText(2f));
            }
            if (isTextToShow)
            {
                panel02.GetComponent<Fade>().FadeMeIn();
                isTextToShow = false;
            }

            buttonStructure.gameObject.SetActive(true);
            buttonStructure.transform.parent.GetComponent<Animation>().Play("ButtonWaitingForClick");
        }
        else
        {
            panel02.GetComponent<Fade>().FadeMeOut();
            buttonBranch.gameObject.SetActive(true);
            allAreActive = true;
        }

        if (allAreActive && !isTutorialRunning)
        {
            StartCoroutine(ShowTutorialTexts());
        }
    }

    [SerializeField] List<float> tutorialWaitTimer;
    bool isTutorialRunning;
    IEnumerator ShowTutorialTexts()
    {
        int counter = 0;
        isTutorialRunning = true;
        foreach (GameObject panel in panels)
        {
            panel.GetComponent<Fade>().FadeMeIn();
            yield return new WaitForSecondsRealtime(tutorialWaitTimer[counter]);

            panel.GetComponent<Fade>().FadeMeOut();
            counter++;
            yield return new WaitForSecondsRealtime(4f);
        }
    }

    IEnumerator WaitForText(float time)
    {
        isCoroutineRunning = true;
        yield return new WaitForSecondsRealtime(time);
        isCoroutineRunning = false;
        isTextToShow = true;
    }

}
