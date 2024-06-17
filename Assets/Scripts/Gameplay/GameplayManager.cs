using UnityEngine;
using UnityEngine.Events;

public abstract class GameplayManager : MonoBehaviour
{
	private const float k_introWaitTime = 0.5f;

	protected HUDManager m_hudManager;
	public void SetHUDManager(HUDManager hudManager)
	{ 
		m_hudManager = hudManager;
		m_hudManager.AssignPauseButton(SetPause);
		m_hudManager.AssignResumeButton(SetResume);
		m_hudManager.AssignNextLevelButton(NextLevel);
	}

	// [NOTE] Want to know Grid Dimension so can determine the points multiplier -- Do here? Or separate ScoreManager?
	// [NOTE] THESE ARE NO LONGER NEEDED HERE -- THEY ARE STORED IN LevelSelectData (static)
	//private int m_gridDimension;
	
	protected OTController[] m_controllers;
	
	// [TODO] Option to set lives to 1 or 3 or 5 or unlimited?
	// If unlimited, don't allow recording the score? Same with unlimited time?
	// ^^^ HAVE AS CHEAT CODES YAY
	//private int m_livesCount = 3;										// [TODO] Delete! Now managing with InGameStats via each controller. Keep this until TODO above is completed

	private float m_introStartTime = 0.0f;
	private bool m_hasStarted = false;
	private bool m_hasEnded = false;									// [TODO] Won't need this eventually?
	private bool m_isPaused = false;
	private void SetPause() { m_isPaused = true; OnPauseChanged(true); }
	private void SetResume() { m_isPaused = false; OnPauseChanged(false); }
	private UnityAction<bool> OnPauseChanged;

	protected float m_levelStartTime = 0.0f;
	protected float m_totalTimePaused = 0.0f;

	// Note the below are used in children only
	protected float m_levelTimeFloat = 0.0f;		// items and exit
	protected int m_levelTimeInt = 0;				// items and exit
	protected int m_timeLimit = 0;                  // items
	protected int m_itemCount = 0;					// items



	protected virtual void Start()
	{
		m_controllers = FindObjectsOfType<OTController>();
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			OnPauseChanged += m_controllers[i].SetInputDisabled;
			m_controllers[i].RespawnPlayer();
		}

		if (LevelEditorData.IsTestingLevel == false)
		{
			InitInteractableBehaviour<Border>(OnPlayerInteractWall);
			InitInteractableBehaviour<Wall>(OnPlayerInteractWall);
		}

		//InitInteractableBehaviour<Special>(OnPlayerInteractSpecial);			// e.g. booster or turn pads

		InitHUD();

		// [TODO][Q]
		// Have optional 3s countdown intro to the level (toggle in options)
		// --> Should always have brief 0.5s to 1s pause regardless?
		// Also have scene transition -- e.g. black circle in and out

		m_introStartTime = Time.time;                               // Rather call in Awake? Or elsewhere?
	}

	private void Update()
	{
		if (m_hasEnded) return;

		if (m_hasStarted == false)
		{
			if (Time.time - m_introStartTime > k_introWaitTime)
			{
				m_levelStartTime = Time.time;
				m_hasStarted = true;
				for (int i = 0; i < m_controllers.Length; ++i)
					m_controllers[i].SetInputDisabled(false);
			}
			return;
		}

		if (m_isPaused)
		{
			m_totalTimePaused += Time.deltaTime;
			return;
		}

		UpdateTimer();
	}



	protected void InitInteractableBehaviour<T>(UnityAction<OTController> action) where T : Interactable_Base
	{
		T[] interactables = FindObjectsOfType<T>();
		for (int i = 0; i < interactables.Length; ++i)
		{
			interactables[i].PlayerEnterEvent += action;
		}
	}

	protected virtual void InitHUD()
	{
		if (LevelSelectData.IsMultiplayer)
		{
			// [TODO] Update both sets of player lives
		}
		else
		{
			// [NOTE] This may also need to be overriden if testing a level editor level! Not essential, though
			m_hudManager.UpdateLivesCount(LevelSelectData.LivesCount);              // [TODO] May want to allow user choosing 1, 3, 5, or unlimited lives
		}
	}



	protected abstract void UpdateTimer();



	private void OnPlayerInteractWall(OTController controller)
	{
		controller.SetInputDisabled(true);
		controller.DestroyPlayerGameObject();

		if (controller.Stats.RemoveLife() == false)
		{
			EndGame(false, controller);
		}
		else
		{
			// [NOTE] This should be done after the death animation is complete! Need another callback...
			// [NOTE] Need also a spawn animation, and THEN can resume control of player
			controller.RespawnPlayer();

			// [TODO] DO THIS ELSEWHERE! Needs to be done after death animation complete
			controller.SetInputDisabled(false);
		}

		m_hudManager.UpdateLivesCount(controller.Stats.Lives);
	}



	// [TODO][Q] Can we delete OTController?
	// Also can we please sort out whatever is going on with m_controllers indexing - this looks terrible
	protected virtual void EndGame(bool isWin, OTController controller)
	{
		m_hasEnded = true;
		m_controllers[0].SetInputDisabled(true);
		m_hudManager.ShowEndScreen(isWin);
	}

	protected void EndGameMultiplayer()
	{
		m_hasEnded = true;
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			//if (m_controllers[i] != null)								// [TODO][Q] Should no longer need this nullcheck? No longer destroying the controller!
			m_controllers[i].SetInputDisabled(true);
		}

		Debug.LogWarning($"END GAME MULTIPLAYER");
	}





	#region UI Buttons
	// [NOTE] This *is* currently in use! Don't need to label it "TEST"?
	public void NextLevel()
	{
		Debug.Log("[GameplayManager::NextLevel]");

		//LevelSelectData.MapData = LevelSelectData.ThemeData.Maps[LevelSelectData.ThemeData.]		// [TODO] Implement!
		UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");							// [Q] Is this the best way to do it? Probably
	}

	// [TODO] Turn into an actual 'exit' button, in the pause menu.
	// [TODO] Ask the player if they're sure they want to quit!
	// [TODO] Implement via EndGame(EGameEndState.Quit) ???
	//			Or can just delete that EGameEndState?
	// Used by both pause menu and the end screen... Only want to show 'are you sure' popup if we're in the pause menu
	public void ReturnToMainMenu()
	{
		Debug.Log("[GameplayManager::ReturnToMainMenu]");
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	public void ReturnToLevelEditor()
	{
		Debug.Log("[GameplayManager::ReturnToMainMenu]");
		UnityEngine.SceneManagement.SceneManager.LoadScene("LevelEditor");
	}
	#endregion
}
