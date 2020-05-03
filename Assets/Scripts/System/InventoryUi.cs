using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUi : MonoBehaviour
{
	public Button agentButton1;
	public Button agentButton2;
	public Button agentButton3;

	public GameObject agent1;
	public GameObject agent2;
	public GameObject agent3;



	GameObject spawnController;
	private SpawnSettings spawnerScript;

	void Start()
	{
		spawnController = GameObject.Find("spawnController");
		spawnerScript = spawnController.GetComponent<SpawnSettings>();

		//Button button1 = agentButton1.GetComponent<Button>();
		//btn.onClick.AddListener(TaskOnClick);



	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			CheckButton();
		}
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
			if (hitList[i].gameObject.name == "ButtonPurple")
			{
				Debug.Log("You have clicked the purple button!");

				spawnerScript.SpawnAgent(agent3, hitList[i].gameObject.transform.position);
				return agent3;

			}

			if (hitList[i].gameObject.name == "ButtonBlack")
			{
				Debug.Log("You have clicked the black button!");

				spawnerScript.SpawnAgent(agent2, hitList[i].gameObject.transform.position);
				return agent2;

			}

			if (hitList[i].gameObject.name == "ButtonYellow")
			{
				Debug.Log("You have clicked the yellow button!");

				spawnerScript.SpawnAgent(agent1, hitList[i].gameObject.transform.position);
				return agent1;

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