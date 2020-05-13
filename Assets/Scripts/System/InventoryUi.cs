using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUi : MonoBehaviour
{
	[SerializeField] Text textFoundation;
	[SerializeField] Text textStructure;
	[SerializeField] Text textBranch;



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



	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			CheckButton();
		}

		textFoundation.text = GridArray.Instance.agentStack.agentFoundation.ToString();
		textStructure.text = GridArray.Instance.agentStack.agentStructure.ToString();
		textBranch.text = GridArray.Instance.agentStack.agentBranch.ToString();

		textAgentAmount.text = GridArray.Instance.agentStack.agentAmount.ToString();


	}

	private bool IsMouseOverUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
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
				Debug.Log("You have clicked the purple button!");

				spawnerScript.spawnMode = true;
				spawnerScript.SpawnAgent(agentFoundation, hitList[i].gameObject.transform.position);
				return agentFoundation;

			}
						

			if (hitList[i].gameObject.name == "ButtonStructure")
			{
				Debug.Log("You have clicked the yellow button!");

				spawnerScript.spawnMode = true;
				spawnerScript.SpawnAgent(agentStructure, hitList[i].gameObject.transform.position);
				return agentStructure;

			}

			if (hitList[i].gameObject.name == "ButtonBranch")
			{

				spawnerScript.spawnMode = true;
				spawnerScript.SpawnAgent(agentBranch, hitList[i].gameObject.transform.position);
				return agentBranch;

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