using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InventoryUi : MonoBehaviour
{
    //Texts of Agent counts
    [Header("Foundation")]
    [SerializeField] Button buttonFoundation;
	[SerializeField] Text textFoundation;
	[SerializeField] Text textFoundationMax;

    [Header("Structure")]
    [SerializeField] Button buttonStructure;
    [SerializeField] Text textStructure;
	[SerializeField] Text textStructureMax;

    [Header("Branch")]
    [SerializeField] Button buttonBranch;
    [SerializeField] Text textBranch;
	[SerializeField] Text textBranchMax;

    [Header ("Options Menu")]
	[SerializeField] GameObject optionsPanel;
	[SerializeField] Button buttonOptions;

    [Space (10)]
	[SerializeField] GameObject infoPanel;

    [Header ("Agent Prefabs")]
	public GameObject agentStructure;
	public GameObject agentBranch;
	public GameObject agentFoundation;

    private string selectedButton;

	private SpawnSettings spawnerScript;

	void Start()
	{
		spawnerScript = SpawnSettings.Instance.GetComponent<SpawnSettings>();
        buttonStructure.interactable = false;
        buttonBranch.interactable = false;
	}

 
    private void Update()
	{
		SetButtonStatus();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ClickPause();
        }

        if (spawnerScript.spawnMode == false)
        {
            selectedButton = "";
        }
        SetAgentsAmount();
    }

    private void SetAgentsAmount()
    {
        textFoundation.text = GridArray.Instance.agentStack.agentAmountFoundation.ToString();
        textFoundationMax.text = "|" + (GridArray.Instance.agentStack.agentAmountFoundation + GridArray.Instance.agentStack.agentFoundation).ToString();

        textStructure.text = GridArray.Instance.agentStack.agentAmountStructure.ToString();
        textStructureMax.text = "|" + (GridArray.Instance.agentStack.agentAmountStructure + GridArray.Instance.agentStack.agentStructure).ToString();

        textBranch.text = GridArray.Instance.agentStack.agentAmountBranch.ToString();
        textBranchMax.text = "|" + (GridArray.Instance.agentStack.agentAmountBranch + GridArray.Instance.agentStack.agentBranch).ToString();

        //textAgentAmount.text = GridArray.Instance.agentStack.agentAmount.ToString();
    }

    void SetButtonStatus()
	{
        if (spawnerScript.firstFoundation)
        {
            buttonStructure.interactable = true;
            if (spawnerScript.firstStructure)
            {
                CheckAgentAvailability();
            }
        }

		if (selectedButton == "foundation")
		{
			if (GridArray.Instance.agentStack.agentAmountFoundation <= 0)
			{
				spawnerScript.spawnMode = false;
                buttonFoundation.interactable = false;
            }
            else
            {
                buttonFoundation.interactable = true;
                buttonFoundation.GetComponent<Image>().color = buttonFoundation.colors.highlightedColor;
                buttonStructure.GetComponent<Image>().color = buttonFoundation.colors.normalColor;
                buttonBranch.GetComponent<Image>().color = buttonFoundation.colors.normalColor;
            }
		}
		if (selectedButton == "structure")
		{
			if (GridArray.Instance.agentStack.agentAmountStructure <= 0)
			{
                spawnerScript.spawnMode = false;
                buttonStructure.interactable = false;
            }
            else
            {
                buttonStructure.interactable = true;
                buttonFoundation.GetComponent<Image>().color = buttonFoundation.colors.normalColor;
                buttonStructure.GetComponent<Image>().color = buttonFoundation.colors.highlightedColor;
                buttonBranch.GetComponent<Image>().color = buttonFoundation.colors.normalColor;
            }
		}
		if (selectedButton == "branch")
		{
			if (GridArray.Instance.agentStack.agentAmountBranch <= 0)
			{
                spawnerScript.spawnMode = false;
                buttonBranch.interactable = false;
            }
            else
            {
                buttonBranch.interactable = true;
                buttonFoundation.GetComponent<Image>().color = buttonFoundation.colors.normalColor;
                buttonStructure.GetComponent<Image>().color = buttonFoundation.colors.normalColor;
                buttonBranch.GetComponent<Image>().color = buttonFoundation.colors.highlightedColor;
            }
		}
	}

    private void CheckAgentAvailability()
    {
        //If Amount is empty
        if (GridArray.Instance.agentStack.agentAmountFoundation <= 0)
        {
            buttonFoundation.interactable = false;
        }
        else
        {
            buttonFoundation.interactable = true;
        }

        if (GridArray.Instance.agentStack.agentAmountStructure <= 0)
        {
            buttonStructure.interactable = false;
        }
        else
        {
            buttonStructure.interactable = true;
        }
        if (GridArray.Instance.agentStack.agentAmountBranch <= 0)
        {
            buttonBranch.interactable = false;
        }
        else
        {
            buttonBranch.interactable = true;
        }
    }

	IEnumerator ChangeAgent()
	{
		//yield return new WaitForSeconds(0.5f);
		spawnerScript.spawnMode = false;
		yield return new WaitForSeconds(0.05f);

	}

    public void SelectFoundation()
    {
        if (GridArray.Instance.agentStack.agentAmountFoundation > 0)
        {

            if(spawnerScript.spawnMode == false)
            {
                spawnerScript.spawnMode = true;
                buttonFoundation.transform.parent.GetComponent<Animation>().Play("ButtonClickAnim");
                selectedButton = "foundation";
                spawnerScript.SpawnAgent(agentFoundation);
            }
            else
            {
                StartCoroutine(ChangeAgent());
                SelectFoundation();
            }
        }
    }

    public void SelectStructure()
    {
        if (GridArray.Instance.agentStack.agentAmountStructure > 0)
        {
            if(spawnerScript.spawnMode == false)
            {
                spawnerScript.spawnMode = true;
                buttonStructure.transform.parent.GetComponent<Animation>().Play("ButtonClickAnim");
                selectedButton = "structure";
                spawnerScript.SpawnAgent(agentStructure);
            }
            else
            {
                StartCoroutine(ChangeAgent());
                SelectStructure();
            }
        }
    }

    public void SelectBranch()
    {
        if (GridArray.Instance.agentStack.agentAmountBranch > 0)
        {
            if (spawnerScript.spawnMode == false)
            {
                spawnerScript.spawnMode = true;
                buttonBranch.transform.parent.GetComponent<Animation>().Play("ButtonClickAnim");
                selectedButton = "branch";
                spawnerScript.SpawnAgent(agentBranch);
            }
            else
            {
                StartCoroutine(ChangeAgent());
                SelectBranch();
            }
        }
    }


    public void ClickOptionsMenu()
    {
        buttonOptions.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(true);
        infoPanel.GetComponent<Animation>().Play("ShowControls");
    }
    public void ResumeGame()
    {
        buttonOptions.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(false);
        infoPanel.GetComponent<Animation>().Play("HideControls");

    }
    public void RestartGame()
    {
        SceneManager.LoadScene("LevelGenerationTest");
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}