using UnityEngine;

public class GameStartup : MonoBehaviour
{
	[SerializeField] private SettingsData_Base[] m_settingsKeys;
	[SerializeField] private ThemesList m_themesList;

	[Space]
	[SerializeField] private SettingsDataString m_gameModeSetting;

	private void Awake()
	{
		SaveSystem.Init(m_themesList);
		SettingsSystem.InitAllSettings(m_settingsKeys);
		PlayerPrefsSystem.InitAllPrefs();

		// [Q] Have const k_multiplayer = "M_"?
		// [Q] If so, move that (and others?) to their own global const file?
		if (SettingsSystem.GetValue(m_gameModeSetting.Key).StartsWith("M_"))
		{
			SettingsSystem.UpdateSettings(m_gameModeSetting.Key, $"{EGameMode.Items}");
			SettingsSystem.SaveSettings();
		}

		// [TODO] Call to SplashAnimator? Or just call in Awake on there?
	}
}
