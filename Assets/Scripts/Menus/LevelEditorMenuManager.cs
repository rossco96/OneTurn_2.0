//using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// [TODO] Can this whole class appear as a popup rather than a separate screen?

public class LevelEditorMenuManager : MonoBehaviour
{
	[SerializeField] private CustomLevelData[] m_customLevels;								// [Q] Can delete this?
	//[SerializeField] private CustomLevelsList m_customLevelsList;							// [TODO] Implement this!

	[Space]
	[SerializeField] private Button m_buttonLoadLevel;
	[SerializeField] private TMPro.TMP_Dropdown m_loadLevelDropdown;
	//[SerializeField] private Sprite m_loadLevelSprite;									// [DELETE] Can set the m_loadLevelDropdown sprite!

	//private CustomLevelData m_currentLoadLevel;

	private string[] m_existingMapmetaFilepaths;
	private string[] m_orderedExistingMapmetas;
	private int m_existingLevelsCount;

	// [TODO] Implement!
	// If editing an existing field (must check no fields empty)
	// then if clicking NEW or switching in the dropdown - Autosave!
	private bool m_levelInfoDirty = false;


	private void OnEnable()
	{
		m_existingMapmetaFilepaths = SaveSystem.GetCustomMapmetaFilepaths();
		m_existingLevelsCount = m_existingMapmetaFilepaths.Length;

		bool levelsToLoad = m_existingLevelsCount > 0;
		m_buttonLoadLevel.interactable = levelsToLoad;
		// Don't do this here! Enable when clicking on LOAD button, disable if then click on NEW button
		//m_loadLevelDropdown.gameObject.SetActive(levelsToLoad);
		if (levelsToLoad)
			InitLoadLevelDropdown();
	}

	// [TODO] Rename this! As it's basically just initialising the existing levels dropdown...
	private void InitLoadLevelDropdown()
	{
		System.Collections.Generic.List<MapmetaData<string, string>> mapmetaDatas = new System.Collections.Generic.List<MapmetaData<string, string>>();
		for (int i = 0; i < m_existingLevelsCount; ++i)
		{
			MapmetaData<string, string> mapmetaData = SaveSystem.GetMapmetaContents(m_existingMapmetaFilepaths[i]);
			mapmetaDatas.Add(mapmetaData);
			// [TODO] Order the maps by the most recently created on top!
		}

		System.Collections.Generic.List<string> dropdownOptions = new System.Collections.Generic.List<string>();
		for (int i = mapmetaDatas.Count - 1; i >= 0; --i)
		{
			int recentIndex = 0;
			int recentTime = 0;
			for (int j = mapmetaDatas.Count - 1; j >= 0; ++j)
			{
				int mapmetaCreationTime = int.Parse(mapmetaDatas[i]["CreationTime"]);							// [TODO][Q] Do we want creation time or most recently edited time?
				if (mapmetaCreationTime > recentTime)
				{
					recentTime = mapmetaCreationTime;
					recentIndex = j;
				}
			}
			// [TODO][Q] Also add sprites?

			dropdownOptions.Add(mapmetaDatas[recentIndex]["MapName"]);
			mapmetaDatas.RemoveAt(recentIndex);
		}

		m_loadLevelDropdown.AddOptions(dropdownOptions);
		m_loadLevelDropdown.value = 0;
	}


	// DELETE!!!
	[ContextMenu("TEST_CreateMapmetaFile")]
	public void TEST_CreateMapmetaFile()
	{
		SaveSystem.CreateCustomMapmetaFile();
	}
	// DELETE!!!


	#region UI Button Methods
	public void ToggleLoadLevelSelectScreen()
	{

	}

	public void ToggleLevelInfoScreen()
	{

	}

	public void CreateNewLevel()
	{

	}

	public void UpdateLevelInfo()
	{

	}

	public void LoadLevel()
	{
		SceneManager.LoadScene("LevelEditor");
	}
	#endregion



	/*
	public void UpdateThemeIndex(int indexDirection)
	{
		m_themeIndex += indexDirection;

		m_currentTheme = m_themesList.ThemesData[m_themeIndex];
		m_themeIconImage.sprite = m_currentTheme.LevelSelectIcon;
		m_numberOfMaps = m_currentTheme.Maps.Length;
		
		m_mapIndex = 0;
		// [TODO] Show "1", or whatever we're using to represent the first level
		m_buttonMapUp.interactable = false;
		if (m_buttonMapDown.interactable == false)
			m_buttonMapDown.interactable = true;
		// [TODO] For testing only! Will never need this, unless only one map in a given theme
		if (m_mapIndex == m_numberOfMaps - 1)
			m_buttonMapDown.interactable = false;

		LevelSelectData.SetThemeData(m_currentTheme);
		LevelSelectData.SetMapData(m_currentTheme.Maps[0]);

		// [TODO][IMPORTANT] 'else' commented out here (and in UpdateMapIndex below) since testing with such small numbers of indexes - there's some overlap!
		if (m_themeIndex == 0)
			m_buttonThemeUp.interactable = false;
		/*else* / if (m_themeIndex == 1 && m_buttonThemeUp.interactable == false)
			m_buttonThemeUp.interactable = true;
		/*else* / if (m_themeIndex == m_numberOfThemes - 1)
			m_buttonThemeDown.interactable = false;
		/*else* / if (m_themeIndex == m_numberOfThemes - 2 && m_buttonThemeDown.interactable == false)
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

		if (m_mapIndex == 0)
			m_buttonMapUp.interactable = false;
		/*else* / if (m_mapIndex == 1 && m_buttonMapUp.interactable == false)
			m_buttonMapUp.interactable = true;
		/*else* / if (m_mapIndex == m_numberOfMaps - 1)
			m_buttonMapDown.interactable = false;
		/*else* / if (m_mapIndex == m_numberOfMaps - 2 && m_buttonMapDown.interactable == false)
			m_buttonMapDown.interactable = true;
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

		LevelSelectData.SetIsMultiplayer(isMultiplayerToggle.isOn);

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
		
		if (gameModeValue == 1)
			m_gameModeDropdown.value = 1;
	}

	public void UpdateGameMode(TMPro.TMP_Dropdown dropdown)
	{
		EGameMode gameMode = (EGameMode)dropdown.value;
		//Debug.Log($"[LevelSelectMenuManager::UpdateGameMode] If generated in the correct order, we should be selecting: '{gameMode}'");
		LevelSelectData.SetGameMode(gameMode);
	}

	public void UpdateTurnDirection(TMPro.TMP_Dropdown dropdown)
	{
		ETurnDirection turnDirection = (ETurnDirection)dropdown.value;
		//Debug.Log($"[LevelSelectMenuManager::UpdateTurnDirection] If generated in the correct order, we should be selecting: '{turnDirection}'");
		LevelSelectData.SetTurnDirection(turnDirection);
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
	//*/
}
