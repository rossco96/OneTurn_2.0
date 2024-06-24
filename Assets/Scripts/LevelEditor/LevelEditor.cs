using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// [TODO] Refactor this into separate classes -- Grid, Tool, Tutorial, Metadata, Other Menu Buttons

public class LevelEditor : MonoBehaviour
{
	private const int k_maxGridSize = 15;														// [Q] Do this here? Also include k_minGridSize = 9?


	[SerializeField] private MapPropertyData m_mapPropertyData;
	[SerializeField] private ThemesList m_themesList;

	[Space]
	[SerializeField] private TextMeshProUGUI m_sliderLabel;
	[SerializeField] private Slider m_gridSlider;
	[SerializeField] private GridLayoutGroup m_gridParent;
	[SerializeField] private GameObject[] m_gridRows;

	[Space]
	[SerializeField] private TMP_Dropdown m_toolsDropdown;
	[SerializeField] private TMP_Dropdown m_themeDropdown;

	[Space]
	[SerializeField] private GameObject m_tutorialPopup;
	[SerializeField] private GameObject m_mainMenuPopup;
	[SerializeField] private GameObject m_testOptionsPopup;

	[Space]
	[SerializeField] private TextMeshProUGUI m_extraInfoToolItemsUsed;
	// Note the two TextMeshProUGUI below could be GameObjects, but want consistency when setting active
	[SerializeField] private TextMeshProUGUI m_extraInfoRotationRight_Label;
	[SerializeField] private TextMeshProUGUI m_extraInfoRotationLeft_Label;
	[SerializeField] private TMP_Dropdown m_extraInfoRotationRight_Dropdown;
	[SerializeField] private TMP_Dropdown m_extraInfoRotationLeft_Dropdown;

	// [TODO] Move the below to their own TestModePopup script???
	[Space]
	[SerializeField] private TMP_Dropdown m_gameModeDropdown;
	[SerializeField] private TMP_Dropdown m_turnDirectionDropdown;
	[SerializeField] private Toggle m_startAtSecondSpawnToggle;
	[SerializeField] private Toggle m_moveThroughWallsToggle;

	private bool m_levelDataDirty = false;

	//private string m_levelFileName;                                                           // [TODO] Implement! ... Or just use LevelEditorData.CustomMapFileName ?

	private int m_gridDimension = 9;
	private EMapPropertyName m_currentTool = EMapPropertyName.BlankSquare;

	private int m_maxItems = 8;
	private int m_maxExits = 1;
	private int m_maxSpawnPoints = 1;

	private int m_placedExits = 0;
	private int m_placedItems = 0;
	private int m_placedSpawnPointsPrimary = 0;
	private int m_placedSpawnPointsSecondary = 0;

	// [TODO][Q] Better way to do this?
	private EFacingDirection m_exitDirectionRight = EFacingDirection.Up;
	private EFacingDirection m_exitDirectionLeft = EFacingDirection.Up;
	private EFacingDirection m_spawnPrimaryDirectionRight = EFacingDirection.Up;
	private EFacingDirection m_spawnPrimaryDirectionLeft = EFacingDirection.Up;
	private EFacingDirection m_spawnSecondaryDirectionRight = EFacingDirection.Up;
	private EFacingDirection m_spawnSecondaryDirectionLeft = EFacingDirection.Up;


	private void Awake()
	{
		InitGridButtons();
		InitToolsDropdown();
		InitThemesDropdown();
		InitExtraInfoDropdowns();
		InitTestDropdowns();

		//m_levelFileName = LevelSelectData.CustomData.											// [TODO] Implement? Or just use LevelEditorData.CustomMapFileName?
		// [TODO] Also display level name on the screen?
	}


	#region INIT
	private void InitGridButtons()
	{
		int gridDimension = LevelEditorData.GridTexture.width;
		m_gridSlider.value = (gridDimension - 7) / 2;
		m_maxItems = ((int)m_gridSlider.value * (int)m_gridSlider.value) + (int)m_gridSlider.value + 6;

		for (int i = 0; i < k_maxGridSize; ++i)
		{
			GridButton[] gridButtons = m_gridRows[i].GetComponentsInChildren<GridButton>(true);
			m_gridRows[i].SetActive(i < gridDimension);
			for (int j = 0; j < k_maxGridSize; ++j)
			{
				GridButton gb = gridButtons[j];
				gb.gameObject.SetActive(j < gridDimension);
				gb.RegisterOnButtonSelected(OnGridButtonClicked);
				if (i < gridDimension && j < gridDimension)
				{
					gb.SetPropertyColor
					(
						(LevelEditorData.LoadExistingLevel)
							? LevelEditorData.GridTexture.GetPixel(j, gridDimension - i - 1)
							: m_mapPropertyData.GetColorByName(EMapPropertyName.BlankSquare)
					);
				}
				else
				{
					gb.SetPropertyColor(m_mapPropertyData.GetColorByName(EMapPropertyName.BlankSquare));
				}
			}
		}

		// [Q] Save immediately if LevelEditorData.LoadExistingLevel == false ??
	}

