using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUi : MonoBehaviour
{
	[SerializeField] Text textFoundation;
	[SerializeField] Text textFoundationMax;

	[SerializeField] Text textStructure;
	[SerializeField] Text textStructureMax;

	[SerializeField] Text textBranch;
	[SerializeField] Text textBranchMax;

	[SerializeField] Button pauseButton;
	[SerializeField] GameObject infoPanel;
	[SerializeField] GameObject destinationPlane;
	[SerializeField] GameObject originPlane;
	[SerializeField] GameObject placeTargetPrefab;
	private GameObject placeTarget;
	int showInfo = 1;
	[SerializeField] Text textAgentAmount;


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
		

		if (spawnerScript.spawnMode == false && placeTarget != null)
		{

			Destroy(placeTarget);
		}


		if (showInfo == 1)
		{
			infoPanel.transform.position = new Vector3(infoPanel.transform.position.x, Mathf.Lerp(infoPanel.transform.position.y, originPlane.transform.position.y, 0.1f), infoPanel.transform.position.z);
		}
		else if (showInfo == 0)
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

	void ClickPause()
	{
		showInfo = 1 - showInfo;
		Debug.Log("click");
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
			if (hitList[i].gameObject.name == "ButtonFoundation")
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

					spawnerScript.SpawnAgent(agentFoundation, hitList[i].gameObject.transform.position);
					return agentFoundation;
				}
				else
				{
					StartCoroutine(ChangeAgent(0));
				}






			}
						

			if (hitList[i].gameObject.name == "ButtonStructure")
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
					spawnerScript.SpawnAgent(agentStructure, hitList[i].gameObject.transform.position);
					return agentStructure;
				}
				else
				{
					StartCoroutine(ChangeAgent(0));
				}



			}

			if (hitList[i].gameObject.name == "ButtonBranch")
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