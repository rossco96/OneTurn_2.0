using TMPro;
using UnityEngine;
using UnityEngine.UI;

// [TODO] Refactor this into separate classes -- Grid, Tool, Tutorial, Metadata, Other Menu Buttons

public class LevelEditor : MonoBehaviour
{
	[SerializeField] private LevelGeneratorColorData m_colorData;

	[Space]
	[SerializeField] private TextMeshProUGUI m_sliderLabel;
	[SerializeField] private GridLayoutGroup m_gridParent;
	[SerializeField] private GameObject[] m_gridRows;

	[Space]
	[SerializeField] private TMP_Dropdown m_toolsDropdown;
	//[SerializeField] private Image m_toolImage;
	//[SerializeField] private TextMeshProUGUI m_toolLabel;

	private string m_levelFileName;                                                             // [TODO] Implement!

	private int m_gridDimensions = 9;
	private ELevelGeneratorColorName m_currentTool = ELevelGeneratorColorName.BlankSquare;


	private void Awake()
	{
		InitGridButtons();
		InitToolsDropdown();

		//m_levelFileName = LevelSelectData.CustomData.											// [TODO] Implement!
	}


	#region Tools Dropdown
	private void InitToolsDropdown()
	{
		System.Collections.Generic.List<string> dropdownOptions = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(ELevelGeneratorColorName)).Length; ++i)
		{
			// [TODO][Q] Also add sprites? Show sprites next to it with the name of the tool?
			if ((ELevelGeneratorColorName)i == ELevelGeneratorColorName.Special) continue;
			string option = $"{(ELevelGeneratorColorName)i}";
			if (option.CanFormatCamelCase(out string optionFormat))
				dropdownOptions.Add(optionFormat);
			else
				dropdownOptions.Add(option);
		}
		m_toolsDropdown.AddOptions(dropdownOptions);
		m_toolsDropdown.value = 0;
	}

	public void OnToolChanged(TMP_Dropdown dropdown)
	{
		m_currentTool = (ELevelGeneratorColorName)dropdown.value;
		// [TODO] Update text (and image?) for the chosen tool
	}
	#endregion


	#region Grid Buttons
	private void InitGridButtons()
	{
		for (int i = 0; i < m_gridRows.Length; ++i)
		{
			GridButton[] gridButtons = m_gridRows[i].GetComponentsInChildren<GridButton>(true);
			for (int j = 0; j < gridButtons.Length; ++j)
			{
				GridButton gb = gridButtons[j];
				//gb.SetGridCoordinate(j, i);										// Has to be j,i way around!
				gb.RegisterOnButtonSelected(OnGridButtonClicked);
				if (LevelEditorData.LoadExistingLevel)
				{

				}
				else
				{
					gb.SetPropertyColor(m_colorData.GetColorByName(ELevelGeneratorColorName.BlankSquare));
				}
			}
		}
	}

	public void OnGridButtonClicked(GridButton gb)
	{
		gb.SetPropertyColor(m_colorData.GetColorByName(m_currentTool));
	}

	public void UpdateGridLayout(Slider slider)
	{
		m_gridDimensions = ((int)slider.value * 2) + 7;
		SetNewGridSize();
		m_sliderLabel.text = $"Size: {m_gridDimensions}x{m_gridDimensions}";
	}

	private void SetNewGridSize()
	{
		// [TODO] There is still some space to play around with on the edges. Unsure why!
		int buttonSize = Screen.width / m_gridDimensions;
		m_gridParent.cellSize = 2 * new Vector2(buttonSize * m_gridDimensions, buttonSize);
		for (int i = 0; i < m_gridRows.Length; ++i)
		{
			GameObject row = m_gridRows[i];
			row.SetActive(i < m_gridDimensions);
			if (i < m_gridDimensions)
			{
				GridLayoutGroup layoutGroup = row.GetComponent<GridLayoutGroup>();
				layoutGroup.cellSize = 2 * new Vector2(buttonSize, buttonSize);
				GridButton[] gridButtons = row.GetComponentsInChildren<GridButton>(true);
				for (int j = 0; j < gridButtons.Length; ++j)
				{
					gridButtons[j].gameObject.SetActive(j < m_gridDimensions);
				}
			}
		}
	}
	#endregion


	#region Menu Buttons
	public void TestLevel()
	{
		// Call Save() before testing! Then load LevelScene...
		Save();
		MapData mapData = new MapData();
		//mapData.GridLayout = 
		LevelSelectData.SetMapData(mapData);
		//LevelSelectData.SetTestMode(true);							// Implement, then set this to false upon reaching the MainMenu scene? How best guarantee with fewest calls?
	}
	
	public void Save()
	{
		SaveSystem.CreateCustomMapFile(LevelEditorData.ExistingFileName, m_gridDimensions);	
	}
	#endregion


	public void TEST_GenerateCustomMapFile()
	{
		SaveSystem.CreateCustomMapFile("abcd1234", m_gridDimensions);
		
		for (int i = 0; i < m_gridDimensions; ++i)
		{
			GridButton[] gridButtons = m_gridRows[i].GetComponentsInChildren<GridButton>(true);
			for (int j = 0; j < m_gridDimensions; ++j)
			{
				GridButton gb = gridButtons[j];
				SaveSystem.AddToCustomMapFile
				(
					gb.PropertyColor,
					j == m_gridDimensions - 1,
					j,																		// NOTE THIS MAY END UP AN UPSIDE DOWN IMAGE
					m_gridDimensions - i - 1												// ... if image creation works at all
				);
			}
		}

		SaveSystem.SaveCustomMapFile();
	}
}
