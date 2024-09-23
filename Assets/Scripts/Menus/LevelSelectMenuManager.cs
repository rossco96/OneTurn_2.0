using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// [TODO] Refactor entire class into subclasses

public class LevelSelectMenuManager : MonoBehaviour
{
	[SerializeField] private ThemesList m_gamesThemesList;
	[SerializeField] private ThemesList m_levelEditorThemes;                        // Used by both m_customThemesList and m_importedThemesList

	[Space]
	[SerializeField] private TutorialPopup m_tutorialPopupLevelSelect;
	[SerializeField] private TutorialPopup m_tutorialPopupMultiplayer;

	[Space]
	[SerializeField] private SettingsDataString m_mapTypeSettingsData;
	[SerializeField] private SettingsDataString m_themeSettingsData;
	[SerializeField] private SettingsDataInt m_mapIndexSettingsData;
	[SerializeField] private SettingsDataString m_gameModeSettingsData;
	[SerializeField] private SettingsDataString m_levelDirectionSettingsData;

	[Space]
	[SerializeField] private GridLayoutGroup m_mapTabsParentGridLayoutGroup;

	[Space]
	[SerializeField] private TextMeshProUGUI m_themeHeading;						// (GameMap = ThemeName, Custom and Imported = MapName)
	[SerializeField] private Image m_themeIconImage;
	[SerializeField] private TextMeshProUGUI m_customImportedThemeText;
	[SerializeField] private TextMeshProUGUI m_mapIndexText;
	[SerializeField] private Button m_gameMapsTab;
	[SerializeField] private Button m_customMapsTab;
	[SerializeField] private Button m_importedMapsTab;
	
	[SerializeField] private Button m_buttonThemeUp;
	[SerializeField] private Button m_buttonThemeDown;
	[SerializeField] private Button m_buttonMapUp;
	[SerializeField] private Button m_buttonMapDown;

	[Space]
	[SerializeField] private Toggle m_multiplayerToggle;
	[SerializeField] private TMP_Dropdown m_gameModeDropdown;

	[Space]
	[SerializeField] private Button m_playButton;
	[SerializeField] private TextMeshProUGUI m_lockedInfo;

	[Space]
	[Header("Single Player Stats")]
	[SerializeField] private GameObject m_statsParentSinglePlayer;
	[SerializeField] private TextMeshProUGUI m_statTime;
	[SerializeField] private TextMeshProUGUI m_statMoves;
	[SerializeField] private TextMeshProUGUI m_statLives;
	[SerializeField] private TextMeshProUGUI m_statExtraInfo;
	[SerializeField] private TextMeshProUGUI m_statScore;
	[SerializeField] private TextMeshProUGUI m_statTotalPoints;

	[Space]
	[Header("Multiplayer Stats")]
	[SerializeField] private GameObject m_statsParentMultiplayer;
	[SerializeField] private TextMeshProUGUI m_multiStatsWinsP1;
	[SerializeField] private TextMeshProUGUI m_multiStatsDraws;
	[SerializeField] private TextMeshProUGUI m_multiStatsWinsP2;
	[SerializeField] private TextMeshProUGUI m_multiStatsScoreP1;
	[SerializeField] private TextMeshProUGUI m_multiStatsScoreP2;

	private ThemesList m_currentThemesList;
	private ThemesList m_customThemesList;
	private ThemesList m_importedThemesList;												// IMPLEMENT!

	private ThemeData m_currentTheme;

	private int m_themeIndex = 0;
	private int m_mapIndex = 0;
	private int m_totalPoints = 0;


	
	// [TODO] Refactor OnEnable()
	private void OnEnable()
	{
		// [TODO] Ensure we're reading from the most recently played level!
		// If no level exists (i.e. a new download) then default to first level -- Same with m_mapIndex
		// --> Make sure to also show tutorial hints if that is the case!

		m_totalPoints = SaveSystem.GetTotalPoints(m_gamesThemesList);
		m_statTotalPoints.text = $"Total Points: {m_totalPoints:n0}";

		InitCustomImportedThemesLists();
		InitSelection();

		LevelSelectData.IsInGame = true;
		LevelEditorData.IsTestingLevel = false;

		// [NOTE] THIS IS SUPER HACKY BUT IT WORKS
		StartCoroutine(SetTabsParent());

		m_tutorialPopupLevelSelect.TryShow();
	}


