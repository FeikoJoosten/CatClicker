using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

#if (UNITY_ANDROID)
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GooglePlayGamesManager : MonoBehaviour {
	private bool isSaving;

	public void Authenticate()
	{
		if (CheckIfThePlayerIsValid() == true) return;

		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			.EnableSavedGames()
			.Build();

		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate();

		LoginToGooglePlayGames();
	}

	public void LoginToGooglePlayGames()
	{
		if (CheckIfThePlayerIsValid() == true) return;
		
		Social.localUser.Authenticate((bool succes) =>
		{
			if (succes)
			{
				OnLoginSucces();
				Debug.Log("Login succesfull");
			}
			else
			{
				Debug.LogWarning("Failed to sign in for google play games");
			}
		});
	}

	private void OnLoginSucces()
	{
		//SaveGame();
	}

	public void LogOutFromGooglePlayGames()
	{
		if (Social.localUser.authenticated == false) return;

		((PlayGamesPlatform)Social.Active).SignOut();
	}

	public void SaveGame(string fileName = null)
	{
		if (!CheckIfThePlayerIsValid()) return;

		// Cloud save is not in ISocialPlatform, it's a Play Games extension,
		// so we have to break the abstraction and use PlayGamesPlatform:
		Debug.Log("Saving progress to the cloud...");
		isSaving = true;
		if (fileName == null)
		{
			((PlayGamesPlatform)Social.Active).SavedGame.ShowSelectSavedGameUI("Save game progress",
				5, true, true, OnSavedGameSelected);
		}
		else
		{
			// save to named file
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(fileName,
				DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				OnSavedGameOpened);
		}
	}

	public void LoadSaveFileFromCloudWithSelectionUI(uint maxNumToDisplay = 5, bool allowCreateNew = false, bool allowDelete = false)
	{
		if (CheckIfThePlayerIsValid() == false) return;

		isSaving = false;

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ShowSelectSavedGameUI("Select saved game to load",
			maxNumToDisplay,
			allowCreateNew,
			allowDelete,
			OnSavedGameSelected);
	}

	public void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
	{
		if (status == SelectUIStatus.SavedGameSelected)
		{
			string filename = game.Filename;
			Debug.Log("opening saved game:  " + game);
			if (isSaving && string.IsNullOrEmpty(filename))
			{
				filename = "save" + DateTime.Now.ToBinary();
			}

			//open the data.
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(filename,
				DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				OnSavedGameOpened);
		}
		else
		{
			Debug.LogWarning("Error selecting save game: " + status);
		}
	}

	public void OpenSavedGame(string filename)
	{
		if (CheckIfThePlayerIsValid() == false) return;

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
	}

	public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			if (isSaving)
			{
				Debug.Log("Saving to " + game);
				byte[] data = GameManager.GetInstance().GetPlayerPrefsAsBytes();
				//TimeSpan playedTime = mProgress.TotalPlayingTime;
				SavedGameMetadataUpdate.Builder builder = new
						SavedGameMetadataUpdate.Builder()
					//.WithUpdatedPlayedTime(playedTime)
					.WithUpdatedDescription("Saved Game at " + DateTime.Now);

				SavedGameMetadataUpdate updatedMetadata = builder.Build();
				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata, data, OnSavedGameWritten);
			}
			else
			{ 
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, OnSaveGameLoaded);
			}
		}
		else
		{
			Debug.LogWarning("Error opening game: " + status);
		}
	}

	public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			Debug.Log("Game " + game.Description + " written");
		}
		else
		{
			Debug.LogWarning("Error saving game: " + status);
		}
	}

	public void OnSaveGameLoaded(SavedGameRequestStatus status, byte[] data)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			Debug.Log("SaveGameLoaded, success=" + status);
			GameManager.GetInstance().ProcessSavedCloudData(data);
		}
		else
		{
			Debug.LogWarning("Error reading game: " + status);
		}
	}

	public void LoadGameData(ISavedGameMetadata game)
	{
		if (CheckIfThePlayerIsValid() == false) return;

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
	}

	public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			// handle processing the byte array data
		}
		else
		{
			// handle error
		}
	}

	public void DeleteGameData(string filename)
	{
		if (CheckIfThePlayerIsValid() == false) return;

		// Open the file to get the metadata.
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLongestPlaytime, DeleteSavedGame);
	}

	public void DeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
			savedGameClient.Delete(game);
		}
		else
		{
			// handle error
		}
	}

	private bool CheckIfThePlayerIsValid()
	{
		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WindowsEditor)
		{
			Debug.LogWarning("Currently we are not on android or windows editor, so we'll ignore the google play games call");
			return false;
		}

		if (Social.Active.localUser.authenticated) return true;

		Debug.Log("You're currently not logged in");
		return false;
	}
}
#endif