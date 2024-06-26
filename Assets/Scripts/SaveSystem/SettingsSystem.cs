using System.IO;
using UnityEngine;

public static class SettingsSystem
{
	#region Vars
	private static readonly string m_settingsFullFilepath = $"{Application.persistentDataPath}/{Application.productName}.settings";
	
	// [TODO] Test with this as only contains 4 items. Make sure we're updating if new settings exist.
	//private static readonly string m_settingsFullFilepath = $"{Application.persistentDataPath}/settings.path";

	private static SDictionary<string, string> m_currentSettings = new SDictionary<string, string>();
	private static SDictionary<string, string> m_updatedSettings = new SDictionary<string, string>();

	private static bool m_settingsDataDirty = false;
	#endregion


	#region INIT
	public static void InitAllSettings(SettingsData_Base[] saveDatas)
	{
		if (UpdateRequired(saveDatas) == false)
			return;

		SDictionary<string, string> gameStartupSettingsData = new SDictionary<string, string>();
		for (int i = 0; i < saveDatas.Length; ++i)
		{
			gameStartupSettingsData.Add(saveDatas[i].Key, saveDatas[i].GetDefaultValueAsString());                          // Are the different classes all pointless if needing to convert to float or int anyways?
		}

		if (File.Exists(m_settingsFullFilepath))
		{
			// Only add the new data which didn't previously exist
			string settingsFile = File.ReadAllText(m_settingsFullFilepath);
			SDictionary<string, string> jsonSettingsData = JsonUtility.FromJson<SDictionary<string, string>>(settingsFile);
			
			for (int i = 0; i < saveDatas.Length; ++i)
			{
				if (jsonSettingsData.ContainsKey(saveDatas[i].Key) == false)
					jsonSettingsData.Add(saveDatas[i].Key, saveDatas[i].GetDefaultValueAsString());                         // Are the different classes all pointless if needing to convert to float or int anyways?
			}
			// [NOTE] If ever deleting settings, will need to add code here to handle that!

			for (int j = 0; j < jsonSettingsData.Count; ++j)
			{
				m_currentSettings.Add(jsonSettingsData.GetKeyAtIndex(j), jsonSettingsData.GetValueAtIndex(j));
				m_updatedSettings.Add(jsonSettingsData.GetKeyAtIndex(j), jsonSettingsData.GetValueAtIndex(j));
			}
			string dataJson = JsonUtility.ToJson(jsonSettingsData);

			File.SetAttributes(m_settingsFullFilepath, FileAttributes.Normal);
			StreamWriter writer = File.CreateText(m_settingsFullFilepath);
			writer.Write(dataJson);
			writer.Close();
			File.SetAttributes(m_settingsFullFilepath, FileAttributes.ReadOnly);
			//File.SetAttributes(m_settingsFullFilepath, FileAttributes.Hidden);
		}
		else
		{
			string dataJson = JsonUtility.ToJson(gameStartupSettingsData);
			// Create the file!
			StreamWriter writer = File.CreateText(m_settingsFullFilepath);
			writer.Write(dataJson);
			writer.Dispose();
			File.SetAttributes(m_settingsFullFilepath, FileAttributes.ReadOnly);
			//File.SetAttributes(m_settingsFullFilepath, FileAttributes.Hidden);
		}
	}

	private static bool UpdateRequired(SettingsData_Base[] saveDatas)
	{
		if (File.Exists(m_settingsFullFilepath))
		{
			string settingsFile = File.ReadAllText(m_settingsFullFilepath);
			SDictionary<string, string> jsonSettingsData = JsonUtility.FromJson<SDictionary<string, string>>(settingsFile);
			if (jsonSettingsData.Count == saveDatas.Length)
			{
				for (int i = 0; i < jsonSettingsData.Count; ++i)
				{
					m_currentSettings.Add(jsonSettingsData.GetKeyAtIndex(i), jsonSettingsData.GetValueAtIndex(i));
					m_updatedSettings.Add(jsonSettingsData.GetKeyAtIndex(i), jsonSettingsData.GetValueAtIndex(i));
				}
				return false;
			}
		}
		return true;
	}
	#endregion


	#region SET / GET
	public static string GetValue(string key) => m_currentSettings[key];

	public static void UpdateSettings(string key, string newValue)
	{
		if (m_updatedSettings[key] == newValue) return;
		m_updatedSettings[key] = newValue;
		m_settingsDataDirty = true;
	}

	public static bool UnsavedData() => m_settingsDataDirty;

	public static void SaveSettings()
	{
		string dataJson = JsonUtility.ToJson(m_updatedSettings);
		File.SetAttributes(m_settingsFullFilepath, FileAttributes.Normal);
		StreamWriter writer = File.CreateText(m_settingsFullFilepath);
		writer.Write(dataJson);
		writer.Close();
		File.SetAttributes(m_settingsFullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(m_settingsFullFilepath, FileAttributes.Hidden);
		m_currentSettings = m_updatedSettings;
		m_settingsDataDirty = false;
	}

	public static void DiscardSettings()
	{
		m_updatedSettings = m_currentSettings;
		m_settingsDataDirty = false;
	}
	#endregion
}
