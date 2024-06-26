using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData_", menuName = "Data/Save System/Settings Data (bool)")]
public class SettingsDataBool : SettingsData_Base
{
	[SerializeField] private bool DefaultValue;
	public override string GetDefaultValueAsString() => $"{DefaultValue}";
}
