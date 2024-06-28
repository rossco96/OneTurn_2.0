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

	private string[] m_existingMapFileNamesByCreationTime;
	private string m_selectedMapFileName;

	// [TODO] Implement!
	// If editing an existing field (must check no fields empty)
	// then if clicking NEW or switching in the dropdown - Autosave!
	//private bool m_levelInfoDirty = false;


	private void OnEnable()
	{
		// [TODO][Q] Implement input character limits?
		//m_inputAuthorName.characterLimit =

		InitExistingLevelsOptions();

		// [Q] Keep these here? Or move to LevelEditor.cs?
		LevelSelectData.IsInGame = false;
		LevelSelectData.IsMultiplayer = false;
		LevelEditorData.IsTestingLevel = true;
	}


	private void InitExistingLevelsOptions()
	{
		m_existingMapFileNamesByCreationTime = SaveSystem.GetCustomMapFileNamesByCreationTime();
		m_buttonLoadLevel.interactable = m_existingMapFileNamesByCreationTime.Length > 0;
		if (m_existingMapFileNamesByCreationTime.Length == 0)
			return;
		InitLoadLevelDropdown();
	}

	private void InitLoadLevelDropdown()
	{
		System.Collections.Generic.List<string> dropdownOptions = new System.Collections.Generic.List<string>();
		for (int i = 0; i < m_existingMapFileNamesByCreationTime.Length; ++i)
		{
			string mapName = SaveSystem.GetMapmetaInfo(m_existingMapFileNamesByCreationTime[i], EMapmetaInfo.MapName);
			dropdownOptions.Add(mapName);
		}
		m_loadLevelDropdown.AddOptions(dropdownOptions);
		m_loadLevelDropdown.value = 0;
	}


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
		m_selectedMapFileName = m_existingMapFileNamesByCreationTime[index];

		m_inputAuthorName.text = SaveSystem.GetMapmetaInfo(m_selectedMapFileName, EMapmetaInfo.AuthorName);
		m_inputMapName.text = SaveSystem.GetMapmetaInfo(m_selectedMapFileName, EMapmetaInfo.MapName);
		m_inputDescription.text = SaveSystem.GetMapmetaInfo(m_selectedMapFileName, EMapmetaInfo.Description);
		
		string gridDimension = SaveSystem.GetMapmetaInfo(m_selectedMapFileName, EMapmetaInfo.GridDimension);
		m_gridDimensionsText.text = $"Size: {gridDimension}x{gridDimension}";

		Texture2D texture = SaveSystem.GetCustomMapTexture(m_selectedMapFileName);
		m_mapImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, int.Parse(gridDimension) * Vector2.one), 0.5f * Vector2.one);

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
		string randomFileName = SaveSystem.CreateCustomMapmetaFile();
		SaveSystem.AddToNewCustomMapmetaFile(EMapmetaInfo.AuthorName, m_inputAuthorName.text);
		SaveSystem.AddToNewCustomMapmetaFile(EMapmetaInfo.MapName, m_inputMapName.text);
		SaveSystem.AddToNewCustomMapmetaFile(EMapmetaInfo.Description, m_inputDescription.text);
		SaveSystem.SaveCustomMapmetaFile();

		// Create blank map file
		SaveSystem.CreateCustomMapFile(randomFileName);
		SaveSystem.SaveCustomMapFile();

		// Create stat file
		SaveSystem.CreateStatFileCustomMap(randomFileName);

		// Set LevelEditorData
		LevelEditorData.LoadExistingLevel = false;
		LevelEditorData.CustomMapFileName = randomFileName;
		LevelEditorData.GridTexture = new Texture2D(9, 9);				// 9x9 is the smallest size and the standard upon creating a new map

		// LoadScene!
		SceneManager.LoadScene("LevelEditor");
	}

	public void EditExistingLevel()
	{
		// Set LevelEditorData
		LevelEditorData.LoadExistingLevel = true;
		LevelEditorData.CustomMapFileName = m_selectedMapFileName;
		LevelEditorData.GridTexture = SaveSystem.GetCustomMapTexture(m_selectedMapFileName);

		// LoadScene!
		SceneManager.LoadScene("LevelEditor");
	}
	#endregion
}
