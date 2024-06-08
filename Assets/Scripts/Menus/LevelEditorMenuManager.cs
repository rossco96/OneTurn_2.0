using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// [TODO] Can this whole class appear as a popup rather than a separate screen?

public class LevelEditorMenuManager : MonoBehaviour
{
	[Space]
	[SerializeField] private Button m_buttonLoadLevel;
	[SerializeField] private TMP_Dropdown m_loadLevelDropdown;

	[Space]
	[SerializeField] private GameObject m_levelInfoPanel;
	[SerializeField] private TMP_InputField m_inputAuthorName;
	[SerializeField] private TMP_InputField m_inputMapName;
	[SerializeField] private TMP_InputField m_inputDescription;
	[SerializeField] private Image m_mapImage;
	[SerializeField] private TextMeshProUGUI m_gridDimensionsText;
	[SerializeField] private Button m_buttonUpdateInfo;

	[Space]
	[SerializeField] private Button m_buttonContinue;

	private string[] m_existingMapmetaFilepaths;
	private int m_existingLevelsCount;

	// [TODO] Implement!
	// If editing an existing field (must check no fields empty)
	// then if clicking NEW or switching in the dropdown - Autosave!
	private bool m_levelInfoDirty = false;


	// [TODO] Refactor OnEnable pls
	private void OnEnable()
	{
		// [TODO][Q] Implement input character limits?
		//m_inputAuthorName.characterLimit =

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
		List<MapmetaData<string, string>> mapmetaDatas = new List<MapmetaData<string, string>>();
		for (int i = 0; i < m_existingLevelsCount; ++i)
		{
			MapmetaData<string, string> mapmetaData = SaveSystem.GetMapmetaContents(m_existingMapmetaFilepaths[i]);
			mapmetaDatas.Add(mapmetaData);
			// [TODO] Order the maps by the most recently created on top!
		}

		List<string> dropdownOptions = new List<string>();
		for (int i = mapmetaDatas.Count - 1; i >= 0; --i)
		{
			int recentIndex = 0;
			long recentTime = 0;
			for (int j = mapmetaDatas.Count - 1; j >= 0; --j)
			{
				// [TODO][Q] Do we want creation time or most recently edited time?
				long mapmetaCreationTime = long.Parse(mapmetaDatas[i][$"{EMapmetaInfo.CreationTime}"]);
				if (mapmetaCreationTime > recentTime)
				{
					recentTime = mapmetaCreationTime;
					recentIndex = j;
				}
			}
			// [TODO][Q] Also add sprites?

			dropdownOptions.Add(mapmetaDatas[recentIndex][$"{EMapmetaInfo.MapName}"]);
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
	public void SetContinueButtonCreate()
	{
		m_buttonContinue.gameObject.SetActive(true);										// [NOTE] Only needed for the first time..
		m_buttonContinue.GetComponentInChildren<TextMeshProUGUI>().text = "Create";
		m_buttonContinue.onClick.RemoveAllListeners();
		m_buttonContinue.onClick.AddListener(CreateNewLevel);
	}

	public void SetContinueButtonEdit()
	{
		m_buttonContinue.gameObject.SetActive(true);                                        // [NOTE] Only needed for the first time..
		m_buttonContinue.GetComponentInChildren<TextMeshProUGUI>().text = "Edit";
		m_buttonContinue.onClick.RemoveAllListeners();
		m_buttonContinue.onClick.AddListener(EditExistingLevel);
	}


	public void LoadNewInfoPanel()
	{
		m_levelInfoPanel.SetActive(true);													// [NOTE] Only needed for the first time..

		//m_inputAuthorName.ActivateInputField();
		m_inputAuthorName.text = string.Empty;
		m_inputMapName.text = string.Empty;
		m_inputDescription.text = string.Empty;

		m_mapImage.gameObject.SetActive(false);
		m_gridDimensionsText.gameObject.SetActive(false);
		m_buttonUpdateInfo.gameObject.SetActive(false);
	}

	public void LoadExistingInfoPanel()
	{
		m_levelInfoPanel.SetActive(true);                                                   // [NOTE] Only needed for the first time..

		// [TODO][Q] Include this info as well?
		//m_creationTime.text = 
		//m_updatedTime.text = 

		// [TODO][IMPORTANT] Replace 0 with the correct value!!!
		m_inputAuthorName.text = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepaths[0], EMapmetaInfo.AuthorName);
		m_inputMapName.text = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepaths[0], EMapmetaInfo.MapName);
		m_inputDescription.text = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepaths[0], EMapmetaInfo.Description);

		//m_mapImage.sprite = ;
		
		string gridDimensions = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepaths[0], EMapmetaInfo.GridDimension);
		m_gridDimensionsText.text = $"Size: {gridDimensions}x{gridDimensions}";

		m_mapImage.gameObject.SetActive(true);
		m_gridDimensionsText.gameObject.SetActive(true);
		m_buttonUpdateInfo.gameObject.SetActive(true);
		m_buttonUpdateInfo.interactable = false;
	}

	public void UpdateLevelInfo()
	{

	}


	public void CreateNewLevel()
	{

	}

	public void EditExistingLevel()
	{
		SceneManager.LoadScene("LevelEditor");
	}
	#endregion


	/*
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
		List<string> gameModes = new List<string>();
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

	public void UpdateGameMode(TMP_Dropdown dropdown)
	{
		EGameMode gameMode = (EGameMode)dropdown.value;
		//Debug.Log($"[LevelSelectMenuManager::UpdateGameMode] If generated in the correct order, we should be selecting: '{gameMode}'");
		LevelSelectData.SetGameMode(gameMode);
	}

	public void UpdateTurnDirection(TMP_Dropdown dropdown)
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
