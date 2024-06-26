using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData_", menuName = "Data/Save System/Settings Data (string)")]
public class SettingsDataString : SettingsData_Base
{
	[SerializeField] private string DefaultValue;
	public override string GetDefaultValueAsString() => DefaultValue;
}
