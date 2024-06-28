using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_mainMenuParent;
	[SerializeField] private GameObject m_levelSelectParent;
	[SerializeField] private GameObject m_levelEditorPopupParent;

	private void Awake()
	{
		LoadMenu();
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
