using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour 
{
	public delegate void OnCloudDataReceived();
	public OnCloudDataReceived onCloudDataReceived;

	private Dictionary<int, int> storeObjects = new Dictionary<int, int>();

	public Dictionary<int, int> StoreObjects
	{
		get { return storeObjects; }
	}

	public int GetSavedInformation(StoreItem objectToCheck)
	{
		int hashCode = objectToCheck.storeInformation.name.GetHashCode();
		if (!PlayerPrefs.HasKey(hashCode.ToString())) return 0;

		if(!storeObjects.ContainsKey(hashCode))
			storeObjects.Add(hashCode, PlayerPrefs.GetInt(hashCode.ToString()));

		return PlayerPrefs.GetInt(hashCode.ToString());
	}

	public bool BuyItem(StoreItem objectToBuy)
	{
		if (objectToBuy.PurchaseCose > GameManager.GetInstance().GetScoreManager().CurrentCP) return false;

		int hashID = objectToBuy.storeInformation.name.GetHashCode();
		
		if (!storeObjects.ContainsKey(hashID))
			storeObjects.Add(hashID, 0);

		storeObjects[hashID] = storeObjects[hashID] + 1;

		GameManager.GetInstance().GetScoreManager().RemoveCP(objectToBuy.PurchaseCose);

		return true;
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
		foreach (KeyValuePair<int, int> pair in storeObjects)
		{
			PlayerPrefs.SetInt(pair.Key.ToString(), pair.Value);
		}

		PlayerPrefs.Save();
	}
	
	public string GetAllInformationAsString()
	{
		string stringToReturn = string.Empty;

		foreach (KeyValuePair<int, int> pair in storeObjects)
		{
			PlayerPrefs.SetInt(pair.Key.ToString(), pair.Value);
			stringToReturn += pair.Key + "," + pair.Value + ";";
		}
		
		return stringToReturn;
	}

	public void HandleCloudInformation(string[] allCloudInfo)
	{
		List<string> storeCloudInfo = new List<string>();

		for (int i = 0; i < allCloudInfo.Length; i++)
		{
			if (string.IsNullOrEmpty(allCloudInfo[i])) continue;
			
			storeCloudInfo.Add(allCloudInfo[i]);
		}

		for (int i = 0; i < storeCloudInfo.Count; i++)
		{
			string[] storeItemInfo = storeCloudInfo[i].Split(new[] {","}, StringSplitOptions.None);

			int hashID = int.Parse(storeItemInfo[0]);
			int purchasedCount = int.Parse(storeItemInfo[1]);

			if (!storeObjects.ContainsKey(hashID))
				storeObjects.Add(hashID, 0);

			if(purchasedCount > storeObjects[hashID])
				storeObjects[hashID] = purchasedCount;
		}

		onCloudDataReceived();
	}
}
