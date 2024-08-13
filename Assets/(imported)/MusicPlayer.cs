using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	public AudioClip[] levelMusicChangeArray;

	public static AudioSource audioSource;

	private string level;
	private AudioClip thisLevelMusic;

	//static MusicPlayer instance = null;



	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	void Start () {
		audioSource = GetComponent<AudioSource>();
		//if (audioSource) { audioSource.volume = PlayerPrefsManager.GetVolumeMusic(); }					// DELETED
		PlayMusic();
	}



	void PlayMusic () {
		if (level == null || level == "00 Splash") {
			thisLevelMusic = levelMusicChangeArray[0];
		} else if (level == "01 Menu" || level == "02 Options" ||
			level == "03a Single" || level == "04a Multi") {
			thisLevelMusic = levelMusicChangeArray[1];
		} else if (level == "99a Win" || level == "99M End") {
			thisLevelMusic = levelMusicChangeArray[3];
		} else if (level == "99b Lose") {
			thisLevelMusic = levelMusicChangeArray[4];
		} else {
			thisLevelMusic = levelMusicChangeArray[2];
		}

		if (thisLevelMusic) {
			if (level == null || level == "00 Splash" || level == "99a Win" ||
				level == "99b Lose" || level == "99M End") {
				if (audioSource.clip != thisLevelMusic) {
					audioSource.clip = thisLevelMusic;
					audioSource.loop = false;
					audioSource.Play();
				}
			} else {
				if (audioSource.clip != thisLevelMusic) {
					audioSource.clip = thisLevelMusic;
					audioSource.loop = true;
					audioSource.Play();
				}
			}
		}
	}


	/*
	void OnLevelWasLoaded () {
		level = SceneManager.GetActiveScene().name;

		if (thisLevelMusic) {
			PlayMusic();
		}
	}
	//*/
}
