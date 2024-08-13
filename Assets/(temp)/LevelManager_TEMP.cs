using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager_TEMP : MonoBehaviour
{

	[SerializeField] private string nextSplashLevel;
	[SerializeField] private float autoLoadNextLevelAfter;


	void Start()
	{
		//if (SceneManager.GetActiveScene().name == "Splash")
		Invoke("LoadNextLevel", autoLoadNextLevelAfter);
	}


	//public void LoadLevel (string name)
	//{
	//	SceneManager.LoadScene(name);
	//}
	
	//public void QuitRequest()
	//{
	//	Application.Quit();
	//}

	private void LoadNextLevel ()
	{
		SceneManager.LoadScene(nextSplashLevel);
	}
}
