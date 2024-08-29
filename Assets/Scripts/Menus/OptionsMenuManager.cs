using TMPro;
using UnityEngine;

public class OptionsMenuManager : MonoBehaviour
{
	[SerializeField] private ThemesList m_gameThemesList;

	[Space]
	[SerializeField] private TMP_Dropdown m_inputModeDropdown;
	[SerializeField] private TutorialPopup m_tutorialPopup;



	private void OnEnable()
	{
		InitInputModeDropdown();
		m_tutorialPopup.TryShow();
	}



	private void InitInputModeDropdown()
	{
		m_inputModeDropdown.ClearOptions();
		System.Collections.Generic.List<string> inputModes = new System.Collections.Generic.List<string>();
		for (int i = 0; i < System.Enum.GetValues(typeof(EInputMode)).Length; ++i)
		{
			string inputMode = $"{(EInputMode)i}";
			if (inputMode.CanFormatCamelCase(out string inputModeCamelCase))
				inputModes.Add(inputModeCamelCase);
			else
				inputModes.Add(inputMode);
		}
		m_inputModeDropdown.AddOptions(inputModes);
	}



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



	public void ResetScoresGame()
	{
		SaveSystem.ResetAllStatFilesGame(m_gameThemesList);
	}
}
