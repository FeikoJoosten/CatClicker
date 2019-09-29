using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickFeedback : OverridableMonoBehaviour
{
	[Header("UI")]
	[SerializeField] private Text CPValueText = null;

	[SerializeField] private Outline outline = null;

	[Header("Movement")]
	[SerializeField] private float lifeTime = 2;
	[SerializeField] private float movementSpeed = 2;
	
	private float startLifeTimeValue;

	private void Start()
	{
		startLifeTimeValue = lifeTime;
	}

	public void SetCPValueText(float value, Vector3 spawnPos)
	{
		CPValueText.text = "+" + string.Format("{0:n1}", value);
		CPValueText.gameObject.transform.position = spawnPos;
	}

	public override void UpdateMe()
	{
		if (lifeTime <= 0) Destroy(gameObject);

		if (lifeTime <= startLifeTimeValue * 0.5f)
		{
			if(CPValueText)
				CPValueText.color = Color.Lerp(CPValueText.color, new Color(CPValueText.color.r, CPValueText.color.g, CPValueText.color.b, lifeTime / startLifeTimeValue), Time.deltaTime);

			if(outline)
				outline.effectColor = Color.Lerp(outline.effectColor, new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, lifeTime / startLifeTimeValue), Time.deltaTime);
		}

		CPValueText.gameObject.transform.position = Vector3.Lerp(CPValueText.gameObject.transform.position, CPValueText.gameObject.transform.position + Vector3.up * movementSpeed, Time.deltaTime);
		lifeTime -= Time.deltaTime;
	}
}
