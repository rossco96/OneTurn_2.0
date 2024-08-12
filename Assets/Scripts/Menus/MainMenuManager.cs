using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_mainMenuParent;
	[SerializeField] private GameObject m_levelSelectParent;
	[SerializeField] private GameObject m_levelEditorPopupParent;

	[SerializeField] private TMPro.TextMeshProUGUI m_demoVersion;

	private void Awake()
	{
		LoadMenu();

		// [NOTE] The below is repeated in a coupe of places! Figure out where and how actually needed!
		// Here, LevelSelectMenuManager, and LevelEditor (prev. LevelEditorMenuManager)

		m_demoVersion.text = $"Version: {Application.version}";
	}

	private void LoadMenu()
	{
		if (LevelSelectData.IsInGame)
		{
			m_mainMenuParent.SetActive(false);
			m_levelSelectParent.SetActive(true);
		}
		else if (LevelEditorData.IsTestingLevel)
		{
			m_levelEditorPopupParent.SetActive(true);
		}
	}
}