	private void InitToolsDropdown()
	{
		System.Collections.Generic.List<string> dropdownOptions = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EMapPropertyName)).Length; ++i)
		{
			// [TODO][Q] Also add sprites? Show sprites next to it with the name of the tool?
			if ((EMapPropertyName)i == EMapPropertyName.Special) continue;
			string option = $"{(EMapPropertyName)i}";
			if (option.CanFormatCamelCase(out string optionFormat))
				dropdownOptions.Add(optionFormat);
			else
				dropdownOptions.Add(option);
		}
		m_toolsDropdown.AddOptions(dropdownOptions);
		m_toolsDropdown.value = 0;

		// [TODO][Q] I don't think this will be possible. Looks like if setting sprites in the dropdown, it can only reference one item image?
		//m_toolsDropdown.itemImage.sprite = m_mapPropertyData.GetDropdownSpriteByName(m_currentTool);
	}

	private void InitThemesDropdown()
	{
		System.Collections.Generic.List<string> dropdownOptions = new System.Collections.Generic.List<string>();
		for (int i = 0; i < m_themesList.ThemesData.Length; ++i)
		{
			dropdownOptions.Add(m_themesList.ThemesData[i].ThemeName);
		}
		m_themeDropdown.AddOptions(dropdownOptions);
		m_themeDropdown.value = 0;
	}

	private void InitExtraInfoDropdowns()
	{
		System.Collections.Generic.List<string> rotations = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EFacingDirection)).Length; ++i)
		{
			rotations.Add($"{(EFacingDirection)i}");
		}
		m_extraInfoRotationRight_Dropdown.AddOptions(rotations);
		m_extraInfoRotationRight_Dropdown.value = 0;
		m_extraInfoRotationLeft_Dropdown.AddOptions(rotations);
		m_extraInfoRotationLeft_Dropdown.value = 0;
	}

	private void InitTestDropdowns()
	{
		System.Collections.Generic.List<string> gameModes = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EGameMode)).Length; ++i)
		{
			string gameMode = $"{(EGameMode)i}";
			if (gameMode.StartsWith("M_")) continue;
			gameModes.Add(gameMode);
		}
		m_gameModeDropdown.AddOptions(gameModes);
		m_gameModeDropdown.value = 0;

		System.Collections.Generic.List<string> turnDirections = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(ETurnDirection)).Length; ++i)
		{
			turnDirections.Add($"{(ETurnDirection)i}");
		}
		m_turnDirectionDropdown.AddOptions(turnDirections);
		m_turnDirectionDropdown.value = 0;
	}
	#endregion


	#region Tools Dropdown
	public void OnToolChanged(TMP_Dropdown dropdown)
	{
		m_currentTool = (EMapPropertyName)dropdown.value;
		switch (m_currentTool)
		{
			case EMapPropertyName.Item:
				m_extraInfoToolItemsUsed.gameObject.SetActive(true);
				m_extraInfoToolItemsUsed.text = $"Items placed: {m_placedItems}/{m_maxItems}";
				m_extraInfoRotationRight_Label.gameObject.SetActive(false);
				m_extraInfoRotationLeft_Label.gameObject.SetActive(false);
				m_extraInfoRotationRight_Dropdown.gameObject.SetActive(false);
				m_extraInfoRotationLeft_Dropdown.gameObject.SetActive(false);
				break;
			case EMapPropertyName.Exit:
				m_extraInfoToolItemsUsed.gameObject.SetActive(true);
				m_extraInfoToolItemsUsed.text = $"Exits placed: {m_placedExits}/{m_maxExits}";
				m_extraInfoRotationRight_Label.gameObject.SetActive(true);
				m_extraInfoRotationLeft_Label.gameObject.SetActive(true);
				m_extraInfoRotationRight_Dropdown.gameObject.SetActive(true);
				m_extraInfoRotationLeft_Dropdown.gameObject.SetActive(true);
				break;
			case EMapPropertyName.SpawnPointPrimary:
				m_extraInfoToolItemsUsed.gameObject.SetActive(true);
				m_extraInfoToolItemsUsed.text = $"Primary spawns placed: {m_placedSpawnPointsPrimary}/{m_maxSpawnPoints}";
				m_extraInfoRotationRight_Label.gameObject.SetActive(true);
				m_extraInfoRotationLeft_Label.gameObject.SetActive(true);
				m_extraInfoRotationRight_Dropdown.gameObject.SetActive(true);
				m_extraInfoRotationLeft_Dropdown.gameObject.SetActive(true);
				break;
			case EMapPropertyName.SpawnPointSecondary:
				m_extraInfoToolItemsUsed.gameObject.SetActive(true);
				m_extraInfoToolItemsUsed.text = $"Secondary spawns placed: {m_placedSpawnPointsSecondary}/{m_maxSpawnPoints}";
				m_extraInfoRotationRight_Label.gameObject.SetActive(true);
				m_extraInfoRotationLeft_Label.gameObject.SetActive(true);
				m_extraInfoRotationRight_Dropdown.gameObject.SetActive(true);
				m_extraInfoRotationLeft_Dropdown.gameObject.SetActive(true);
				break;
			default:
				m_extraInfoToolItemsUsed.gameObject.SetActive(false);
				m_extraInfoRotationRight_Label.gameObject.SetActive(false);
				m_extraInfoRotationLeft_Label.gameObject.SetActive(false);
				m_extraInfoRotationRight_Dropdown.gameObject.SetActive(false);
				m_extraInfoRotationLeft_Dropdown.gameObject.SetActive(false);
				break;
		}
		
		// [TODO] Must set this up in the editor!
		m_toolsDropdown.captionImage.sprite = m_mapPropertyData.GetDropdownSpriteByName(m_currentTool);
	}
	#endregion


	#region Grid Buttons
	public void OnGridButtonClicked(GridButton gb)
	{
		EMapPropertyName property = m_mapPropertyData.GetNameByColor(gb.PropertyColor);
		if (UpdateGridPropertiesCount(property) == false)
			return;
		m_levelDataDirty = true;
		gb.SetPropertyColor(m_mapPropertyData.GetColorByName(m_currentTool));
	}

	private bool UpdateGridPropertiesCount(EMapPropertyName property)
	{
		if (m_currentTool == property)
			return false;

		switch (m_currentTool)
		{
			case EMapPropertyName.Item:
				if (m_placedItems >= m_maxItems)
					return false;
				m_placedItems++;
				m_extraInfoToolItemsUsed.text = $"Items placed: {m_placedItems}/{m_maxItems}";
				break;
			case EMapPropertyName.Exit:
				if (m_placedExits >= m_maxExits)
					return false;
				m_placedExits++;
				m_extraInfoToolItemsUsed.text = $"Exits placed: {m_placedExits}/{m_maxExits}";
				break;
			case EMapPropertyName.SpawnPointPrimary:
				if (m_placedSpawnPointsPrimary >= m_maxSpawnPoints)
					return false;
				m_placedSpawnPointsPrimary++;
				m_extraInfoToolItemsUsed.text = $"Primary spawns placed: {m_placedSpawnPointsPrimary}/{m_maxSpawnPoints}";
				break;
			case EMapPropertyName.SpawnPointSecondary:
				if (m_placedSpawnPointsSecondary >= m_maxSpawnPoints)
					return false;
				m_placedSpawnPointsSecondary++;
				m_extraInfoToolItemsUsed.text = $"Secondary spawns placed: {m_placedSpawnPointsSecondary}/{m_maxSpawnPoints}";
				break;
			default:
				break;
		}

		switch (property)
		{
			case EMapPropertyName.Item:
				if (m_currentTool != EMapPropertyName.Item)
					m_placedItems--;
				break;
			case EMapPropertyName.Exit:
				if (m_currentTool != EMapPropertyName.Exit)
					m_placedExits--;
				break;
			case EMapPropertyName.SpawnPointPrimary:
				if (m_currentTool != EMapPropertyName.SpawnPointPrimary)
					m_placedSpawnPointsPrimary--;
				break;
			case EMapPropertyName.SpawnPointSecondary:
				if (m_currentTool != EMapPropertyName.SpawnPointSecondary)
					m_placedSpawnPointsSecondary--;
				break;
			default:
				break;
		}

		return true;
	}

	public void UpdateGridLayout(Slider slider)
	{
		m_gridDimension = ((int)slider.value * 2) + 7;
		m_maxItems = ((int)slider.value * (int)slider.value) + (int)slider.value + 6;
		m_extraInfoToolItemsUsed.text = $"Items placed: {m_placedItems}/{m_maxItems}";
		SetNewGridSize();
		m_sliderLabel.text = $"Size: {m_gridDimension}x{m_gridDimension}";
	}

	private void SetNewGridSize()
	{
		// Take 40 away, since we want 20 pixel buffer from the edges of the screen
		int buttonSize = (Screen.width - 40) / m_gridDimension;
		m_gridParent.cellSize = new Vector2(buttonSize * m_gridDimension, buttonSize);
		for (int i = 0; i < k_maxGridSize; ++i)
		{
			GameObject row = m_gridRows[i];
			row.SetActive(i < m_gridDimension);
			if (i < m_gridDimension)
			{
				GridLayoutGroup layoutGroup = row.GetComponent<GridLayoutGroup>();
				layoutGroup.cellSize = new Vector2(buttonSize, buttonSize);
				GridButton[] gridButtons = row.GetComponentsInChildren<GridButton>(true);
				for (int j = 0; j < k_maxGridSize; ++j)
				{
					gridButtons[j].gameObject.SetActive(j < m_gridDimension);
				}
			}
		}
	}
	#endregion


	#region Menu Buttons
	public void ReturnToMenu()
	{
		// Must set this to false on returning to the main menu as LevelScene checks this
		LevelEditorData.IsTestingLevel = false;
		if (m_levelDataDirty)
			m_mainMenuPopup.SetActive(true);
		else
			SceneManager.LoadScene("MainMenu");
	}
	
	public void Save()
	{
		// [TODO][IMPORTANT] Must also save the metadata!!!
		// ... Is that done via any of the SaveSystem._CustomMapFile() methods?
		SaveSystem.CreateCustomMapFile(LevelEditorData.CustomMapFileName, m_gridDimension);
		for (int i = 0; i < m_gridDimension; ++i)
		{
			GridButton[] gridButtons = m_gridRows[i].GetComponentsInChildren<GridButton>(true);
			for (int j = 0; j < m_gridDimension; ++j)
			{
				Color color = gridButtons[j].PropertyColor;
				SaveSystem.AddToCustomMapFile(color, j, m_gridDimension - i - 1);
			}
		}
		SaveSystem.SaveCustomMapFile();
		m_levelDataDirty = false;
		// [TODO] "Saved" popup? Or brief text bubble which doesn't get in the way?
	}

	// [TODO] If can't make look more effecient, at least refactor?
	public void TestLevel()
	{
		LevelEditorData.IsTestingLevel = true;

		// [TODO][Q] Must be better way of doing this?
		MapData map = ScriptableObject.CreateInstance<MapData>();
		map.ExitFacingDirectionLeft = m_exitDirectionLeft;
		map.ExitFacingDirectionRight = m_exitDirectionRight;
		map.PlayerSpawnDirectionRight = new EFacingDirection[] { m_spawnPrimaryDirectionRight, m_spawnSecondaryDirectionRight };
		map.PlayerSpawnDirectionLeft = new EFacingDirection[] { m_spawnPrimaryDirectionLeft, m_spawnSecondaryDirectionLeft };

		LevelEditorData.ResetGridTexture(m_gridDimension);
		for (int i = 0; i < m_gridDimension; ++i)
		{
			GridButton[] gridButtons = m_gridRows[i].GetComponentsInChildren<GridButton>(true);
			for (int j = 0; j < m_gridDimension; ++j)
			{
				Color color = gridButtons[j].PropertyColor;
				LevelEditorData.AddToGridTexture(color, j, m_gridDimension - i - 1);
			}
		}
		map.GridLayout = LevelEditorData.GridTexture;

		LevelSelectData.SetMapData(map);
		LevelSelectData.ThemeData = m_themesList.ThemesData[m_themeDropdown.value];
		LevelSelectData.FileName = LevelEditorData.CustomMapFileName;

		LevelSelectData.GameMode = (EGameMode)m_gameModeDropdown.value;
		LevelSelectData.TurnDirection = (ETurnDirection)m_turnDirectionDropdown.value;
		LevelEditorData.StartAtSecondSpawnPoint = m_startAtSecondSpawnToggle.isOn;
		LevelEditorData.AllowMoveThroughWalls = m_moveThroughWallsToggle.isOn;

		SceneManager.LoadScene("LevelScene");
	}
	#endregion
}
