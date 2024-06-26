using UnityEngine;
using UnityEngine.UI;

public class SettingsObjectSlider : SettingsObject_Base
{
	[SerializeField] private Slider m_slider;

	public override void ApplyExistingValue()
	{
		string valueString = SettingsSystem.GetValue(m_data.Key);
		m_slider.value = float.Parse(valueString);
	}

	public override void SetNewValue()
	{
		SettingsSystem.UpdateSettings(m_data.Key, $"{m_slider.value}");
	}
}
