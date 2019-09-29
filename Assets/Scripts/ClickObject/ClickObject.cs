using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour 
{
	[SerializeField] private float baseClickValue = 1;
	[SerializeField] private ClickFeedback clickFeedback = null;

	private float baseClickValueMultiplier = 1;

	public void AddBaseValue(float valueToAdd)
	{
		baseClickValue += valueToAdd;
	}

	public void AddPercentage(float pertencageToAdd)
	{
		baseClickValueMultiplier += pertencageToAdd;
	}

	public void OnClick()
	{
		GameManager.GetInstance().GetScoreManager().AddCP(baseClickValue * baseClickValueMultiplier);
		
		//SpawnFeedbackObject();

		//if (Application.platform != RuntimePlatform.WindowsEditor || !Input.GetMouseButtonUp(0)) return;

		ClickFeedback editorFeedback = Instantiate(clickFeedback, transform);
		editorFeedback.SetCPValueText(baseClickValue * baseClickValueMultiplier, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y)));
	}

	private void SpawnFeedbackObject()
	{
		for (int i = 0; i < Input.touches.Length; i++)
		{
			if (Input.touches[i].phase != TouchPhase.Began) continue;

			ClickFeedback feedback = Instantiate(clickFeedback, transform);
			feedback.SetCPValueText(baseClickValue * baseClickValueMultiplier, Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y)));
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		Save();
	}

	private void OnApplicationQuit()
	{
		Save();
	}

	private void Save()
	{
		PlayerPrefs.SetString("ClickObjectMultiplier", baseClickValueMultiplier.ToString());
		PlayerPrefs.SetFloat("ClickObject", baseClickValue);
		PlayerPrefs.Save();
	}
}
