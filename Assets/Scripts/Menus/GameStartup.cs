using UnityEngine;

public class GameStartup : MonoBehaviour
{
	[SerializeField] private SettingsData_Base[] m_settingsKeys;

	private void Awake()
	{
		SaveSystem.Init();
		SettingsSystem.InitAllSettings(m_settingsKeys);
		PlayerPrefsSystem.InitAllPrefs();

		// [TODO] Call to SplashAnimator? Or just call in Awake on there?
	}
}
