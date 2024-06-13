using System;
using System.IO;
using UnityEngine;

// [TODO][Refactor] Child classes - for map, mapmeta, and stat

public static class SaveSystem
{
	#region Vars
	private static readonly string m_initFullFilepath = $"{Application.persistentDataPath}/{Application.productName}.init";
	private static readonly string m_gameMapsDirectory = $"{Application.persistentDataPath}/Maps/Game";
	private static readonly string m_customMapsDirectory = $"{Application.persistentDataPath}/Maps/Custom";
	private static readonly string m_importedMapsDirectory = $"{Application.persistentDataPath}/Maps/Imported";

	private static readonly string m_mapExtension = "png";
	private static readonly string m_mapMetaExtension = "mapmeta";
	private static readonly string m_mapStatsExtension = "stat";

	private static readonly string m_multiplayerGameModePrefix = "M_";

	
	private static string m_mapFullFilepath = string.Empty;
	private static string m_mapmetaFullFilepath = string.Empty;
	
	private static Texture2D m_customMapTexture;
	private static MapmetaData<string, string> m_customMapmetaInfo;
	#endregion


	#region Init / Version Check
	public static void Init()																// [TODO][IMPORTANT] Ensure this is being called from GameStartup or Splash script!
	{
		InitCustomImportDirectories();														// [TODO] delete delete delete delete delete delete delete
		string currentVersion = Application.version;
		
		if (File.Exists(m_initFullFilepath))
		{
			string text = File.ReadAllText(m_initFullFilepath);
			// If it's the same version, no new levels (etc.)
			if (text == currentVersion)
				return;
			File.SetAttributes(m_initFullFilepath, FileAttributes.Normal);
		}
		else
		{
			InitCustomImportDirectories();
		}

		StreamWriter writer = File.CreateText(m_initFullFilepath);
		writer.Write(currentVersion);
		writer.Dispose();
		File.SetAttributes(m_initFullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(statPath, FileAttributes.Hidden);

		InitNewLevels();
	}

	private static void InitNewLevels()
	{
		ThemesList themesList = Resources.Load<ThemesList>("ThemesList");                     // [TODO] Super important! Move this to a folder called Resources!
		ThemeData[] themeData = themesList.ThemesData;

		for (int i = 0; i < themeData.Length; ++i)
		{
			ThemeData currentTheme = themeData[i];
			string themeDirectory = $"{m_gameMapsDirectory}/{currentTheme.ThemeName}";
			if (Directory.Exists(themeDirectory) == false)
				Directory.CreateDirectory(themeDirectory);

			MapData[] themeMaps = currentTheme.Maps;
			for (int j = 0; j < themeMaps.Length; ++j)
			{
				string completeFilePath = $"{themeDirectory}/{themeMaps[j].GridLayout.imageContentsHash}.{m_mapStatsExtension}";
				if (File.Exists(completeFilePath) == false)
					GenerateStatFile(completeFilePath);
			}
		}
	}

	private static void InitCustomImportDirectories()
	{
		Directory.CreateDirectory(m_customMapsDirectory);
		Directory.CreateDirectory(m_importedMapsDirectory);
	}
	#endregion


	#region Custom Map Files
	public static void CreateCustomMapFile(string mapFileName, int gridDimension = 9)
	{
		m_customMapTexture = new Texture2D(gridDimension, gridDimension);
		m_mapFullFilepath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapExtension}";
		if (File.Exists(m_mapFullFilepath))
		{
			// [TODO][IMPORTANT] Make sure to backup the file first!? Just in case! Can overwrite backups in the same session.
			//File.SetAttributes(m_mapFullFilePath, FileAttributes.Normal);
		}
	}

	public static void AddToCustomMapFile(Color color, int x, int y)
	{
		m_customMapTexture.SetPixel(x, y, color);
	}

