using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData_", menuName = "Data/Save System/Settings Data (float)")]
public class SettingsDataFloat : SettingsData_Base
{
	[SerializeField] private float DefaultValue;
	public override string GetDefaultValueAsString() => $"{DefaultValue}";
}
