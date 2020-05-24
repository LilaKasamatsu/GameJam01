﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InventoryUi : MonoBehaviour
{
	//Texts of Agent counts
	[SerializeField] Text textFoundation;
	[SerializeField] Text textFoundationMax;
	[SerializeField] Color colorFoundation;

	[SerializeField] Text textStructure;
	[SerializeField] Text textStructureMax;
	[SerializeField] Color colorStructure;

	[SerializeField] Text textBranch;
	[SerializeField] Text textBranchMax;
	[SerializeField] Color colorBranch;

	[SerializeField] Color colorGrey;
	//

	[SerializeField] GameObject pauseMenu;
	[SerializeField] Button pauseButton;
	[SerializeField] GameObject infoPanel;
	[SerializeField] GameObject destinationPlane;
	[SerializeField] GameObject originPlane;
	[SerializeField] GameObject placeTargetPrefab;
	private GameObject placeTarget;
	int isPaused = 0;
	[SerializeField] Text textAgentAmount;

	private string selectedButton;

	public GameObject agentStructure;
	public GameObject agentBranch;
	public GameObject agentFoundation;



	GameObject spawnController;
	private SpawnSettings spawnerScript;

	void Start()
	{
	

		spawnerScript = SpawnSettings.Instance.GetComponent<SpawnSettings>();

		//Button button1 = agentButton1.GetComponent<Button>();
		//btn.onClick.AddListener(TaskOnClick);
		pauseButton.GetComponent<Button>().onClick.AddListener(() => ClickPause());



	}


	private void Update()
	{
		SetButtonColor();

		if (Input.GetKeyDown(KeyCode.Escape))
        {
			ClickPause();
        }

		if (spawnerScript.spawnMode == false)
		{
			selectedButton = "";
			if (placeTarget != null)
			{
				Destroy(placeTarget);
			}
		}


		if (isPaused == 1)
		{
			infoPanel.transform.position = new Vector3(infoPanel.transform.position.x, Mathf.Lerp(infoPanel.transform.position.y, originPlane.transform.position.y, 0.1f), infoPanel.transform.position.z);
		}
		else if (isPaused == 0)
		{
			infoPanel.transform.position = new Vector3(infoPanel.transform.position.x, Mathf.Lerp(infoPanel.transform.position.y, destinationPlane.transform.position.y, 0.1f), infoPanel.transform.position.z);
		}


		if (Input.GetMouseButtonDown(0))
		{
			CheckButton();
		}

		textFoundation.text = GridArray.Instance.agentStack.agentAmountFoundation.ToString();
		textFoundationMax.text = "|" + (GridArray.Instance.agentStack.agentAmountFoundation + GridArray.Instance.agentStack.agentFoundation).ToString();

		textStructure.text = GridArray.Instance.agentStack.agentAmountStructure.ToString();
		textStructureMax.text = "|" + (GridArray.Instance.agentStack.agentAmountStructure + GridArray.Instance.agentStack.agentStructure).ToString();

		textBranch.text = GridArray.Instance.agentStack.agentAmountBranch.ToString();
		textBranchMax.text = "|" + (GridArray.Instance.agentStack.agentAmountBranch + GridArray.Instance.agentStack.agentBranch).ToString();

		//textAgentAmount.text = GridArray.Instance.agentStack.agentAmount.ToString();


	}
	void SetButtonColor()
	{
		if (selectedButton != "structure")
		{
			if (GridArray.Instance.agentStack.agentAmountFoundation > 0)
			{
				GameObject.Find("ButtonFoundation").GetComponent<Image>().color = Color.white;

			}

		}

		if (spawnerScript.firstFoundation == false)
		{
			if (selectedButton != "structure")
			{
				GameObject.Find("ButtonStructure").GetComponent<Image>().color = colorGrey;

			}

			if (selectedButton != "branch")
			{
				GameObject.Find("ButtonBranch").GetComponent<Image>().color = colorGrey;
		
			}


		}	
		if (spawnerScript.firstFoundation == true)
		{
			if (selectedButton != "structure")
			{
				if (GridArray.Instance.agentStack.agentAmountStructure > 0)
				{
					GameObject.Find("ButtonStructure").GetComponent<Image>().color = Color.white;

				}
			}


		}

		if (spawnerScript.firstStructure == false)
		{
			if (selectedButton != "branch")
			{
				GameObject.Find("ButtonBranch").GetComponent<Image>().color = colorGrey;			
			}


		}
		if (spawnerScript.firstStructure == true)
		{
			if (selectedButton != "branch")
			{
				if (GridArray.Instance.agentStack.agentAmountBranch > 0)
				{
					GameObject.Find("ButtonBranch").GetComponent<Image>().color = Color.white;

				}

			}

		}


		if (selectedButton == "foundation")
		{
			if (GridArray.Instance.agentStack.agentAmountFoundation <= 0)
			{
				SpawnSettings.Instance.spawnMode = false;
			}
			GameObject.Find("ButtonFoundation").GetComponent<Image>().color = colorFoundation;
		}
		if (selectedButton == "structure")
		{
			if (GridArray.Instance.agentStack.agentAmountStructure <= 0)
			{
				SpawnSettings.Instance.spawnMode = false;
			}
			GameObject.Find("ButtonStructure").GetComponent<Image>().color = colorStructure;
		}
		if (selectedButton == "branch")
		{
			if (GridArray.Instance.agentStack.agentAmountBranch <= 0)
			{
				SpawnSettings.Instance.spawnMode = false;
			}
			GameObject.Find("ButtonBranch").GetComponent<Image>().color = colorBranch;
		}


		//If Amount is empty
		if (GridArray.Instance.agentStack.agentAmountFoundation <= 0)
		{
			GameObject.Find("ButtonFoundation").GetComponent<Image>().color = colorGrey;
		}
		if (GridArray.Instance.agentStack.agentAmountStructure <= 0)
		{
			GameObject.Find("ButtonStructure").GetComponent<Image>().color = colorGrey;
		}
		if (GridArray.Instance.agentStack.agentAmountBranch <= 0)
		{
			GameObject.Find("ButtonBranch").GetComponent<Image>().color = colorGrey;
		}

	}
	public void ClickPause()
	{
		if (isPaused == 1)
        {
			pauseMenu.SetActive(false);
			pauseButton.gameObject.SetActive(true);

		}
		else
        {
			pauseMenu.SetActive(true);
			pauseButton.gameObject.SetActive(false);

		}
		isPaused = 1 - isPaused;
		Time.timeScale = 1 - Time.timeScale;

		//Debug.Log("click");
	}

	public void Restart()
    {
		SceneManager.LoadScene("LevelGenerationTest");
    }

	private bool IsMouseOverUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}



	IEnumerator ChangeAgent(int type)
	{
		//yield return new WaitForSeconds(0.5f);
		spawnerScript.spawnMode = false;
		yield return new WaitForSeconds(0.05f);

		CheckButton();

	}


	private GameObject CheckButton()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = Input.mousePosition;

		List<RaycastResult> hitList = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, hitList);

		for (int i = 0; i < hitList.Count; i++)
		{
			if (hitList[i].gameObject.name == "ButtonFoundation" && GridArray.Instance.agentStack.agentAmountFoundation > 0)
			{
				if(spawnerScript.spawnMode == false)
				{
					spawnerScript.spawnMode = true;
					Debug.Log("You have the foundation!");

					hitList[i].gameObject.transform.parent.GetComponent<Animation>().Play();

					if (placeTarget == null)
					{
						placeTarget = Instantiate(placeTargetPrefab, hitList[i].gameObject.transform.position, Quaternion.identity) as GameObject;
						placeTarget.transform.parent = transform.GetChild(0).transform.GetChild(0);
						placeTarget.transform.SetSiblingIndex(0);

					}

					selectedButton = "foundation";
					spawnerScript.SpawnAgent(agentFoundation, hitList[i].gameObject.transform.position);
					return agentFoundation;
				}
				else
				{
					StartCoroutine(ChangeAgent(0));
				}

							   				 

			}
						

			if (hitList[i].gameObject.name == "ButtonStructure" && GridArray.Instance.agentStack.agentAmountStructure > 0)
			{
				if (spawnerScript.spawnMode == false)
				{
					spawnerScript.spawnMode = true;
					Debug.Log("You have clicked the yellow button!");
					hitList[i].gameObject.transform.parent.GetComponent<Animation>().Play();
					if (placeTarget == null)
					{
						placeTarget = Instantiate(placeTargetPrefab, hitList[i].gameObject.transform.position, Quaternion.identity) as GameObject;
						placeTarget.transform.parent = transform.GetChild(0).transform.GetChild(0);
						placeTarget.transform.SetSiblingIndex(0);

					}
					selectedButton = "structure";
					spawnerScript.SpawnAgent(agentStructure, hitList[i].gameObject.transform.position);
					return agentStructure;
				}
				else
				{
					StartCoroutine(ChangeAgent(0));
				}



			}

			if (hitList[i].gameObject.name == "ButtonBranch" && GridArray.Instance.agentStack.agentAmountBranch > 0)
			{
				if (spawnerScript.spawnMode == false)
				{
					hitList[i].gameObject.transform.parent.GetComponent<Animation>().Play();
					if (placeTarget == null)
					{
						placeTarget = Instantiate(placeTargetPrefab, hitList[i].gameObject.transform.position, Quaternion.identity) as GameObject;
						placeTarget.transform.parent = transform.GetChild(0).transform.GetChild(0);
						placeTarget.transform.SetSiblingIndex(0);

					}
					selectedButton = "branch";
					spawnerScript.spawnMode = true;
					spawnerScript.SpawnAgent(agentBranch, hitList[i].gameObject.transform.position);
					return agentBranch;
				}
				else
				{
					StartCoroutine(ChangeAgent(0));
				}


			}

		}
		return null;

	}



	void TaskOnClick()
	{
		//Debug.Log("You have clicked the button!");

		//spawnerScript.SpawnAgent(agent1);
	}
}