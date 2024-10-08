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
	}

	// [NOTE] Want to know Grid Dimension so can determine the points multiplier -- Do here? Or separate ScoreManager?
	// [NOTE] THESE ARE NO LONGER NEEDED HERE -- THEY ARE STORED IN LevelSelectData (static)
	//private int m_gridDimension;
	
	protected OTController[] m_controllers;								// [TODO] Limit to 2x players max? This is accounting for up to 4x players, and choosing how many...
	
	// [TODO] Option to set lives to 1 or 3 or 5 or unlimited?
	// If unlimited, don't allow recording the score? Same with unlimited time?
	// ^^^ HAVE AS CHEAT CODES YAY
	//private int m_livesCount = 3;										// [TODO] Delete! Now managing with InGameStats via each controller. Keep this until TODO above is completed

	private float m_introStartTime = 0.0f;

	// [TODO][IMPORTANT] USE GAME STATES! DUH!
	private bool m_initialised = false;
	private bool m_hasStarted = false;
	protected bool m_hasEnded = false;									// [TODO] Won't need this eventually? [NOTE] Made protected for GameplayManager_LevelEditor ONLY.
	private bool m_isPaused = false;
	private void SetPause() { m_isPaused = true; OnPauseChanged(true); }
	private void SetResume() { m_isPaused = false; OnPauseChanged(false); }
	private UnityAction<bool> OnPauseChanged;

	protected float m_levelStartTime = 0.0f;
	protected float m_totalTimePaused = 0.0f;

	// Note the below are used in children only (then put it into the children, not here? just have duplicates across classes, that's chill?)
	protected float m_levelTimeElapsedFloat = 0.0f;			// items and exit and travel
	protected int m_levelDisplayTimeInt = 0;				// items and exit and travel
	protected float m_countdownTimeRemainingFloat = 0.0f;	// items and travel
	protected float m_timeLimit = 0;						// items and travel, and m_bomb (but halved!)
	protected int m_itemCount = 0;							// items



	protected virtual void Start()
	{
		if (m_hudManager.PauseGameplayManagerOnStart)
		{
			m_hudManager.AssignTutorialCloseButton(Init);
			return;
		}
		Init();
	}

	private void Init()
	{
		m_controllers = FindObjectsOfType<OTController>();
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			OnPauseChanged += m_controllers[i].SetInputDisabled;
			//m_controllers[i].RespawnPlayer();									// Call this from LevelGenerator instead!
		}

		if (LevelEditorData.IsTestingLevel == false)
		{
			InitInteractableBehaviour<Border>(OnPlayerInteractWall);			// [TODO] Move this to outside the IF statement! Figure out where we're re-Initting it (presumably GameMan_Editor?)
			InitInteractableBehaviour<Wall>(OnPlayerInteractWall);
		}

		// [TODO] Refactor the below? And the above??
		SpecialInteractable_Base[] specialInteractables = FindObjectsOfType<SpecialInteractable_Base>();
		if (specialInteractables != null && specialInteractables.Length > 0)
		{
			for (int i = 0; i < specialInteractables.Length; ++i)
			{
				specialInteractables[i].OnPlayerCrash(SpecialEndGame, SpecialUpdateHUDLives);
			}
		}

		InitHUD();

		// [TODO][Q]
		// Have optional 3s countdown intro to the level (toggle in options)
		// --> Should always have brief 0.5s to 1s pause regardless?
		// Also have scene transition -- e.g. black circle in and out

		m_introStartTime = Time.time;                               // Rather call in Awake? Or elsewhere?
		m_initialised = true;
	}

	private void Update()
	{
		if (m_initialised == false || m_hasEnded) return;

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



	protected void InitInteractableBehaviour<T>(UnityAction<OTController, Interactable_Base> action) where T : Interactable_Base
	{
		T[] interactables = FindObjectsOfType<T>();
		for (int i = 0; i < interactables.Length; ++i)
		{
			interactables[i].PlayerEnterEvent += action;
		}
	}


	protected virtual void InitHUD()
	{
		// [NOTE] This may also need to be overriden if testing a level editor level! Not essential, though
		m_hudManager.UpdateLivesCountP1(LevelSelectData.LivesCount);              // [TODO] May want to allow user choosing 1, 3, 5, or unlimited lives

		if (LevelSelectData.IsMultiplayer)
		{
			// [TODO] Update both sets of player lives
			m_hudManager.UpdateLivesCountP2(LevelSelectData.LivesCount);
		}
	}



	protected abstract void UpdateTimer();



	private void OnPlayerInteractWall(OTController controller, Interactable_Base interactable)
	{
		controller.SetInputDisabled(true);
		controller.DestroyPlayerGameObject();

		controller.Stats.Lives--;
		if (controller.Stats.Lives == 0)
		{
			//Debug.Log($"[ccc] {LevelSelectData.IsMultiplayer}");
			if (LevelSelectData.IsMultiplayer)
				EndGameMultiplayer();														// [Q] Do we want to end the game if the other player dies? Or allow keep playing? TEST!
			else
				EndGame(false);
		}
		else
		{
			// [NOTE] This should be done after the death animation is complete! Need another callback...
			// [NOTE] Need also a spawn animation, and THEN can resume control of player
			controller.RespawnPlayer();

			// [TODO] DO THIS ELSEWHERE! Needs to be done after death animation complete
			controller.SetInputDisabled(false);
		}

		if (controller.Index == 0)
			m_hudManager.UpdateLivesCountP1(controller.Stats.Lives);
		else //if (controller.Index == 1)
			m_hudManager.UpdateLivesCountP2(controller.Stats.Lives);
	}


	// DUPLICATE OF ABOVE FOR EXTERNAL ACCESS, BUT MEANS WE CAN REFACTOR THE ABOVE (NO?)
	public void SpecialUpdateHUDLives(bool isP1, int lives)
	{
		if (isP1)
			m_hudManager.UpdateLivesCountP1(lives);
		else //if (controller.Index == 1)
			m_hudManager.UpdateLivesCountP2(lives);
	}

	public void SpecialEndGame(bool isMultiplayer, bool isWin)
	{
		if (isMultiplayer)
			EndGameMultiplayer();
		else
			EndGame(isWin);
	}
	
	// [TODO][Q] Can we delete OTController?
	// Also can we please sort out whatever is going on with m_controllers indexing - this looks terrible
	protected virtual void EndGame(bool isWin)
	{
		m_hasEnded = true;
		m_controllers[0].SetInputDisabled(true);
		m_hudManager.SetWinLoseTitle(isWin);
		m_hudManager.ShowEndScreen();
	}

	protected virtual void EndGameMultiplayer()
	{
		m_hasEnded = true;
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			m_controllers[i].SetInputDisabled(true);
		}
		m_hudManager.ShowEndScreen();
	}
}
