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

	private string[] m_existingMapmetaFilepathsByCreationTime;

	// [TODO] Implement!
	// If editing an existing field (must check no fields empty)
	// then if clicking NEW or switching in the dropdown - Autosave!
	//private bool m_levelInfoDirty = false;


	// [TODO] Refactor OnEnable pls
	private void OnEnable()
	{
		// [TODO][Q] Implement input character limits?
		//m_inputAuthorName.characterLimit =

		InitExistingLevelsOptions();
	}

	// [TODO] Rename this! As it's basically just initialising the existing levels dropdown...
	private void InitExistingLevelsOptions()
	{
		string[] existingMapmetaFilepaths = SaveSystem.GetCustomMapmetaFilepaths();
		m_buttonLoadLevel.interactable = existingMapmetaFilepaths.Length > 0;

		if (existingMapmetaFilepaths.Length == 0) return;

		m_existingMapmetaFilepathsByCreationTime = new string[0];
		InitLoadLevelDropdown(existingMapmetaFilepaths);
	}

	private void InitLoadLevelDropdown(string[] filepaths)
	{
		List<MapmetaData<string, string>> mapmetaDatas = new List<MapmetaData<string, string>>();
		for (int i = 0; i < filepaths.Length; ++i)
		{
			MapmetaData<string, string> mapmetaData = SaveSystem.GetMapmetaContents(filepaths[i]);
			mapmetaDatas.Add(mapmetaData);
		}
		
		// Order the maps by the most recently created on top:

		List<string> dropdownOptions = new List<string>();
		for (int i = mapmetaDatas.Count - 1; i >= 0; --i)
		{
			int recentIndex = 0;
			long recentTime = 0;
			for (int j = mapmetaDatas.Count - 1; j >= 0; --j)
			{
				// [TODO][Q] Do we want creation time or most recently edited time?
				long mapmetaCreationTime = long.Parse(mapmetaDatas[j][$"{EMapmetaInfo.CreationTime}"]);
				if (mapmetaCreationTime > recentTime)
				{
					recentTime = mapmetaCreationTime;
					recentIndex = j;
				}
			}

			dropdownOptions.Add(mapmetaDatas[recentIndex][$"{EMapmetaInfo.MapName}"]);
			mapmetaDatas.RemoveAt(recentIndex);
			m_existingMapmetaFilepathsByCreationTime = m_existingMapmetaFilepathsByCreationTime.Add(filepaths[recentIndex]);
			filepaths = filepaths.RemoveAt(recentIndex);
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
		m_buttonContinue.gameObject.SetActive(true);										// [NOTE] Only needed for the first time.. can also move to inspector
		m_buttonContinue.GetComponentInChildren<TextMeshProUGUI>().text = "Create";
		m_buttonContinue.onClick.RemoveAllListeners();
		m_buttonContinue.onClick.AddListener(CreateNewLevel);
	}

	public void SetContinueButtonEdit()
	{
		m_buttonContinue.gameObject.SetActive(true);                                        //
		m_buttonContinue.GetComponentInChildren<TextMeshProUGUI>().text = "Edit";
		m_buttonContinue.onClick.RemoveAllListeners();
		m_buttonContinue.onClick.AddListener(EditExistingLevel);
	}


	public void LoadNewInfoPanel()
	{
		m_levelInfoPanel.SetActive(true);													//

		//m_inputAuthorName.ActivateInputField();	// what this do?
		m_inputAuthorName.text = string.Empty;
		m_inputMapName.text = string.Empty;
		m_inputDescription.text = string.Empty;

		m_mapImage.gameObject.SetActive(false);
		m_gridDimensionsText.gameObject.SetActive(false);
		m_buttonUpdateInfo.gameObject.SetActive(false);
	}

	public void LoadExistingInfoPanel(TMP_Dropdown dropdown)
	{
		m_levelInfoPanel.SetActive(true);                                                   //

		// [TODO][Q] Include this info as well? If so then uneditable, of course
		//m_creationTime.text = 
		//m_updatedTime.text = 

		int index = dropdown.value;

		m_inputAuthorName.text = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepathsByCreationTime[index], EMapmetaInfo.AuthorName);
		m_inputMapName.text = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepathsByCreationTime[index], EMapmetaInfo.MapName);
		m_inputDescription.text = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepathsByCreationTime[index], EMapmetaInfo.Description);
		
		string gridDimension = SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepathsByCreationTime[index], EMapmetaInfo.GridDimension);
		m_gridDimensionsText.text = $"Size: {gridDimension}x{gridDimension}";

		Texture2D texture = SaveSystem.GetCustomMapTexture(m_existingMapmetaFilepathsByCreationTime[index]);
		//m_mapImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, int.Parse(gridDimension) * Vector2.one), 0.5f * Vector2.one);	// Commented out for testing only!
		m_mapImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, 9 * Vector2.one), 0.5f * Vector2.one);							// (delete this after testing

		// [NOTE] Only needed if clicking the LOAD button. Not required for dropdown value changes (could put in separate method?)
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
		// Create mapmeta file through SaveSystem
		string fullFilepath = SaveSystem.CreateCustomMapmetaFile();
		SaveSystem.AddToCustomMapmetaFile(EMapmetaInfo.AuthorName, m_inputAuthorName.text);
		SaveSystem.AddToCustomMapmetaFile(EMapmetaInfo.MapName, m_inputMapName.text);
		SaveSystem.AddToCustomMapmetaFile(EMapmetaInfo.Description, m_inputDescription.text);
		SaveSystem.SaveCustomMapmetaFile();

		SaveSystem.CreateCustomMapFile(fullFilepath);
		SaveSystem.SaveCustomMapFile();

		// [TODO]
		//SaveSystem.CreateCustomStatFile(randomFileName);

		// Set LevelEditorData
		LevelEditorData.LoadExistingLevel = false;
		LevelEditorData.CustomMapFullFilepath = fullFilepath;

		SceneManager.LoadScene("LevelEditor");
	}

	public void EditExistingLevel()
	{
		// Set LevelEditorData
		LevelEditorData.LoadExistingLevel = true;
		LevelEditorData.CustomMapFullFilepath = m_existingMapmetaFilepathsByCreationTime[m_loadLevelDropdown.value];    // [TODO] Cache currently selected filepath?
		LevelEditorData.GridTexture = SaveSystem.GetCustomMapTexture(m_existingMapmetaFilepathsByCreationTime[m_loadLevelDropdown.value]);
		LevelEditorData.GridDimension = int.Parse(SaveSystem.GetMapmetaInfo(m_existingMapmetaFilepathsByCreationTime[m_loadLevelDropdown.value], EMapmetaInfo.GridDimension));

		SceneManager.LoadScene("LevelEditor");
	}
	#endregion
}
