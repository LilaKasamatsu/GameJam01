using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUi : MonoBehaviour
{
	[SerializeField] Text textFoundation;
	[SerializeField] Text textStructure;


	[SerializeField] Text textAgentAmount;


	public GameObject agent1;
	public GameObject agent2;
	public GameObject agent3;



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
				spawnerScript.SpawnAgent(agent3, hitList[i].gameObject.transform.position);
				return agent3;

			}

			if (hitList[i].gameObject.name == "ButtonBlack")
			{
				Debug.Log("You have clicked the black button!");

				if( GridArray.Instance.agentStack.agentFoundation >= 5 && GridArray.Instance.agentStack.agentStructure >= 5)
				{
					SpawnSettings.Instance.CreateStartPoint();
					GridArray.Instance.agentStack.agentStructure -= 5;

					GridArray.Instance.agentStack.agentFoundation -= 5;


				}


				//spawnerScript.spawnMode = true;
				//spawnerScript.SpawnAgent(agent2, hitList[i].gameObject.transform.position);
				//return agent2;

			}

			if (hitList[i].gameObject.name == "ButtonStructure")
			{
				Debug.Log("You have clicked the yellow button!");

				spawnerScript.spawnMode = true;
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