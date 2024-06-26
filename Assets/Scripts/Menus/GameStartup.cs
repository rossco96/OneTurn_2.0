using UnityEngine;

public class GameStartup : MonoBehaviour
{
	[SerializeField] private SettingsData_Base[] m_settingsKeys;
	//[SerializeField] private SaveData_Base[] m_prefsKeys;						// [Q] Still do PlayerPrefsSystem via script?

	[SerializeField] private GameObject m_splashScreenParent;
	[SerializeField] private GameObject m_mainMenuParent;
	[SerializeField] private GameObject m_levelSelectParent;
	[SerializeField] private GameObject m_levelEditorPopupParent;

	//[SerializeField] private SplashAnimator m_splashAnimator;

	private void Awake()
	{
		SaveSystem.Init();
		SettingsSystem.InitAllSettings(m_settingsKeys);
		PlayerPrefsSystem.InitAllPrefs();

		// [TODO] NO LONGER NEED THE BELOW! PUT INTO ITS OWN SPLASH SCREEN
		/*
		if (PlayerPrefs.GetInt(k_playerPrefsStartupKey) == 0)
		{
			//m_splashAnimator.AddOnCompleteListener(LoadMenu);					// TEMP COMMENTED OUT
			//m_splashAnimator.Start();											// 
			PlayerPrefs.SetInt(k_playerPrefsStartupKey, 1);
			LoadMenu();															// [TODO] TEMP HERE UNTIL SplashAnimator setup as above!
		}
		else
		{
			LoadMenu();
		}
		//*/
	}

	// [TODO] CREATE MainMenuManager AND PUT IN THERE (anything else that class would need to do or nah?)
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