	// SO HACKY :(
	// (ensures we're resizing the grid layout group correctly)
	private System.Collections.IEnumerator SetTabsParent()
	{
		yield return null;
		// 3 is the number of tabs
		int cellWidth = Mathf.FloorToInt(m_mapTabsParentGridLayoutGroup.GetComponent<RectTransform>().rect.width / 3);
		m_mapTabsParentGridLayoutGroup.cellSize = new Vector2(cellWidth, m_mapTabsParentGridLayoutGroup.cellSize.y);
	}



	public void SelectGameTab()
	{
		m_gameMapsTab.targetGraphic.color = Color.yellow;
		m_customMapsTab.targetGraphic.color = m_customMapsTab.colors.normalColor;
		m_importedMapsTab.targetGraphic.color = m_importedMapsTab.colors.normalColor;

		LevelSelectData.MapType = EMapType.Game;
		m_statTotalPoints.gameObject.SetActive(true);
		m_themeIconImage.gameObject.SetActive(true);
		m_customImportedThemeText.gameObject.SetActive(false);

		m_currentThemesList = m_gamesThemesList;
		m_themeIndex = 0;
		UpdateThemeIndex(0);
	}

	public void SelectCustomTab()
	{
		m_gameMapsTab.targetGraphic.color = m_gameMapsTab.colors.normalColor;
		m_customMapsTab.targetGraphic.color = Color.yellow;
		m_importedMapsTab.targetGraphic.color = m_importedMapsTab.colors.normalColor;

		LevelSelectData.MapType = EMapType.Custom;
		m_statTotalPoints.gameObject.SetActive(false);
		m_themeIconImage.gameObject.SetActive(false);
		m_customImportedThemeText.gameObject.SetActive(true);
		
		m_lockedInfo.gameObject.SetActive(false);

		// [TODO][Q] For stats on custom levels. Do we force reset upon any editing?
		// Otherwise could create a straight line of items for max points and then turn it into a really hard level
		// ... Does it matter?

		m_currentThemesList = m_customThemesList;
		m_themeIndex = 0;
		UpdateThemeIndex(0);
	}

	public void SelectImportedTab()
	{
		m_gameMapsTab.targetGraphic.color = m_gameMapsTab.colors.normalColor;
		m_customMapsTab.targetGraphic.color = m_customMapsTab.colors.normalColor;
		m_importedMapsTab.targetGraphic.color = Color.yellow;

		LevelSelectData.MapType = EMapType.Imported;
		m_statTotalPoints.gameObject.SetActive(false);
		m_themeIconImage.gameObject.SetActive(false);
		m_customImportedThemeText.gameObject.SetActive(true);

		m_lockedInfo.gameObject.SetActive(false);

		m_currentThemesList = m_importedThemesList;
		m_themeIndex = 0;
		UpdateThemeIndex(0);
	}



