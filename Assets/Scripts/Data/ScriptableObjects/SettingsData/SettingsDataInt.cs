using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData_", menuName = "Data/Save System/Settings Data (int)")]
public class SettingsDataInt : SettingsData_Base
{
	[SerializeField] private int DefaultValue;
	public override string GetDefaultValueAsString() => $"{DefaultValue}";
}