	public static void SaveCustomMapFile()
	{
		if (File.Exists(m_mapFullFilepath))
		{
			// [TODO][IMPORTANT] Make sure to backup the file first!? Just in case! Can overwrite backups in the same session.
			File.SetAttributes(m_mapFullFilepath, FileAttributes.Normal);
		}
		FileStream wrtier = new FileStream(m_mapFullFilepath, FileMode.Create, FileAccess.Write, FileShare.None);
		m_customMapTexture.filterMode = FilterMode.Point;
		wrtier.Write(m_customMapTexture.EncodeToPNG(), 0, m_customMapTexture.EncodeToPNG().Length);
		wrtier.Dispose();
		File.SetAttributes(m_mapFullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(m_mapFullFilePath, FileAttributes.Hidden);
	}
	#endregion


	#region Mapmeta Files
	// [TODO][IMPORTANT] Implement this similar to Custom Map Files region above!
	public static string CreateCustomMapmetaFile()
	{
		string randomFileName = Path.GetRandomFileName();
		m_mapmetaFullFilepath = $"{m_customMapsDirectory}/{randomFileName}.{m_mapMetaExtension}";
		
		m_customMapmetaInfo = new MapmetaData<string, string>();
		m_customMapmetaInfo.Add($"{EMapmetaInfo.CreationTime}", $"{DateTime.Now.Ticks}");
		m_customMapmetaInfo.Add($"{EMapmetaInfo.UpdatedTime}", $"{DateTime.Now.Ticks}");
		m_customMapmetaInfo.Add($"{EMapmetaInfo.GridDimension}", "9");					// All new levels initialised as a 9x9 grid

		// [TODO] Delete this!
		// Well, move it to the UPDATE section
		if (File.Exists(m_mapmetaFullFilepath))
		{
			// [TODO] Make sure to backup the file first!? Just in case! Can overwrite backups in the same session.
			File.SetAttributes(m_mapmetaFullFilepath, FileAttributes.Normal);
		}
		// ^^^ ^^^ ^^^

		return randomFileName;
	}

	public static void AddToCustomMapmetaFile(EMapmetaInfo infoType, string value)
	{
		m_customMapmetaInfo.Add($"{infoType}", value);
	}

	public static void SaveCustomMapmetaFile()
	{
		string dataJson = JsonUtility.ToJson(m_customMapmetaInfo);
		StreamWriter metaWriter = File.CreateText(m_mapmetaFullFilepath);
		metaWriter.Write(dataJson);
		metaWriter.Close();
		File.SetAttributes(m_mapmetaFullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(m_mapmetaFullFilepath, FileAttributes.Hidden);
	}
	#endregion


	#region Stat Files
	public static void CreateCustomStatFile(string mapFileName)
	{
		string mapFilePath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapExtension}";
		GenerateStatFile(mapFilePath);
	}

	private static void GenerateStatFile(string filePath)
	{
		StatsData<StatKey<string, string, string>, float> statsData = new StatsData<StatKey<string, string, string>, float>();
		string[] gameModes = Enum.GetNames(typeof(EGameMode));
		string[] turnDirection = Enum.GetNames(typeof(ETurnDirection));
		string[] statsSection = Enum.GetNames(typeof(EStatsSection));

		for (int gameModeIndex = 0; gameModeIndex < gameModes.Length; ++gameModeIndex)
		{
			if ($"{(EGameMode)gameModeIndex}".StartsWith(m_multiplayerGameModePrefix)) continue;
			string mode = gameModes[gameModeIndex];
			for (int turnDirectionIndex = 0; turnDirectionIndex < turnDirection.Length; ++turnDirectionIndex)
			{
				string direction = turnDirection[turnDirectionIndex];
				for (int statsSectionIndex = 0; statsSectionIndex < statsSection.Length; ++statsSectionIndex)
				{
					if ((EGameMode)gameModeIndex == EGameMode.Exit && (EStatsSection)statsSectionIndex == EStatsSection.Items) continue;
					string stat = statsSection[statsSectionIndex];
					StatKey<string, string, string> statKey = new StatKey<string, string, string>(mode, direction, stat);
					statsData.Add(statKey, 0);
				}
			}
		}

		string jsonData = JsonUtility.ToJson(statsData);
		if (File.Exists(filePath))
			File.SetAttributes(filePath, FileAttributes.Normal);
		StreamWriter dictWriter = File.CreateText(filePath);
		dictWriter.Write(jsonData);
		dictWriter.Close();
		File.SetAttributes(filePath, FileAttributes.ReadOnly);
		//File.SetAttributes(filePath, FileAttributes.Hidden);
	}
	#endregion


	#region Get Data
	public static string[] GetCustomMapFileNamesByCreationTime()
	{
		string[] allFiles = Directory.GetFiles(m_customMapsDirectory);
		string[] fileNames = new string[0];
		long[] fileCreationTimes = new long[0];
		
		for (int i = 0; i < allFiles.Length; ++i)
		{
			if (allFiles[i].Contains($".{m_mapMetaExtension}"))
			{
				string fileName = FullPathToCustomFileName(allFiles[i]);
				fileNames = fileNames.Add(fileName);

				string mapmetaFile = File.ReadAllText(allFiles[i]);
				MapmetaData<string, string> mapmetaInfo = JsonUtility.FromJson<MapmetaData<string, string>>(mapmetaFile);
				long creationTime = long.Parse(mapmetaInfo[$"{EMapmetaInfo.CreationTime}"]);
				fileCreationTimes = fileCreationTimes.Add(creationTime);
			}
		}

		int[] creationTimesOrder = fileCreationTimes.RetrieveListOrder();
		string[] orderedFileNames = new string[fileNames.Length];

		for (int i = 0; i < fileNames.Length; ++i)
		{
			int orderIndex = creationTimesOrder[i];
			orderedFileNames[i] = fileNames[orderIndex];
		}

		return orderedFileNames;
	}


	public static Texture2D GetCustomMapTexture(string mapFileName)
	{
		string mapFilepath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapExtension}";

		int gridDimension = int.Parse(GetMapmetaInfo(mapFileName, EMapmetaInfo.GridDimension));
		Texture2D texture = new Texture2D(gridDimension, gridDimension);		
		FileStream reader = new FileStream(mapFilepath, FileMode.Open, FileAccess.Read, FileShare.None);

		byte[] bytes = new byte[reader.Length];
		int bytesToRead = (int)reader.Length;
		int bytesRead = 0;
		while (bytesToRead > 0)
		{
			int n = reader.Read(bytes, bytesRead, bytesToRead);
			if (n == 0) break;
			bytesRead += n;
			bytesToRead -= n;
		}

		reader.Dispose();
		texture.LoadImage(bytes);
		texture.filterMode = FilterMode.Point;
		return texture;
	}


