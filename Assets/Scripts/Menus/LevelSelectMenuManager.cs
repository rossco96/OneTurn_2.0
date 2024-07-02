using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// [TODO] Refactor entire class into subclasses

public class LevelSelectMenuManager : MonoBehaviour
{
	[SerializeField] private ThemesList m_themesList;

	[Space]
	[SerializeField] private GridLayoutGroup m_mapTabsParentGridLayoutGroup;
	[SerializeField] private RectTransform m_mapTabsParentRectTransform;
	[SerializeField] private Button m_buttonGameTab;
	[SerializeField] private Button m_buttonCustomTab;
	[SerializeField] private Button m_buttonImportedTab;

	[Space]
	[SerializeField] private Image m_themeIconImage;
	
	[SerializeField] private Button m_buttonThemeUp;
	[SerializeField] private Button m_buttonThemeDown;
	[SerializeField] private Button m_buttonMapUp;
	[SerializeField] private Button m_buttonMapDown;

	[Space]
	[SerializeField] private TMP_Dropdown m_gameModeDropdown;

	[Space]
	[Header("Single Player Stats")]
	[SerializeField] private GameObject m_statsParentSinglePlayer;
	[SerializeField] private TextMeshProUGUI m_statTime;
	[SerializeField] private TextMeshProUGUI m_statMoves;
	[SerializeField] private TextMeshProUGUI m_statLives;
	[SerializeField] private TextMeshProUGUI m_statItems;
	[SerializeField] private TextMeshProUGUI m_statScore;
	[SerializeField] private TextMeshProUGUI m_statTotalPoints;

	[Space]
	[Header("Multiplayer Stats")]
	[SerializeField] private GameObject m_statsParentMultiplayer;

	private ThemeData m_currentTheme;

	private int m_numberOfThemes;
	private int m_themeIndex;
	private int m_numberOfMaps;
	private int m_mapIndex;


	
	// [TODO] Refactor OnEnable()
	private void OnEnable()
	{
		m_numberOfThemes = m_themesList.ThemesData.Length;

		// [TODO] Ensure we're reading from the most recently played level!
		// If no level exists (i.e. a new download) then default to first level -- Same with m_mapIndex
		// --> Make sure to also show tutorial hints if that is the case!

		// [TODO] Temp here!!!
		m_themeIndex = 0;
		m_mapIndex = 0;
		m_currentTheme = m_themesList.ThemesData[0];
		m_numberOfMaps = m_currentTheme.Maps.Length;
		LevelSelectData.ThemeData = m_currentTheme;
		LevelSelectData.SetMapData(m_currentTheme.Maps[0]);
		LevelSelectData.ChosenMapIndex = 0;
		LevelSelectData.FileName = $"{Hash128.Compute(m_currentTheme.Maps[0].GridLayout.EncodeToPNG())}";
		LevelSelectData.MapType = EMapType.Game;

		m_buttonThemeUp.interactable = false;
		m_buttonMapUp.interactable = false;
		// ^^^^^ ^^^^^ ^^^^^

		//m_currentTheme = 
		//m_themeIndex = 
		//m_mapIndex = 

		if (LevelSelectData.IsMultiplayer == false)
			UpdateLevelStatsSinglePlayer();
		else
			UpdateStatsMultiplayer();

		m_statTotalPoints.text = $"Total Points: {SaveSystem.GetTotalPoints(m_themesList):n0}";
		// [TODO] Figure out if we're displaying the total points - only if it was a GameMap that was played previously! Similarly, also set the correct tab.
		
		m_statsParentSinglePlayer.SetActive(LevelSelectData.IsMultiplayer == false);
		m_statsParentMultiplayer.SetActive(LevelSelectData.IsMultiplayer);

		// [Q] Keep these here?
		LevelSelectData.IsInGame = true;
		LevelSelectData.IsMultiplayer = false;
		LevelEditorData.IsTestingLevel = false;

		// [NOTE] THIS IS SUPER HACKY BUT IT WORKS
		StartCoroutine(SetTabsParent());
	}


	// (ensures we're resizing the grid layout group correctly)
	private System.Collections.IEnumerator SetTabsParent()
	{
		yield return null;
		// 3 is the number of tabs. 80.0f is the pre-defined height
		m_mapTabsParentGridLayoutGroup.cellSize = new Vector2(m_mapTabsParentRectTransform.rect.width / 3, 80.0f);
	}



	public void SelectGameTab()
	{
		LevelSelectData.MapType = EMapType.Game;

		m_statTotalPoints.gameObject.SetActive(true);
	}

	public void SelectCustomTab()
	{
		LevelSelectData.MapType = EMapType.Custom;

		// [TODO][Q] For stats on custom levels. Do we force reset upon any editing?
		// Otherwise could create a straight line of items for max points and then turn it into a really hard level
		// ... Does it matter?

		m_statTotalPoints.gameObject.SetActive(false);
	}

	public void SelectImportedTab()
	{
		LevelSelectData.MapType = EMapType.Imported;

		m_statTotalPoints.gameObject.SetActive(false);
	}



