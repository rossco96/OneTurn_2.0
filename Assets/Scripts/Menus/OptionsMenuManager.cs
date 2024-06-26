using UnityEngine;

public class OptionsMenuManager : MonoBehaviour
{
	public void SaveSettings()
	{
		// [TODO] Disable or hide button if no settings changed
		SettingsSystem.SaveSettings();
	}

	public void MainMenu()
	{
		// Show 'Unsaved Data' popup, if applicable
		// Then have RETURN and SAVE and CANCEL buttons on said popup
	}

	public void TEMP_DiscardSettings()
	{
		SettingsSystem.DiscardSettings();
	}
}