	private void InitCustomImportedThemesLists()
	{
		// CUSTOM
		string[] fileNames = SaveSystem.GetCustomMapFileNamesByCreationTime();
		System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string>> customMapsDictionary = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string>>();

		for (int i = 0; i < fileNames.Length; ++i)
		{
			string fileName = fileNames[i];
			int gridSize = int.Parse(SaveSystem.GetMapmetaInfo(fileName, EMapmetaInfo.GridDimension));
			if (customMapsDictionary.ContainsKey(gridSize))
			{
				System.Collections.Generic.List<string> currentCustomGrids = customMapsDictionary[gridSize];
				currentCustomGrids.Add(fileName);
				customMapsDictionary[gridSize] = currentCustomGrids;
			}
			else
			{
				customMapsDictionary.Add(gridSize, new System.Collections.Generic.List<string>() { fileName });
			}
		}

		m_customThemesList = (ThemesList)ScriptableObject.CreateInstance(typeof(ThemesList));
		m_customThemesList.ThemesData = new ThemeData[0];

		var dictionaryEnumerator = customMapsDictionary.GetEnumerator();
		while (dictionaryEnumerator.MoveNext())
		{
			int gridDimension = dictionaryEnumerator.Current.Key;

			ThemeData themeData = (ThemeData)ScriptableObject.CreateInstance(typeof(ThemeData));
			themeData.ThemeName = $"{gridDimension}x{gridDimension}";
			
			// [Q][IMPORTANT] Is this Rect set up enough? Need to set actual params?
			// Also want pic of e.g. "9x9" pls?																		// CURRENTLY JUST USING TEXT!
			//themeData.LevelSelectIcon = Sprite.Create(null/*levelTexture*/, new Rect(), 0.5f * Vector2.one);		// CHANGE!
			
			themeData.Maps = new MapData[0];

			LevelPlayInfo levelPlayInfo = (LevelPlayInfo)ScriptableObject.CreateInstance(typeof(LevelPlayInfo));
			levelPlayInfo.GridDimension = gridDimension;
			levelPlayInfo.ItemTimeLimit = LevelEditorTools.GetTimeLimitFromGridDimension(gridDimension);
			levelPlayInfo.TotalItems = LevelEditorTools.GetItemsFromGridDimension(gridDimension);
			themeData.LevelPlayInfo = levelPlayInfo;

			System.Collections.Generic.List<string> currentGridSizeMaps = customMapsDictionary[gridDimension];
			for (int i = 0; i < currentGridSizeMaps.Count; ++i)
			{
				// [TODO] Create method inside SaveSystem which has "out MapData" param which just does all the below.

				MapData mapData = (MapData)ScriptableObject.CreateInstance(typeof(MapData));
				mapData.GridLayout = SaveSystem.GetCustomMapTexture(currentGridSizeMaps[i]);
				mapData.CustomImportedMapFileName = currentGridSizeMaps[i];
				mapData.MapName = SaveSystem.GetMapmetaInfo(currentGridSizeMaps[i], EMapmetaInfo.MapName);

				mapData.ExitFacingDirectionRight = SaveSystem.GetCustomMapFacingInfo(currentGridSizeMaps[i], EMapPropertyName.Exit, ETurnDirection.Right);
				mapData.ExitFacingDirectionLeft = SaveSystem.GetCustomMapFacingInfo(currentGridSizeMaps[i], EMapPropertyName.Exit, ETurnDirection.Left);

				mapData.PlayerSpawnDirectionRight = new EFacingDirection[2];
				mapData.PlayerSpawnDirectionLeft = new EFacingDirection[2];

				mapData.PlayerSpawnDirectionRight[0] = SaveSystem.GetCustomMapFacingInfo(currentGridSizeMaps[i], EMapPropertyName.SpawnPointPrimary, ETurnDirection.Right);
				mapData.PlayerSpawnDirectionLeft[0] = SaveSystem.GetCustomMapFacingInfo(currentGridSizeMaps[i], EMapPropertyName.SpawnPointPrimary, ETurnDirection.Left);
				mapData.PlayerSpawnDirectionRight[1] = SaveSystem.GetCustomMapFacingInfo(currentGridSizeMaps[i], EMapPropertyName.SpawnPointSecondary, ETurnDirection.Right);
				mapData.PlayerSpawnDirectionLeft[1] = SaveSystem.GetCustomMapFacingInfo(currentGridSizeMaps[i], EMapPropertyName.SpawnPointSecondary, ETurnDirection.Left);

				themeData.Maps = themeData.Maps.Add(mapData);

				// [TODO][IMPORTANT]
				// Need to implement the below!
				// Not currently saving the theme type inside .mapmeta file!
				
				/*
				for (int j = 0; j < m_levelEditorThemes.ThemesData.Length; ++j)
				{
					if (m_levelEditorThemes.ThemesData[j].ThemeName == SaveSystem.GetMapmetaInfo(currentGridSizeMaps[i], EMapmetaInfo.Theme))
					{
						themeData.BackgroundSprite = m_levelEditorThemes.ThemesData[j].BackgroundSprite;
						themeData.ExitSprite = m_levelEditorThemes.ThemesData[j].ExitSprite;
						themeData.ItemSprite = m_levelEditorThemes.ThemesData[j].ItemSprite;
						themeData.PlayerSprite = m_levelEditorThemes.ThemesData[j].PlayerSprite;
						themeData.WallSprite = m_levelEditorThemes.ThemesData[j].WallSprite;
						break;
					}
				}
				//*/
			}

			m_customThemesList.ThemesData = m_customThemesList.ThemesData.Add(themeData);
		}

		m_customThemesList.ThemesData = m_customThemesList.ThemesData.GetOrderedCustomThemeData();

		m_customMapsTab.interactable = (customMapsDictionary.Count > 0);

		//
		// IMPORTED
		//

		//m_importedMapsTab.interactable = (importedMapsDictionary.Count > 0);
	}



