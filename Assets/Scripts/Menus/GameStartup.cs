using UnityEngine;

public class GameStartup : MonoBehaviour
{
	[SerializeField] private SettingsData_Base[] m_settingsKeys;
	[SerializeField] private ThemesList m_themesList;

	private void Awake()
	{
		SaveSystem.Init(m_themesList);
		SettingsSystem.InitAllSettings(m_settingsKeys);
		PlayerPrefsSystem.InitAllPrefs();

		// [TODO] Call to SplashAnimator? Or just call in Awake on there?
	}
}
