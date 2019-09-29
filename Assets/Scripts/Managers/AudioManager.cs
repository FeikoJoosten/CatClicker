using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	private List<AudioSource> sfxAudio = new List<AudioSource>();
	private List<AudioSource> musicAudio = new List<AudioSource>();

	private float savedSFXVolume = 100;
	private float savedMusicVolume = 100;

	public float SavedSFXVolume
	{
		get { return savedSFXVolume; }
	}
	public float SavedMusicVolume
	{
		get { return savedMusicVolume; }
	}

	private void Awake()
	{
		if (PlayerPrefs.HasKey("SFXVolume") == true && PlayerPrefs.HasKey("MusicVolume") == true)
		{
			savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume");
			savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume");

			UpdateAudioVolumes(savedSFXVolume, savedMusicVolume);
		}
		else
		{
			UpdateAudioVolumes(100, 100);
		}
	}

	public void AddSFXAudioSource(AudioSource source)
	{
		source.volume = savedSFXVolume;
		sfxAudio.Add(source);
	}

	public void AddMusicAudioSource(AudioSource source)
	{
		source.volume = savedMusicVolume;
		musicAudio.Add(source);
	}

	public void PlaySFXSound(AudioSource sourceToPlayFrom, AudioClip clipToPlay)
	{
		if (sourceToPlayFrom.clip != clipToPlay)
		{
			sourceToPlayFrom.clip = clipToPlay;
		}

		if (sourceToPlayFrom.volume != savedSFXVolume)
		{
			sourceToPlayFrom.volume = savedSFXVolume;
		}

		if (savedSFXVolume == 0)
		{
			return;
		}

		sourceToPlayFrom.Play();
	}

	public void PlayMusicSound(AudioSource sourceToPlayFrom, AudioClip clipToPlay)
	{
		if (savedMusicVolume == 0)
		{
			return;
		}
		if (sourceToPlayFrom.clip != clipToPlay)
		{
			sourceToPlayFrom.clip = clipToPlay;
		}

		if (sourceToPlayFrom.volume != savedMusicVolume)
		{
			sourceToPlayFrom.volume = savedMusicVolume;
		}

		sourceToPlayFrom.Play();
	}

	public void UpdateSFXVolume(float sfxVolume)
	{
		for (int i = 0; i < sfxAudio.Count; i++)
		{
			if (sfxAudio[i] == null) continue;
			sfxAudio[i].volume = sfxVolume;
		}

		savedSFXVolume = sfxVolume;
		PlayerPrefs.SetFloat("SFXVolume", savedSFXVolume);
		PlayerPrefs.Save();
	}

	public void UpdateMusicVolume(float musicVolume)
	{
		for (int i = 0; i < musicAudio.Count; i++)
		{
			if (musicAudio[i] == null) continue;
			musicAudio[i].volume = musicVolume;
		}

		savedMusicVolume = musicVolume;
		PlayerPrefs.SetFloat("MusicVolume", savedMusicVolume);
		PlayerPrefs.Save();
	}

	public void UpdateAudioVolumes(float sFXVolume, float musicVolume)
	{
		UpdateSFXVolume(sFXVolume);
		UpdateMusicVolume(musicVolume);
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
		PlayerPrefs.SetFloat("SFXVolume", savedSFXVolume);
		PlayerPrefs.SetFloat("MusicVolume", savedMusicVolume);
		PlayerPrefs.Save();
	}
}
