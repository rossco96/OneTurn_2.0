using UnityEngine;
using UnityEngine.UI;

public class SettingsObjectToggle : SettingsObject_Base
{
	[SerializeField] private Toggle m_toggle;

	public override void ApplyExistingValue()
	{
		string valueString = SettingsSystem.GetValue(m_data.Key);
		m_toggle.isOn = bool.Parse(valueString);
	}

	public override void SetNewValue()
	{
		SettingsSystem.UpdateSettings(m_data.Key, $"{m_toggle.isOn}");
	}
}
