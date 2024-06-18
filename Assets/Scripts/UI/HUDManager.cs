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
	[Header("Stats (single player)")]
	[SerializeField] private GameObject m_statsParent;
	[SerializeField] private GameObject m_statsParentP2;					// [TODO][Q] Is this the best way to do this?
	[SerializeField] private TextMeshProUGUI m_timerText;
	[SerializeField] private Slider m_timerSlider;
	[SerializeField] private TextMeshProUGUI m_livesCount;
	[SerializeField] private TextMeshProUGUI m_itemsCount;
	
	[Space]
	[Header("Pause")]
	[SerializeField] private Button m_pauseButton;
	[SerializeField] private Button m_resumeButton;                         // [TODO][Q] Do we want this in its own PauseManager?

	[Space]
	[Header("End Game (single player)")]
	[SerializeField] private GameObject m_endScreenParent;
	[SerializeField] private TextMeshProUGUI m_levelTitle;
	[SerializeField] private TextMeshProUGUI m_winLoseTitle;
	[SerializeField] private TextMeshProUGUI m_endTimer;
	[SerializeField] private TextMeshProUGUI m_endMovesCount;
	[SerializeField] private TextMeshProUGUI m_endLivesCount;
	[SerializeField] private TextMeshProUGUI m_endItemsCount;
	[SerializeField] private TextMeshProUGUI m_endTotalScore;
	[SerializeField] private Button m_nextLevelButton;
	#endregion


	private void Awake()
	{
		// [TODO][IMPORTANT] This wiil also change for multiplayer! Calculate!	<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		
		// [Q] Is this a correct formula??? Or good enough???
		m_statsParent.transform.localPosition = new Vector3(0, -(Camera.main.aspect * Camera.main.aspect * Screen.width), 0);

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
		m_levelTitle.text = $"{LevelSelectData.ThemeData.ThemeName} : {currentLevelIndex}";
	}


	public void SetTimerSliderActive(bool active)
	{
		m_timerSlider.gameObject.SetActive(active);
	}

	public void SetItemsCountActive(bool active)
	{
		m_itemsCount.gameObject.SetActive(active);
	}



	public void UpdateLivesCount(int lives)
	{
		m_livesCount.text = $"Lives: {lives}";
	}

	public void UpdateItemsCount(int items)
	{
		m_itemsCount.text = $"Items: {items}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
	}

	public void UpdateTimerTextExit(int timeTaken)
	{
		m_timerText.text = $"Time Taken: {timeTaken}s";
	}

	public void UpdateTimerTextItems(float timeLeft)
	{
		m_timerText.text = $"Time Left: {timeLeft}s";
	}

	public void UpdateTimerSlider(float timeLeft)
	{
		m_timerSlider.value = 1 - (timeLeft / LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit);
	}



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
		Debug.Log("[GameplayManager::NextLevel]");
		LevelSelectData.MapData = LevelSelectData.ThemeData.Maps[LevelSelectData.ChosenMapIndex + 1];
		RetryLevel();
	}

	public void RetryLevel()
	{
		Debug.Log("[GameplayManager::RetryLevel]");
		SceneManager.LoadScene("LevelScene");                           // [Q] Is this the best way to do it? Probably
	}

	// [TODO] Turn into an actual 'exit' button, in the pause menu.
	// [TODO] Ask the player if they're sure they want to quit!
	// [TODO] Implement via EndGame(EGameEndState.Quit) ???
	//			Or can just delete that EGameEndState?
	// Used by both pause menu and the end screen... Only want to show 'are you sure' popup if we're in the pause menu
	public void ReturnToMainMenu()
	{
		Debug.Log("[GameplayManager::ReturnToMainMenu]");
		SceneManager.LoadScene("MainMenu");
	}

	public void ReturnToLevelEditor()
	{
		Debug.Log("[GameplayManager::ReturnToMainMenu]");
		SceneManager.LoadScene("LevelEditor");
	}
	#endregion



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
		m_winLoseTitle.text = (isWin) ? "Yay! You Win!" : "Uh-oh! You lost!";
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
		m_endScreenParent.SetActive(true);
	}
}
