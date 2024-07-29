/*
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	private static TutorialManager m_instance;

	//private TutorialPopup[] m_tutorialPopups;

	private void Awake()
	{
		if (m_instance == null)
		{
			DontDestroyOnLoad(gameObject);
			m_instance = this;
		}
		else if (m_instance != this)
		{
			Destroy(gameObject);
			return;
		}

		// [TODO][Q][IMPORTANT]
		// MUST BE A BETTER WAY TO DO THIS? WANT TO DO THIS HERE?
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += AssignLevelTutorialPopups;
	}

	private void AssignLevelTutorialPopups(UnityEngine.SceneManagement.Scene s, UnityEngine.SceneManagement.LoadSceneMode lsm)
	{
		//m_tutorialPopups = FindObjectsOfType<TutorialPopup>(true);
		TutorialPopup[] tutorialPopups = FindObjectsOfType<TutorialPopup>(true);
		for (int i = 0; i < tutorialPopups.Length; ++i)
		{
			tutorialPopups[i].OnTutorial += this;
		}
	}


}
//*/