	public void UpdateThemeIndex(int indexDirection)
	{
		m_themeIndex += indexDirection;

		m_currentTheme = m_themesList.ThemesData[m_themeIndex];
		m_themeIconImage.sprite = m_currentTheme.LevelSelectIcon;
		m_numberOfMaps = m_currentTheme.Maps.Length;
		
		m_mapIndex = 0;
		// [TODO] Show "1", or whatever we're using to represent the first level
		m_buttonMapUp.interactable = false;
		m_buttonMapDown.interactable = true;

		LevelSelectData.ThemeData = m_currentTheme;
		LevelSelectData.SetMapData(m_currentTheme.Maps[0]);
		LevelSelectData.ChosenMapIndex = 0;
		LevelSelectData.FileName = $"{Hash128.Compute(m_currentTheme.Maps[0].GridLayout.EncodeToPNG())}";

		if (LevelSelectData.IsMultiplayer == false)
			UpdateLevelStatsSinglePlayer();

		if (m_themeIndex == 0)
			m_buttonThemeUp.interactable = false;
		else if (m_themeIndex == 1 && m_buttonThemeUp.interactable == false)
			m_buttonThemeUp.interactable = true;
		
		if (m_themeIndex == m_numberOfThemes - 1)
			m_buttonThemeDown.interactable = false;
		else if (m_themeIndex == m_numberOfThemes - 2 && m_buttonThemeDown.interactable == false)
			m_buttonThemeDown.interactable = true;
	}

	public void UpdateMapIndex(int indexDirection)
	{
		if (indexDirection != -1 && indexDirection != 1)
		{
			Debug.LogError("[SinglePlayerMenuManager::UpdateMapIndex] Invalid indexDirection. Must only be -1 or 1.");
			return;
		}
		m_mapIndex += indexDirection;
		// [TODO] Show "1", or whatever we're using to represent the first level
		LevelSelectData.SetMapData(m_currentTheme.Maps[m_mapIndex]);
		LevelSelectData.ChosenMapIndex = m_mapIndex;
		LevelSelectData.FileName = $"{Hash128.Compute(m_currentTheme.Maps[m_mapIndex].GridLayout.EncodeToPNG())}";

		if (LevelSelectData.IsMultiplayer == false)
			UpdateLevelStatsSinglePlayer();

		if (m_mapIndex == 0)
			m_buttonMapUp.interactable = false;
		else if (m_mapIndex == 1 && m_buttonMapUp.interactable == false)
			m_buttonMapUp.interactable = true;
		
		if (m_mapIndex == m_numberOfMaps - 1)
			m_buttonMapDown.interactable = false;
		else if (m_mapIndex == m_numberOfMaps - 2 && m_buttonMapDown.interactable == false)
			m_buttonMapDown.interactable = true;
	}

	private void UpdateLevelStatsSinglePlayer()
	{
		m_statTime.text = $"Time Taken: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Time)}s";
		m_statMoves.text = $"Moves Taken: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Moves)}";
		m_statLives.text = $"Lives Left: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Lives)}";	// [TODO][Q] Do we want time left, quickest time, or different depending on items or exit mode?
		if (LevelSelectData.GameMode == EGameMode.Items)
			m_statItems.text = $"Items Collected: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Items)}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
		m_statScore.text = $"Level Score: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Score):n0}";
	}

	private void UpdateStatsMultiplayer()
	{
		// [TODO]
	}



	public void UpdateMultiplayerToggle(Toggle isMultiplayerToggle)
	{
		// [TODO]
		// Is there a better way of retaining the data???
		// [NOTE]
		// Currently doing like this since we're clearing the list each time, but because EXIT is in both modes,
		// want to retain that data if switching between single player and multiplayer (and if ITEMS selected then no updating is required)

		int gameModeValue = -1;
		if (m_gameModeDropdown.value > 1)
			m_gameModeDropdown.value = 0;
		else if (m_gameModeDropdown.value == 1)
			gameModeValue = 1;

		LevelSelectData.IsMultiplayer = isMultiplayerToggle.isOn;

		m_gameModeDropdown.ClearOptions();
		System.Collections.Generic.List<string> gameModes = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EGameMode)).Length; ++i)
		{
			string gameMode = $"{(EGameMode)i}";
			if (gameMode.StartsWith("M_"))
			{
				if (isMultiplayerToggle.isOn == false)
					continue;
				else
					gameMode = gameMode.Substring(2);
			}
			gameModes.Add(gameMode);
		}
		m_gameModeDropdown.AddOptions(gameModes);
		
		if (gameModeValue == 1)
			m_gameModeDropdown.value = 1;

		m_statsParentSinglePlayer.SetActive(LevelSelectData.IsMultiplayer == false);
		m_statsParentMultiplayer.SetActive(LevelSelectData.IsMultiplayer);
	}

	public void UpdateGameMode(TMP_Dropdown dropdown)
	{
		EGameMode gameMode = (EGameMode)dropdown.value;
		LevelSelectData.GameMode = gameMode;
		if (LevelSelectData.IsMultiplayer == false)
		{
			m_statItems.gameObject.SetActive(gameMode == EGameMode.Items);
			UpdateLevelStatsSinglePlayer();
		}
	}

	public void UpdateTurnDirection(TMP_Dropdown dropdown)
	{
		ETurnDirection turnDirection = (ETurnDirection)dropdown.value;
		LevelSelectData.TurnDirection = turnDirection;
		if (LevelSelectData.IsMultiplayer == false)
			UpdateLevelStatsSinglePlayer();
	}



	public void LoadLevel()
	{
		// [TODO]
		//	o Get ThemeData and then the specific MapData from the index chosen
		//	o Pass ThemeData and MapData to *STATIC* LevelGenerator
		//	o Using GameplayManager, within the level itself, retrieve the data
		//	o Generate the level

		// [NOTE] No longer implementing as below!

		//if (m_isMultiplayer)
		//	SceneManager.LoadScene("LevelScene_Multiplayer");
		//else
			SceneManager.LoadScene("LevelScene");
	}
}
