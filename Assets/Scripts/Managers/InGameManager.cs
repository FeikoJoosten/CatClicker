using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : OverridableMonoBehaviour
{
	[SerializeField] private Text currentCPText = null;
	[SerializeField] private Text currentCPSText = null;
	[SerializeField] private Text currentCPStoreText = null;
	[SerializeField] private RectTransform[] scrollRecttContentTransforms = null;
	[SerializeField] private ClickFeedback clickFeedback = null;
	[SerializeField] private RectTransform clickFeedbackSpawnArea = null;

	private CultureInfo systemLanguage;

	private void Start()
	{
		systemLanguage = ScreenManager.GetCultureDefualt(Application.systemLanguage);

		for (int i = 0; i < scrollRecttContentTransforms.Length; i++)
		{
			if (scrollRecttContentTransforms[i].childCount == 0) continue;

			float itemSpacing = scrollRecttContentTransforms[i].GetComponent<VerticalLayoutGroup>().spacing;

			float defaultHeight = scrollRecttContentTransforms[i].GetChild(0).GetComponent<RectTransform>().rect.height + itemSpacing;
			float contentHeight = (defaultHeight * scrollRecttContentTransforms[i].childCount) - itemSpacing;
			scrollRecttContentTransforms[i].sizeDelta = new Vector2(scrollRecttContentTransforms[i].sizeDelta.x, contentHeight);
		}

		GameManager.GetInstance().GetScoreManager().onAutoClick += SpawnAutoCPSFeedback;
	}

	private void SetCurrentCPText(float CPValue)
	{
		Debug.Log(systemLanguage.NumberFormat.GetFormat(RegionInfo.CurrentRegion.GetType()));
		currentCPText.text = CPValue.ToString("N0", systemLanguage.NumberFormat);
		currentCPStoreText.text = currentCPText.text;
	}

	private void SetCurrentCPSText(float CPSValue)
	{
		currentCPSText.text = CPSValue.ToString("N1", systemLanguage.NumberFormat);
	}

	public override void UpdateMe()
	{
		SetCurrentCPText(GameManager.GetInstance().GetScoreManager().CurrentCP);
		SetCurrentCPSText(GameManager.GetInstance().GetScoreManager().CurrentCPS);
	}

	public void SaveGame()
	{
#if UNITY_ANDROID
		GameManager.GetInstance().GetGooglePlayGamesManager().SaveGame(); 
#endif
	}

	public void LoadGame()
	{
#if UNITY_ANDROID
		GameManager.GetInstance().GetGooglePlayGamesManager().LoadSaveFileFromCloudWithSelectionUI();
#endif
	}

	public void SpawnAutoCPSFeedback()
	{
		Vector2 spawnPos = new Vector2( UnityEngine.Random.Range(clickFeedbackSpawnArea.rect.min.x, clickFeedbackSpawnArea.rect.max.x), 
										UnityEngine.Random.Range(clickFeedbackSpawnArea.rect.min.y, clickFeedbackSpawnArea.rect.max.y));
		ClickFeedback feedback = Instantiate(clickFeedback, clickFeedbackSpawnArea.transform);
		feedback.SetCPValueText(GameManager.GetInstance().GetScoreManager().CurrentAutoCPS, spawnPos);
	}
}
