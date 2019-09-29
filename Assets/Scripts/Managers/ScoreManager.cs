using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	private float currentCP;
	private float currentCPS;
	private float currentAutoCPS;
	public StoreManager.OnCloudDataReceived onCloudDataReceived;

	public delegate void OnAutoClick();
	public OnAutoClick onAutoClick;

	public float CurrentCP
	{
		get { return currentCP; }
	}
	public float CurrentCPS
	{
		get { return currentCPS; }
	}
	public float CurrentAutoCPS
	{
		get { return currentAutoCPS; }
	}

	private void Start()
	{
		if (PlayerPrefs.HasKey("CurrentCP"))
			currentCP = float.Parse(PlayerPrefs.GetString("CurrentCP"));

		StartCoroutine(CalculateAndSetCPS());

		currentCPS = currentAutoCPS;
	}

	public void HandleCloudData(string cloudInformation)
	{
		string[] cloudData = cloudInformation.Split(new[] {","}, StringSplitOptions.None);

		float cloudCPValue = float.Parse(cloudData[0]);

		if (cloudCPValue > currentCP)
			currentCP = cloudCPValue;
	}

	public string GetAllInformationAsString()
	{
		return currentCP.ToString();
	}

	[EditorButton(new object[] {1000})]
	public void AddCP(float valueToAdd)
	{
		currentCP += valueToAdd;
	}

	public void AddAutoCP()
	{
		if (currentAutoCPS == 0) return;

		AddCP(currentAutoCPS);
		onAutoClick();
	}

	public void AddAutoCPS(float autoCPSToAdd)
	{
		currentAutoCPS += autoCPSToAdd;
	}

	public void RemoveCP(float valueToRemove)
	{
		currentCP -= valueToRemove;
	}
	
	private IEnumerator CalculateAndSetCPS()
	{
		float currentCPAtStart = currentCP;

		yield return new WaitForSeconds(1);

		AddAutoCP();
		float CPSDifference = currentCP - currentCPAtStart;

		if (CPSDifference > 0)
			currentCPS = currentCP - currentCPAtStart;
		else
			currentCPS = CurrentCPS;

		StartCoroutine(CalculateAndSetCPS());
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
		PlayerPrefs.SetString("CurrentCP", currentCP.ToString());
		PlayerPrefs.Save();
	}
}
