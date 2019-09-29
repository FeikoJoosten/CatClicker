using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ScreenManager : OverridableMonoBehaviour {
	[SerializeField] private RectTransform startScreen = null;
	[SerializeField] private RectTransform allScreens = null;
	[SerializeField] private float cameraAnimationSpeed = 10;

	private RectTransform previousScreen;
	private RectTransform targetScreen;
	private Vector3 targetScreenPos;
	private Vector3 cameraAnimationStartPos;

	private float animationStartTime = 0;
	private float totalAnimationmovementDistance = 0;

	public static CultureInfo GetCultureDefualt(SystemLanguage language)
	{
		CultureInfo[] cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures);
		string cultureName = string.Empty;
		foreach (CultureInfo cult in cultureInfo)
		{
			string[] name = cult.EnglishName.Split(new[] {" "}, StringSplitOptions.None);

			if (name.Length == 0) continue;

			//Debug.Log(name[0] + " - " + Enum.GetName(language.GetType(), language));
			if (name[0] != Enum.GetName(language.GetType(), language)) continue;

			Debug.Log("Language: " + cult.Name + " Region info: " + RegionInfo.CurrentRegion);
			cultureName = cult.Name;
			break;
		}

		Debug.Log("Final Chosen name: " + cultureName);

		if(cultureName == string.Empty)
			return CultureInfo.CurrentCulture;

		return CultureInfo.CreateSpecificCulture(cultureName);
	}

	private void Start()
	{
		if (allScreens.childCount > 0)
		{
			float screenHeightInUnits = Camera.main.orthographicSize * 2;
			float screenWidthInUnits = screenHeightInUnits * Screen.width / Screen.height;
			float defaultWidth = allScreens.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

			for (int i = 0; i < allScreens.childCount; i++)
			{
				RectTransform child = allScreens.GetChild(i).GetComponent<RectTransform>();
				int position = Mathf.RoundToInt(child.position.x / defaultWidth);
				child.sizeDelta = new Vector2(screenWidthInUnits, screenHeightInUnits);
				child.position = new Vector3(child.sizeDelta.x * position, 0);
			}

			allScreens.sizeDelta = new Vector2(screenWidthInUnits, screenHeightInUnits);
		}

		AnimateToScreen(startScreen);
	}
	
	public void AnimateToScreen(RectTransform screenToAnimateTowards)
	{
		previousScreen = targetScreen;
		targetScreen = screenToAnimateTowards;

		if (targetScreen.gameObject.activeInHierarchy == false)
			targetScreen.gameObject.SetActive(true);

		targetScreenPos = new Vector3(screenToAnimateTowards.position.x, 0);
		cameraAnimationStartPos = Camera.main.transform.position;

		animationStartTime = Time.time;
		totalAnimationmovementDistance = Vector3.Distance(cameraAnimationStartPos, targetScreenPos);
	}

	public override void UpdateMe()
	{
		if (Camera.main.transform.position == targetScreenPos)
		{
			if (previousScreen != null && previousScreen.gameObject.activeInHierarchy == true && previousScreen != startScreen)
				previousScreen.gameObject.SetActive(false);
			return;
		}

		float distCovered = (Time.time - animationStartTime) * cameraAnimationSpeed;
		float fracJourney = distCovered / totalAnimationmovementDistance;
		Camera.main.transform.position = Vector3.Lerp(cameraAnimationStartPos, targetScreenPos, fracJourney);
	}
}
