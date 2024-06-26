using TMPro;
using UnityEngine;

public class SettingsObjectDropdown : SettingsObject_Base
{
	[SerializeField] private TMP_Dropdown m_dropdown;

	public override void ApplyExistingValue()
	{
		string valueString = SettingsSystem.GetValue(m_data.Key);
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
		SettingsSystem.UpdateSettings(m_data.Key, m_dropdown.itemText.text);
	}
}
