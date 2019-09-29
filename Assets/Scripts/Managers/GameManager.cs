using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;
	private AudioManager audioManager = null;
	private ScoreManager scoreManager = null;
	private UpdateManager updateManager = null;
	private StoreManager storeManager = null;
#if (UNITY_ANDROID)
	private GooglePlayGamesManager googlePlayGamesManager = null;
#endif

	public static GameManager GetInstance()
	{
		if (instance != null) return instance;

		GameManager[] foundGameManagers = FindObjectsOfType<GameManager>();
		if (foundGameManagers.Length > 1) return foundGameManagers[0];

		GameObject spawnedObject = new GameObject("Game Manager");
		instance = spawnedObject.AddComponent<GameManager>();

		return instance;
	}

	public AudioManager GetAudioManager()
	{
		return audioManager ?? (audioManager = gameObject.AddComponent<AudioManager>());
	}

	public ScoreManager GetScoreManager()
	{
		return scoreManager ?? (scoreManager = gameObject.AddComponent<ScoreManager>());
	}

	public UpdateManager GetUpdateManager()
	{
		return updateManager ?? (updateManager = gameObject.AddComponent<UpdateManager>());
	}

	public StoreManager GetStoreManager()
	{
		return storeManager ?? (storeManager = gameObject.AddComponent<StoreManager>());
	}

#if UNITY_ANDROID
	public GooglePlayGamesManager GetGooglePlayGamesManager()
	{
		if (googlePlayGamesManager != null) return googlePlayGamesManager;

		googlePlayGamesManager = gameObject.AddComponent<GooglePlayGamesManager>();
		googlePlayGamesManager.Authenticate();

		return googlePlayGamesManager;
	} 
#endif

	public byte[] GetPlayerPrefsAsBytes()
	{
		string allInfo = GetScoreManager().GetAllInformationAsString() + ";";
		allInfo += GetStoreManager().GetAllInformationAsString();

		return System.Text.Encoding.Default.GetBytes(allInfo);
	}

	public void ProcessSavedCloudData(byte[] cloudData)
	{
		string dataText = System.Text.Encoding.Default.GetString(cloudData);

		List<string> splitDataText = dataText.Split(new [] {";"}, StringSplitOptions.None).ToList();

		GetScoreManager().HandleCloudData(splitDataText[0]);
		splitDataText.RemoveAt(0);

		GetStoreManager().HandleCloudInformation(splitDataText.ToArray());
	}
}
