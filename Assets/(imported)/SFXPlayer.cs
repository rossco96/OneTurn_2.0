using UnityEngine;
using System.Collections;

public class SFXPlayer : MonoBehaviour {

	public AudioClip[] soundEffectsArray;

	public static AudioSource audioSource;
	public static AudioClip[] sfxArraySTAT;



	void Start () {
		audioSource = GetComponent<AudioSource>();
		//if (audioSource) { audioSource.volume = PlayerPrefsManager.GetVolumeSFX(); }					// DELETED
		sfxArraySTAT = new AudioClip[3];
		for (int i=0; i<3; i++) {
			if (i < soundEffectsArray.Length) {
				sfxArraySTAT[i] = soundEffectsArray[i];
			}
		}
	}
	


	public void PlaySelectSound () {
		AudioClip selectSound = null;
		if (soundEffectsArray[0]) { selectSound = soundEffectsArray[0]; }
		audioSource.clip = selectSound;
		audioSource.Play();
	}



	public void PlayCrashSound () {
		CrashSound();
	}



	public static void CrashSound () {
		AudioClip crashSound = null;
		if (sfxArraySTAT[1]) { crashSound = sfxArraySTAT[1]; }
		audioSource.clip = crashSound;
		audioSource.Play();
	}



	public static void PlayItemCollectSound () {
		AudioClip collectSound = null;
		if (sfxArraySTAT[2]) { collectSound = sfxArraySTAT[2]; }
		audioSource.clip = collectSound;
		audioSource.Play();
	}
}
