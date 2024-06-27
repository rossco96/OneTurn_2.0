using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashAnimator : MonoBehaviour
{
	// [TODO] This is ALL temp, just so we can progress to the next level!
	private const string k_mainMenuSceneName = "MainMenu";
	private const float k_waitTime = 2.0f;

	private float m_currentWaitTime = 0.0f;

	private void Update()
	{
		m_currentWaitTime += Time.deltaTime;
		if (m_currentWaitTime >= k_waitTime)
		{
			LoadMainMenu();
		}
	}

	private void LoadMainMenu()
	{
		SceneManager.LoadScene(k_mainMenuSceneName);
	}
}
