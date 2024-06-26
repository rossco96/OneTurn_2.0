using UnityEngine;

public abstract class SettingsData_Base : ScriptableObject
{
	public string Key;
	public abstract string GetDefaultValueAsString();
}
