using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// [TODO] Refactor intoi separate scripts for in-game stats, popups, end game stats (any others?)
// And link those behaviours through here... So GameplayManager can still reference HUDManager only

public class HUDManager : MonoBehaviour
{
	#region Vars
	[Header("Tutorial")]
	[SerializeField] private GameObject m_tutorialParent;
	[SerializeField] private SettingsData_Base m_tutorialSettingsData;

	[Space]
	[Header("Stats (single player)")]
	[SerializeField] private GameObject m_statsParentP1;
	[SerializeField] private TextMeshProUGUI m_timerTextP1;
	[SerializeField] private Slider m_timerSliderP1;
	[SerializeField] private TextMeshProUGUI m_livesCountP1;
	[SerializeField] private TextMeshProUGUI m_itemsCountP1;

	[Space]
	[Header("Stats (multiplayer)")]
	[SerializeField] private GameObject m_statsParentP2;                    // [TODO][Q] Is this the best way to do this?

	[Space]
	[Header("Input Buttons")]
	[SerializeField] private GameObject m_inputButtonsParentP1;
	[SerializeField] private GameObject m_inputButtonsParentP2;
	[SerializeField] private SettingsData_Base m_inputSettingsData;

	[Space]
	[Header("Pause")]
	[SerializeField] private Button m_pauseButton;
	[SerializeField] private Button m_resumeButton;                         // [TODO][Q] Do we want this in its own PauseManager?
	[SerializeField] private GameObject m_pauseParentGameplay;
	[SerializeField] private GameObject m_pauseParentEditor;

	[Space]
	[Header("EndScreen MASTER Parent")]
	[SerializeField] private GameObject m_allEndScreensParent;

	[Space]
	[Header("End Game (single player)")]
	[SerializeField] private GameObject m_endScreenParentSingle;
	[SerializeField] private TextMeshProUGUI m_endLevelTitle;
	[SerializeField] private TextMeshProUGUI m_endWinLoseTitle;
	[SerializeField] private TextMeshProUGUI m_endTimer;
	[SerializeField] private TextMeshProUGUI m_endMovesCount;
	[SerializeField] private TextMeshProUGUI m_endLivesCount;
	[SerializeField] private TextMeshProUGUI m_endItemsCount;
	[SerializeField] private TextMeshProUGUI m_endTotalScore;
	[SerializeField] private Button m_nextLevelButton;

	[Space]
	[Header("End Game (multiplayer)")]
	[SerializeField] private GameObject m_endScreenParentMulti;

	[Space]
	[Header("End Game (level editor)")]
	[SerializeField] private GameObject m_endScreenParentEditor;
	#endregion


	private void Awake()
	{
		// [TODO] Implement in editor, so we can turn it off and not always have this popup. Or at least be able to close it!
		//InitTutorialPopup();
		
		InitStats();
		InitInputButtons();
		InitPauseMenu();
		InitEndScreen();
	}


	#region INIT
	private void InitTutorialPopup()
	{
		if (bool.Parse(SettingsSystem.GetValue(m_inputSettingsData.Key)))
		{
			m_tutorialParent.SetActive(true);
		}
	}

	private void InitStats()
	{
		// [TODO][IMPORTANT] This wiil also change for multiplayer! Calculate!	<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		// [Q] Is this a correct formula??? Or good enough???
		m_statsParentP1.transform.localPosition = new Vector3(0, -(Camera.main.aspect * Camera.main.aspect * Screen.width), 0);

		m_statsParentP2.SetActive(LevelSelectData.IsMultiplayer);
	}

	private void InitInputButtons()
	{
		if (SettingsSystem.GetValue(m_inputSettingsData.Key) != $"{EInputMode.Buttons}")
			return;
		// [TODO][IMPORTANT] Must deselect IsMultiplayer if entering the LevelEditor!
		m_inputButtonsParentP1.SetActive(true);
		m_inputButtonsParentP2.SetActive(LevelSelectData.IsMultiplayer);
	}

	private void InitPauseMenu()
	{
		// [TODO] Refactor so these bools don't overlap
		// Only need one of them since both are always(?) referenced together!
		m_pauseParentGameplay.SetActive(LevelSelectData.IsInGame);
		m_pauseParentEditor.SetActive(LevelEditorData.IsTestingLevel);
	}

