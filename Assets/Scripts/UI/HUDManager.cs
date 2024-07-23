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
	[Space]
	[Header("Global Settings")]
	[SerializeField] private SettingsDataInt m_mapIndexSettingsData;

	[Space]
	[Header("Tutorial")]
	[SerializeField] private GameObject m_tutorialParent;
	[SerializeField] private SettingsData_Base m_tutorialSettingsData;

	[Space]
	[Header("Stats (single player)")]
	[SerializeField] private GameObject m_statsParentP1;
	[SerializeField] private TextMeshProUGUI m_timerTextP1;
	[SerializeField] private Slider m_timerSliderP1;
	[SerializeField] private TextMeshProUGUI m_livesCountP1;
	[SerializeField] private TextMeshProUGUI m_countStatP1;

	[Space]
	[Header("Stats (multiplayer)")]
	[SerializeField] private GameObject m_statsParentP2;                    // [TODO][Q] Is this the best way to do this?
	[SerializeField] private TextMeshProUGUI m_timerTextP2;
	[SerializeField] private Slider m_timerSliderP2;
	[SerializeField] private TextMeshProUGUI m_livesCountP2;
	[SerializeField] private TextMeshProUGUI m_countStatP2;

	[Space]
	[Header("Input Buttons")]
	[SerializeField] private GameObject m_inputButtonsParentP1;
	[SerializeField] private GameObject m_inputButtonsParentP2;
	[SerializeField] private SettingsData_Base m_inputSettingsData;
	[SerializeField] private Button m_buttonStopTravelP1;

	[Space]
	[Header("Pause")]
	[SerializeField] private Button m_pauseButton;
	[SerializeField] private Button m_resumeButton;                         // [TODO][Q] Do we want this in its own PauseManager?
	[SerializeField] private GameObject m_pauseParentGameplay;
	[SerializeField] private GameObject m_pauseParentEditor;

	[Space]
	[Header("EndScreen MASTER Parent")]
	[SerializeField] private GameObject m_endScreenParent;
	[SerializeField] private TextMeshProUGUI m_endLevelTitle;
	[SerializeField] private TextMeshProUGUI m_endWinLoseTitle;
	[SerializeField] private TextMeshProUGUI m_endTimer;
	[SerializeField] private Button m_nextLevelButton;

	[Space]
	[Header("End Game (single player)")]
	[SerializeField] private GameObject m_endStatsParentSingle;
	[SerializeField] private TextMeshProUGUI m_endMovesCount;
	[SerializeField] private TextMeshProUGUI m_endLivesCount;
	[SerializeField] private TextMeshProUGUI m_endExtraInfoStat;
	[SerializeField] private TextMeshProUGUI m_endTotalScore;

	[Space]
	[Header("End Game (multiplayer)")]
	// [TODO] REFACTOR! Try not duplicate two sets of P1 stats. Just move the parent into the correct position!
	// [TODO] Are we having a different stats section layout for multiplayer? Would make sense, I think. But then would need to duplicate P1 stats... Unless we find a way to do e.g. m_statsString = $"P1 Items: {m_itemsP1} ... P2 Items: {m_itemsP2}"
	[SerializeField] private GameObject m_endStatsParentMulti;
	[SerializeField] private TextMeshProUGUI m_endMovesCountP1;
	[SerializeField] private TextMeshProUGUI m_endLivesCountP1;
	[SerializeField] private TextMeshProUGUI m_endExtraInfoStatP1;
	[SerializeField] private TextMeshProUGUI m_endTotalScoreP1;
	[SerializeField] private TextMeshProUGUI m_endMovesCountP2;
	[SerializeField] private TextMeshProUGUI m_endLivesCountP2;
	[SerializeField] private TextMeshProUGUI m_endExtraInfoStatP2;
	[SerializeField] private TextMeshProUGUI m_endTotalScoreP2;
	[SerializeField] private TextMeshProUGUI m_headToHeadScoreP1;
	[SerializeField] private TextMeshProUGUI m_headToHeadScoreP2;
	[SerializeField] private TextMeshProUGUI m_headToHeadScoreDraw;

	// [TODO][Q] Can we just use single player end game and change text and functionality of return to main menu button?
	[Space]
	[Header("End Game (level editor)")]
	[SerializeField] private GameObject m_endStatsParentEditor;

	public int MovesChaser = 0;
	public int MovesTarget = 0;
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
		if (LevelSelectData.GameMode == EGameMode.Travel && LevelSelectData.IsMultiplayer == false)
		{
			m_buttonStopTravelP1.gameObject.SetActive(true);
		}
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
			m_endStatsParentEditor.SetActive(true);
		else if (LevelSelectData.IsMultiplayer)
			m_endStatsParentMulti.SetActive(true);
		else
			m_endStatsParentSingle.SetActive(true);
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


	public void AssignTravelButtonP1(UnityAction onTravelStopped)
	{
		m_buttonStopTravelP1.onClick.AddListener(onTravelStopped);
	}


	public void AssignLevelEditorResumeEndLevel(UnityAction onResume)
	{
		m_nextLevelButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
		m_nextLevelButton.onClick.AddListener(LevelEditorContinue);
		m_nextLevelButton.onClick.AddListener(onResume);
		m_nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue?";
	}

	public void AssignMChaseNextRound(UnityAction onResume)
	{
		m_nextLevelButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
		m_nextLevelButton.onClick.AddListener(RetryLevel);
		m_nextLevelButton.onClick.AddListener(onResume);
		m_nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next Round";
	}


	public void NextLevel()
	{
		LevelSelectData.ChosenMapIndex++;
		LevelSelectData.MapData = LevelSelectData.ThemeData.Maps[LevelSelectData.ChosenMapIndex];
		SettingsSystem.UpdateSettings(m_mapIndexSettingsData.Key, $"{LevelSelectData.ChosenMapIndex}");
		// NOTE we want to autosave the map index
		SettingsSystem.SaveSettings();
		RetryLevel();
	}

	public void RetryLevel()
	{
		SceneManager.LoadScene("LevelScene");
	}

	private void LevelEditorContinue()
	{
		m_endScreenParent.SetActive(false);
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
	public void SetTimerSliderActiveP1(bool active) { m_timerSliderP1.gameObject.SetActive(active); }
	public void SetTimerSliderActiveP2(bool active) { m_timerSliderP2.gameObject.SetActive(active); }

	public void SetCountStatActiveP1(bool active) { m_countStatP1.gameObject.SetActive(active); }
	public void SetCountStatActiveP2(bool active) { m_countStatP2.gameObject.SetActive(active); }

	public void SetMultiplayerStatsActive() { m_statsParentP2.SetActive(true); }
	#endregion



	#region GameplayManager Calls (updates)
	public void UpdateLivesCountP1(int lives) { m_livesCountP1.text = $"Lives: {lives}"; }
	public void UpdateLivesCountP2(int lives) { m_livesCountP2.text = $"Lives: {lives}"; }

	public void UpdateItemsCountP1(int items) { m_countStatP1.text = $"Items: {items}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}"; }
	public void UpdateItemsCountP2(int items) { m_countStatP2.text = $"Items: {items}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}"; }

	public void UpdateTravelSquaresPercentP1(float squaresPercent) { m_countStatP1.text = $"Squares: {squaresPercent}%"; }
	public void UpdateTravelSquaresPercentP2(float squaresPercent) { m_countStatP2.text = $"Squares: {squaresPercent}%"; }

	public void InitChaseMovesP1(bool isPlayerTurn)
	{
		int totalMoves = (LevelSelectData.ChaseIsRoundTwo) ? MovesTarget : MovesChaser;
		int initMoves = (isPlayerTurn) ? totalMoves : 0;
		m_countStatP1.text = $"Moves: {initMoves}/{totalMoves}";
	}
	public void InitChaseMovesP2(bool isPlayerTurn)
	{
		int totalMoves = (LevelSelectData.ChaseIsRoundTwo) ? MovesChaser : MovesTarget;
		int initMoves = (isPlayerTurn) ? totalMoves : 0;
		m_countStatP2.text = $"Moves: {initMoves}/{totalMoves}";
	}
	public void UpdateChaseMovesP1(int movesLeft)
	{
		int totalMoves = (LevelSelectData.ChaseIsRoundTwo) ? MovesTarget : MovesChaser;
		m_countStatP1.text = $"Moves: {movesLeft}/{totalMoves}";
	}
	public void UpdateChaseMovesP2(int movesLeft)
	{
		int totalMoves = (LevelSelectData.ChaseIsRoundTwo) ? MovesChaser : MovesTarget;
		m_countStatP2.text = $"Moves: {movesLeft}/{totalMoves}";
	}

	public void UpdateTimerTextCountUpP1(int timeTaken) { m_timerTextP1.text = $"Time Taken: {timeTaken}s"; }
	public void UpdateTimerTextCountUpP2(int timeTaken) { m_timerTextP2.text = $"Time Taken: {timeTaken}s"; }

	public void UpdateTimerTextCountDownP1(float timeLeft) { m_timerTextP1.text = $"Time Left: {timeLeft}s"; }
	public void UpdateTimerTextCountDownP2(float timeLeft) { m_timerTextP2.text = $"Time Left: {timeLeft}s"; }

	public void UpdateTimerSliderP1(float timeLeft) { m_timerSliderP1.value = 1 - (timeLeft / LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit); }
	public void UpdateTimerSliderP2(float timeLeft) { m_timerSliderP2.value = 1 - (timeLeft / LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit); }
	#endregion



	#region GameplayManager Calls (end screen)
	public void SetEndScreenStatsSingle(int totalScore, float timeTaken, int movesTaken, int livesLeft, float extraInfo = 0)
	{
		m_endTotalScore.text = $"Total Score: {totalScore:n0}";
		m_endTimer.text = $"Time Taken: {timeTaken}s";
		m_endMovesCount.text = $"Moves Taken: {movesTaken}";
		m_endLivesCount.text = $"Lives Left: {livesLeft}";
		if (LevelSelectData.GameMode == EGameMode.Items)
			m_endExtraInfoStat.text = $"Items Collected: {extraInfo}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
		else if (LevelSelectData.GameMode == EGameMode.Travel)
			m_endExtraInfoStat.text = $"Area Covered: {extraInfo}%";
		else
			m_endExtraInfoStat.gameObject.SetActive(false);
	}

	public void SetWinLoseTitle(bool isWin)
	{
		m_nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = (isWin) ? "Next Level" : "Skip Level?";
		m_endWinLoseTitle.text = (isWin) ? "Yay! You Win!" : "Uh-oh! You lost!";
	}


	public void SetEndScreenStatsMultiP1(int totalScore, int movesTaken, int livesLeft, float extraInfo = 0)
	{
		m_endTotalScoreP1.text = $"P1: {totalScore:n0}";
		m_endMovesCountP1.text = $"Moves Taken: {movesTaken}";
		m_endLivesCountP1.text = $"Lives Left: {livesLeft}";
		if (LevelSelectData.GameMode == EGameMode.Items)
			m_endExtraInfoStatP1.text = $"Items Collected: {extraInfo}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
		else if (LevelSelectData.GameMode == EGameMode.Travel)
			m_endExtraInfoStatP1.text = $"Area Covered: {extraInfo}%";
		else
			m_endExtraInfoStatP1.gameObject.SetActive(false);
	}

	public void SetEndScreenStatsMultiP2(int totalScore, int movesTaken, int livesLeft, float extraInfo = 0)
	{
		m_endTotalScoreP2.text = $"P2: {totalScore:n0}";
		m_endMovesCountP2.text = $"Moves Taken: {movesTaken}";
		m_endLivesCountP2.text = $"Lives Left: {livesLeft}";
		if (LevelSelectData.GameMode == EGameMode.Items)
			m_endExtraInfoStatP2.text = $"Items Collected: {extraInfo}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
		else if (LevelSelectData.GameMode == EGameMode.Travel)
			m_endExtraInfoStatP2.text = $"Area Covered: {extraInfo}%";
		else
			m_endExtraInfoStatP2.gameObject.SetActive(false);
	}

	public void SetWinLoseTitleMulti(EMultiplayerResult result)
	{
		switch (result)
		{
			case EMultiplayerResult.P1:		m_endWinLoseTitle.text = "P1 WINS!";	break;
			case EMultiplayerResult.P2:		m_endWinLoseTitle.text = "P2 WINS!";	break;
			case EMultiplayerResult.Draw:	m_endWinLoseTitle.text = "DRAW!";		break;
			default:																break;
		}

		// [TODO] Animate adding to the score!
		m_headToHeadScoreP1.text = $"P1\n{PlayerPrefsSystem.MultiplayerGetWinsP1()}";
		m_headToHeadScoreP2.text = $"P2\n{PlayerPrefsSystem.MultiplayerGetWinsP2()}";
		m_headToHeadScoreDraw.text = $"Draws\n{PlayerPrefsSystem.MultiplayerGetDraws()}";
	}


	public void ShowEndScreen()
	{
		if (LevelEditorData.IsTestingLevel == false && LevelSelectData.MapData == LevelSelectData.ThemeData.Maps[LevelSelectData.ThemeData.Maps.Length - 1])
			m_nextLevelButton.gameObject.SetActive(false);
		m_endScreenParent.SetActive(true);
	}
	#endregion
}
