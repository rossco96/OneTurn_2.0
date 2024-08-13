using UnityEngine;

public class MusicPlayer_TEMP : MonoBehaviour
{
	[SerializeField] private AudioClip thisLevelMusic;
	
	private AudioSource audioSource;


	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
		PlayMusic();
	}

	void PlayMusic ()
	{
		audioSource.clip = thisLevelMusic;
		audioSource.loop = false;
		audioSource.Play();
	}
}