	private void InitSelection()
	{
		for (int i = 0; i < System.Enum.GetValues(typeof(EMapType)).Length; ++i)
		{
			if ($"{(EMapType)i}" == SettingsSystem.GetValue(m_mapTypeSettingsData.Key))
			{
				LevelSelectData.MapType = (EMapType)i;
				break;
			}
		}

		switch (LevelSelectData.MapType)
		{
			case EMapType.Game:
				m_currentThemesList = m_gamesThemesList;
				m_gameMapsTab.onClick.Invoke();
				m_gameMapsTab.Select();
				break;
			case EMapType.Custom:
				m_currentThemesList = m_customThemesList;
				m_customMapsTab.onClick.Invoke();
				m_customMapsTab.Select();
				break;
			case EMapType.Imported:
				m_currentThemesList = m_importedThemesList;
				m_importedMapsTab.onClick.Invoke();
				m_importedMapsTab.Select();
				break;
			default:
				break;
		}

		string themeName = SettingsSystem.GetValue(m_themeSettingsData.Key);
		for (int i = 0; i < m_currentThemesList.ThemesData.Length; ++i)
		{
			if ($"{m_currentThemesList.ThemesData[i].ThemeName}" == themeName)
			{
				m_themeIndex = i;
				break;
			}
		}
		m_currentTheme = m_currentThemesList.ThemesData[m_themeIndex];
		LevelSelectData.ThemeData = m_currentTheme;
		
		m_mapIndex = int.Parse(SettingsSystem.GetValue(m_mapIndexSettingsData.Key));
		LevelSelectData.ChosenMapIndex = m_mapIndex;
		LevelSelectData.SetMapData(m_currentTheme.Maps[m_mapIndex]);
		LevelSelectData.FileName = m_currentTheme.Maps[m_mapIndex].FileName;

		m_multiplayerToggle.isOn = LevelSelectData.IsMultiplayer;

		for (int i = 0; i < System.Enum.GetValues(typeof(EGameMode)).Length; ++i)
		{
			if ($"{(EGameMode)i}" == SettingsSystem.GetValue(m_gameModeSettingsData.Key))
			{
				LevelSelectData.GameMode = (EGameMode)i;
				break;
			}
		}
		
		for (int i = 0; i < System.Enum.GetValues(typeof(ETurnDirection)).Length; ++i)
		{
			if ($"{(ETurnDirection)i}" == SettingsSystem.GetValue(m_levelDirectionSettingsData.Key))
			{
				LevelSelectData.TurnDirection = (ETurnDirection)i;
				break;
			}
		}

		UpdateThemeIndex(0, true);
		
		// Init game mode dropdown choices:

		int gameModeValue = (int)LevelSelectData.GameMode;
		m_gameModeDropdown.ClearOptions();
		System.Collections.Generic.List<string> gameModes = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EGameMode)).Length; ++i)
		{
			string gameMode = $"{(EGameMode)i}";
			if (gameMode.StartsWith("M_"))                              // NOTE "M_" is stored as const, somewhere, as something like k_multiplayerModePrefix
			{
				if (LevelSelectData.IsMultiplayer == false)
					continue;
				else
					gameMode = gameMode.Substring(2);
			}
			gameModes.Add(gameMode);
		}
		m_gameModeDropdown.AddOptions(gameModes);
		m_gameModeDropdown.value = gameModeValue;
	}



	public void UpdateThemeButton(int indexDirection)
	{
		UpdateThemeIndex(indexDirection);
	}

	public void UpdateThemeIndex(int indexDirection, bool keepMapIndex = false)
	{
		m_themeIndex += indexDirection;

		m_currentTheme = m_currentThemesList.ThemesData[m_themeIndex];
		if (LevelSelectData.MapType == EMapType.Game)
		{
			m_themeIconImage.sprite = ((ThemeDataGameMaps)m_currentTheme).LevelSelectIcon;
			bool isLocked = m_totalPoints < ((ThemeDataGameMaps)m_currentTheme).PointsToUnlock;
			m_themeIconImage.color = (isLocked) ? new Color(0.7f, 0.7f, 0.7f, 1) : new Color(1, 1, 1, 1);
			m_playButton.interactable = (isLocked == false);
			if (isLocked)
			{
				m_lockedInfo.gameObject.SetActive(true);
				m_statsParentSinglePlayer.SetActive(false);
				m_statsParentMultiplayer.SetActive(false);
				m_lockedInfo.text = $"This theme is locked! Get {((ThemeDataGameMaps)m_currentTheme).PointsToUnlock:n0} points to Unlock.";
			}
			else
			{
				m_lockedInfo.gameObject.SetActive(false);
				m_statsParentSinglePlayer.SetActive(LevelSelectData.IsMultiplayer == false);
				m_statsParentMultiplayer.SetActive(LevelSelectData.IsMultiplayer);
			}
			m_themeHeading.text = m_currentTheme.ThemeName;
		}
		else
		{
			m_customImportedThemeText.text = m_currentTheme.ThemeName;
		}

		if (keepMapIndex == false)
			m_mapIndex = 0;
		
		m_mapIndexText.text = $"{m_mapIndex}";
		// [TODO] Show "1", or whatever we're using to represent the first level
		m_buttonMapUp.interactable = false;
		m_buttonMapDown.interactable = true;

		LevelSelectData.ThemeData = m_currentTheme;

		if (m_themeIndex == 0)
			m_buttonThemeUp.interactable = false;
		else
			m_buttonThemeUp.interactable = true;
		
		if (m_themeIndex == m_currentThemesList.ThemesData.Length - 1)
			m_buttonThemeDown.interactable = false;
		else
			m_buttonThemeDown.interactable = true;

		UpdateMapIndex(0);
	}

	public void UpdateMapIndex(int indexDirection)
	{
		m_mapIndex += indexDirection;
		m_mapIndexText.text = $"{m_mapIndex}";
		
		// [TODO] Show "1", or whatever we're using to represent the first level
		
		LevelSelectData.SetMapData(m_currentTheme.Maps[m_mapIndex]);
		LevelSelectData.ChosenMapIndex = m_mapIndex;
		LevelSelectData.FileName = m_currentTheme.Maps[m_mapIndex].FileName;

		if (LevelSelectData.MapType != EMapType.Game)
		{
			m_themeHeading.text = m_currentTheme.Maps[m_mapIndex].MapName;
			m_playButton.interactable = SaveSystem.IsMapPlayable(LevelSelectData.FileName, EGameMode.Items) || SaveSystem.IsMapPlayable(LevelSelectData.FileName, EGameMode.Exit);
			if (SaveSystem.IsMapPlayableMultiplayer(LevelSelectData.FileName) == false)
				m_multiplayerToggle.isOn = false;
			m_multiplayerToggle.interactable = SaveSystem.IsMapPlayableMultiplayer(LevelSelectData.FileName);
		}

		if (LevelSelectData.IsMultiplayer == false)
			UpdateLevelStatsSinglePlayer();

		if (m_mapIndex == 0)
			m_buttonMapUp.interactable = false;
		else
			m_buttonMapUp.interactable = true;
		
		if (m_mapIndex == m_currentTheme.Maps.Length - 1)
			m_buttonMapDown.interactable = false;
		else
			m_buttonMapDown.interactable = true;
	}



	private void UpdateLevelStatsSinglePlayer()
	{
		m_statTime.text = $"Time Taken: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Time)}s";
		m_statMoves.text = $"Moves Taken: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Moves)}";
		m_statLives.text = $"Lives Left: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Lives)}";	// [TODO][Q] Do we want time left, quickest time, or different depending on items or exit mode?
		if (LevelSelectData.GameMode == EGameMode.Items)
			m_statExtraInfo.text = $"Items Collected: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Items)}/{LevelSelectData.ThemeData.LevelPlayInfo.TotalItems}";
		else if (LevelSelectData.GameMode == EGameMode.Travel)
			m_statExtraInfo.text = $"Area Covered: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Area)}%";
		m_statScore.text = $"Level Score: {SaveSystem.GetStatsInfo(LevelSelectData.FileName, EStatsSection.Score):n0}";
	}

	private void UpdateStatsMultiplayer()
	{
		// [Q] Best place to call this?
		if (LevelSelectData.GameMode == EGameMode.M_Chase)
			LevelSelectData.ChaseIsRoundTwo = false;

		m_statsParentMultiplayer.GetComponent<GridLayoutGroup>().cellSize = new Vector2
		(
			m_statsParentMultiplayer.GetComponent<RectTransform>().rect.width / 4.0f,
			m_statsParentMultiplayer.GetComponent<GridLayoutGroup>().cellSize.y
		);

		m_multiStatsWinsP1.text = $"{PlayerPrefsSystem.MultiplayerGetWinsP1()}";
		m_multiStatsDraws.text = $"{PlayerPrefsSystem.MultiplayerGetDraws()}";
		m_multiStatsWinsP2.text = $"{PlayerPrefsSystem.MultiplayerGetWinsP2()}";
		m_multiStatsScoreP1.text = $"{PlayerPrefsSystem.MultiplayerGetScoreP1():n0}";
		m_multiStatsScoreP2.text = $"{PlayerPrefsSystem.MultiplayerGetScoreP2():n0}";
	}



	public void UpdateMultiplayerToggle(Toggle isMultiplayerToggle)
	{
		// [TODO]
		// Is there a better way of retaining the data???
		// [NOTE]
		// Currently doing like this since we're clearing the list each time, but because EXIT is in both modes,
		// want to retain that data if switching between single player and multiplayer (and if ITEMS selected then no updating is required)

		int gameModeValue = -1;
		if (m_gameModeDropdown.value > 2)												// NOT IDEAL - this will need changing if we add any new game modes to the enum. already changed from 1 to 2
			m_gameModeDropdown.value = 0;
		else if (m_gameModeDropdown.value > 0)
			gameModeValue = m_gameModeDropdown.value;

		LevelSelectData.IsMultiplayer = isMultiplayerToggle.isOn;

		m_gameModeDropdown.ClearOptions();
		System.Collections.Generic.List<string> gameModes = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EGameMode)).Length; ++i)
		{
			string gameMode = $"{(EGameMode)i}";
			if (gameMode.StartsWith("M_"))								// NOTE "M_" is stored as const, somewhere, as something like k_multiplayerModePrefix
			{
				if (isMultiplayerToggle.isOn == false)
					continue;
				else
					gameMode = gameMode.Substring(2);
			}
			gameModes.Add(gameMode);
		}
		m_gameModeDropdown.AddOptions(gameModes);
		
		if (gameModeValue == 1 || gameModeValue == 2)									// Same note as NOT IDEAL above
			m_gameModeDropdown.value = gameModeValue;

		if (m_totalPoints >= ((ThemeDataGameMaps)m_currentTheme).PointsToUnlock)		// This IF-ELSE is repeated!
		{
			m_lockedInfo.gameObject.SetActive(false);
			m_statsParentSinglePlayer.SetActive(LevelSelectData.IsMultiplayer == false);
			m_statsParentMultiplayer.SetActive(LevelSelectData.IsMultiplayer);
		}
		else
		{
			m_lockedInfo.gameObject.SetActive(true);
			m_statsParentSinglePlayer.SetActive(false);
			m_statsParentMultiplayer.SetActive(false);
			m_lockedInfo.text = $"This theme is locked! Get {((ThemeDataGameMaps)m_currentTheme).PointsToUnlock:n0} points to Unlock.";
		}

		if (LevelSelectData.IsMultiplayer)
			UpdateStatsMultiplayer();
		else
			UpdateLevelStatsSinglePlayer();

		if (isMultiplayerToggle.isOn)
			m_tutorialPopupMultiplayer.TryShow();
	}

	public void UpdateGameMode(TMP_Dropdown dropdown)
	{
		EGameMode gameMode = (EGameMode)dropdown.value;
		LevelSelectData.GameMode = gameMode;
		if (LevelSelectData.IsMultiplayer == false)
		{
			m_statExtraInfo.gameObject.SetActive(gameMode == EGameMode.Items || gameMode == EGameMode.Travel);
			UpdateLevelStatsSinglePlayer();
		}
		if (LevelSelectData.MapType != EMapType.Game)
		{
			if (gameMode == EGameMode.Items || gameMode == EGameMode.Exit)
				m_playButton.interactable = SaveSystem.IsMapPlayable(LevelSelectData.FileName, gameMode);
			else
				m_playButton.interactable = true;
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
		SettingsSystem.UpdateSettings(m_mapTypeSettingsData.Key, $"{LevelSelectData.MapType}");
		SettingsSystem.UpdateSettings(m_themeSettingsData.Key, m_currentTheme.ThemeName);
		SettingsSystem.UpdateSettings(m_mapIndexSettingsData.Key, $"{m_mapIndex}");
		SettingsSystem.UpdateSettings(m_gameModeSettingsData.Key, $"{LevelSelectData.GameMode}");
		SettingsSystem.UpdateSettings(m_levelDirectionSettingsData.Key, $"{LevelSelectData.TurnDirection}");
		SettingsSystem.SaveSettings();
		SceneManager.LoadScene("LevelScene");
	}
}
