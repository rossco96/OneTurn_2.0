using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenuManager : MonoBehaviour
{
	[SerializeField] private ThemesList m_themesList;

	[Space]
	[SerializeField] private Button m_buttonGameTab;
	[SerializeField] private Button m_buttonCustomTab;

	[Space]
	[SerializeField] private Image m_themeIconImage;
	
	[SerializeField] private Button m_buttonThemeUp;
	[SerializeField] private Button m_buttonThemeDown;
	[SerializeField] private Button m_buttonMapUp;
	[SerializeField] private Button m_buttonMapDown;

	[Space]
	[SerializeField] private TMPro.TMP_Dropdown m_gameModeDropdown;

	private ThemeData m_currentTheme;

	private int m_numberOfThemes;
	private int m_themeIndex;
	private int m_numberOfMaps;
	private int m_mapIndex;



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

		m_buttonThemeUp.interactable = false;
		m_buttonMapUp.interactable = false;
		// ^^^^^ ^^^^^ ^^^^^

		//m_currentTheme = 
		//m_themeIndex = 
		//m_mapIndex = 
	}



	public void SelectGameTab()
	{

	}

	public void SelectCustomTab()
	{

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
		if (m_buttonMapDown.interactable == false)
			m_buttonMapDown.interactable = true;
		// [TODO] For testing only! Will never need this, unless only one map in a given theme
		if (m_mapIndex == m_numberOfMaps - 1)
			m_buttonMapDown.interactable = false;

		LevelSelectData.ThemeData = m_currentTheme;
		LevelSelectData.SetMapData(m_currentTheme.Maps[0]);

		// [TODO][IMPORTANT] 'else' commented out here (and in UpdateMapIndex below) since testing with such small numbers of indexes - there's some overlap!
		if (m_themeIndex == 0)
			m_buttonThemeUp.interactable = false;
		/*else*/ if (m_themeIndex == 1 && m_buttonThemeUp.interactable == false)
			m_buttonThemeUp.interactable = true;
		/*else*/ if (m_themeIndex == m_numberOfThemes - 1)
			m_buttonThemeDown.interactable = false;
		/*else*/ if (m_themeIndex == m_numberOfThemes - 2 && m_buttonThemeDown.interactable == false)
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
		/*
		m_mapData = mapData;
		m_gridDimension = m_mapData.GridLayout.width;
		// [NOTE][IMPORTANT] 0.2f at the end is because the border is *CURRENTLY* 1/10th the width of the walls (multiplied by two lots of borders, one each side of the screen)
		// [TODO][Q] Make sure we're calculating the difference?? Ask an artist about import settings??
		// Border should always be the same size, regardless of grid dimnsion!
		m_gridSizeMultiplier = (UnityEngine.Camera.main.aspect * UnityEngine.Camera.main.orthographicSize * 2.0f) / (m_gridDimension + 0.2f);
		//*/

		if (m_mapIndex == 0)
			m_buttonMapUp.interactable = false;
		/*else*/ if (m_mapIndex == 1 && m_buttonMapUp.interactable == false)
			m_buttonMapUp.interactable = true;
		/*else*/ if (m_mapIndex == m_numberOfMaps - 1)
			m_buttonMapDown.interactable = false;
		/*else*/ if (m_mapIndex == m_numberOfMaps - 2 && m_buttonMapDown.interactable == false)
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

		LevelSelectData.IsMultiplayer = isMultiplayerToggle.isOn;

		m_gameModeDropdown.ClearOptions();
		System.Collections.Generic.List<string> gameModes = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EGameMode)).Length; ++i)
		{
			string gameMode = $"{(EGameMode)i}";
			if (gameMode.StartsWith("_"))
				continue;

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
		LevelSelectData.GameMode = gameMode;
	}

	public void UpdateTurnDirection(TMPro.TMP_Dropdown dropdown)
	{
		ETurnDirection turnDirection = (ETurnDirection)dropdown.value;
		LevelSelectData.TurnDirection = turnDirection;
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