	private void InitEndScreen()
	{
		// [TODO] Move this elsewhere! Will no doubt want to use multiple times?
		// Pretty sure we already try do something like this when setting end screen stats!
		int currentLevelIndex = 0;
		for (int i = 0; i < LevelSelectData.ThemeData.Maps.Length; ++i)
		{
			if (LevelSelectData.MapData == LevelSelectData.ThemeData.Maps[i])
			{
				currentLevelIndex = i + i;
				break;
			}
		}
		m_endLevelTitle.text = $"{LevelSelectData.ThemeData.ThemeName} : {currentLevelIndex}";

		if (LevelEditorData.IsTestingLevel)
			m_endScreenParentEditor.SetActive(true);
		else if (LevelSelectData.IsMultiplayer)
			m_endScreenParentMulti.SetActive(true);
		else
			m_endScreenParentSingle.SetActive(true);
	}
	#endregion



	#region UI Buttons
	public void AssignPauseButton(UnityAction onPause)
	{
		m_pauseButton.onClick.AddListener(onPause);
	}

	// [TODO][Q] Do we want this in its own PauseManager?
	public void AssignResumeButton(UnityAction onResume)
	{
		m_resumeButton.onClick.AddListener(onResume);
	}


	public void NextLevel()
	{
		LevelSelectData.ChosenMapIndex++;
		LevelSelectData.MapData = LevelSelectData.ThemeData.Maps[LevelSelectData.ChosenMapIndex];
		RetryLevel();
	}

	public void RetryLevel()
	{
		SceneManager.LoadScene("LevelScene");
	}

	// [TODO] Turn into an actual 'exit' button, in the pause menu.
	// [TODO] Ask the player if they're sure they want to quit!
	// [TODO] Implement via EndGame(EGameEndState.Quit) ???
	//			Or can just delete that EGameEndState?
	// Used by both pause menu and the end screen... Only want to show 'are you sure' popup if we're in the pause menu
	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void ReturnToLevelEditor()
	{
		SceneManager.LoadScene("LevelEditor");
	}
	#endregion



	#region GameplayManager Calls (SETs)
	public void SetTimerSliderActive(bool active)
	{
		m_timerSliderP1.gameObject.SetActive(active);
	}

	public void SetItemsCountActive(bool active)
	{
		m_itemsCountP1.gameObject.SetActive(active);
	}
	#endregion



	#region GameplayManager Calls (updates)
	public void UpdateLivesCount(int lives)
	{
		m_livesCountP1.text = $"Lives: {lives}";
	}

	public void UpdateItemsCount(int items)
	{
		m_itemsCountP1.text = $"Items: {items}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
	}

	public void UpdateTimerTextExit(int timeTaken)
	{
		m_timerTextP1.text = $"Time Taken: {timeTaken}s";
	}

	public void UpdateTimerTextItems(float timeLeft)
	{
		m_timerTextP1.text = $"Time Left: {timeLeft}s";
	}

	public void UpdateTimerSlider(float timeLeft)
	{
		m_timerSliderP1.value = 1 - (timeLeft / LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit);
	}
	#endregion



	#region GameplayManager Calls (end screen)
	public void SetEndScreenStats(int totalScore, float timeTaken, int movesTaken, int livesLeft, bool isItemsGameMode = false, int itemsCollected = 0)
	{
		m_endTotalScore.text = $"Total Score: {totalScore:n0}";
		m_endTimer.text = $"Time Taken: {timeTaken}s";
		m_endMovesCount.text = $"Moves Taken: {movesTaken}";
		m_endLivesCount.text = $"Lives Left: {livesLeft}";
		if (isItemsGameMode)
			m_endItemsCount.text = $"Items Found: {itemsCollected}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
		else
			m_endItemsCount.gameObject.SetActive(false);
	}

	public void ShowEndScreen(bool isWin)
	{
		m_endWinLoseTitle.text = (isWin) ? "Yay! You Win!" : "Uh-oh! You lost!";
		if (LevelSelectData.MapData == LevelSelectData.ThemeData.Maps[LevelSelectData.ThemeData.Maps.Length - 1])
		{
			m_nextLevelButton.gameObject.SetActive(false);
		}
		else
		{
			// [TODO] If failed, switch the position (function) of the RETRY button and the SKIP button
			// ... If doing that, will have to switch back upon winning, if we're not reloading the scene... Or do we just reload the scene?
			m_nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = (isWin) ? "Next Level" : "Skip Level?";
		}
		m_allEndScreensParent.SetActive(true);
	}
	#endregion
}