	public static string GetMapmetaInfo(string mapFileName, EMapmetaInfo infoType)
	{
		string fullFilepath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapMetaExtension}";
		string mapmetaFile = File.ReadAllText(fullFilepath);
		MapmetaData<string, string> mapmetaInfo = JsonUtility.FromJson<MapmetaData<string, string>>(mapmetaFile);

		string infoString = mapmetaInfo[$"{infoType}"];
		switch (infoType)
		{
			case EMapmetaInfo.CreationTime:
			case EMapmetaInfo.UpdatedTime:
				long ticks = long.Parse(infoString);
				DateTime dateTime = new DateTime(ticks);
				infoString = dateTime.ToShortDateString();
				break;
			default:
				break;
		}

		return infoString;
	}


	// [TODO] Implement!
	public static string[] GetImportedMapmetaFilepaths()
	{
		return new string[0];
	}
	#endregion


	#region Tools
	// [TODO] Feels hacky replacing "/" and "\\", but it's needed and it works?
	private static string FullPathToCustomFileName(string fullFilepath)
	{
		string fileName = fullFilepath.Replace($"{m_customMapsDirectory}", "");
		fileName = fileName.Replace("/", "");
		fileName = fileName.Replace("\\", "");
		int extensionIndex = fileName.LastIndexOf(".");
		return fileName.Substring(0, extensionIndex);
	}
	#endregion
}
