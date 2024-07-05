using TMPro;
using UnityEngine;

public class SettingsObjectDropdown : SettingsObject_Base
{
	[SerializeField] private TMP_Dropdown m_dropdown;

	public override void ApplyExistingValue()
	{
		// Also not ideal here! Again, are we always populating dropdowns with camelcase? and want it going back to no-spaces
		string valueString = (SettingsSystem.GetValue(m_data.Key).CanFormatCamelCase(out string camelCaseString))
			? camelCaseString
			: SettingsSystem.GetValue(m_data.Key);
		for (int i = 0; i < m_dropdown.options.Count; ++i)
		{
			if (m_dropdown.options[i].text == valueString)
			{
				m_dropdown.value = i;
				return;
			}
		}
	}

	public override void SetNewValue()
	{
		// [TODO] Will we ever NOT want this? Regardless, pls rename these two camel case extensions
		string optionNoSpaces = m_dropdown.options[m_dropdown.value].text.FromCamelCase();
		SettingsSystem.UpdateSettings(m_data.Key, optionNoSpaces);
	}
}
