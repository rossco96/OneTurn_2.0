using System;
using System.IO;
using UnityEngine;

// [TODO][Refactor] Child classes - for map, mapmeta, and stat

public static class SaveSystem
{
	#region Vars
	private static readonly string m_initFullFilepath = $"{Application.persistentDataPath}/{Application.productName}.init";
	//private static readonly string m_settingsFullFilepath = $"{Application.persistentDataPath}/Settings.info";	// [TODO] Implement - store e.g. volume, input type, previously played level, etc.
	private static readonly string m_gameMapsDirectory = $"{Application.persistentDataPath}/Maps/Game";
	private static readonly string m_customMapsDirectory = $"{Application.persistentDataPath}/Maps/Custom";
	private static readonly string m_importedMapsDirectory = $"{Application.persistentDataPath}/Maps/Imported";

	private static readonly string m_mapExtension = "png";
	private static readonly string m_mapMetaExtension = "mapmeta";
	private static readonly string m_mapStatsExtension = "stat";

	private static readonly string m_multiplayerGameModePrefix = "M_";					// [TODO] Move as is also used elsewhere... Or make public

	
	private static string m_mapFullFilepath = string.Empty;
	private static string m_mapmetaFullFilepath = string.Empty;
	
	private static Texture2D m_customMapTexture;
	private static MapmetaData<string, string> m_customMapmetaInfo;
	#endregion


	#region Init / Version Check
	public static void Init()																// [TODO][IMPORTANT] Ensure this is being called from GameStartup or Splash script!
	{
		string currentVersion = Application.version;
		
		if (File.Exists(m_initFullFilepath))
		{
			string text = File.ReadAllText(m_initFullFilepath);
			// If it's the same version, no new levels (etc.)
			// [TODO][IMPORTANT] This may not be the only info in the Init file! Checking "text == currentVersion" may not be appropriate!
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
				string completeFilePath = $"{themeDirectory}/{Hash128.Compute(themeMaps[j].GridLayout.EncodeToPNG())}.{m_mapStatsExtension}";
				if (File.Exists(completeFilePath) == false)
					CreateStatFile($"{completeFilePath}");
			}
		}
	}

	// [TODO][IMPORTANT][Q] Do we also need to call Directory.CreateDirectory(m_gameMapsDirectory) ?
	// --> If so, also rename the method!
	private static void InitCustomImportDirectories()
	{
		//Directory.CreateDirectory(m_gameMapsDirectory);
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

	public static void AddToNewCustomMapmetaFile(EMapmetaInfo infoType, string value)
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

	public static void UpdateExistingMapmetaFile(string mapFileName, EMapmetaInfo infoType, string value)
	{
		string fullFilepath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapMetaExtension}";
		string mapmetaFile = File.ReadAllText(fullFilepath);
		MapmetaData<string, string> mapmetaInfo = JsonUtility.FromJson<MapmetaData<string, string>>(mapmetaFile);
		mapmetaInfo[$"{infoType}"] = value;
		string dataJson = JsonUtility.ToJson(mapmetaInfo);

		File.SetAttributes(fullFilepath, FileAttributes.Normal);
		StreamWriter metaWriter = File.CreateText(fullFilepath);
		metaWriter.Write(dataJson);
		metaWriter.Close();
		File.SetAttributes(fullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(m_mapmetaFullFilepath, FileAttributes.Hidden);
	}
	#endregion


	#region Stat Files
	public static void CreateStatFileCustomMap(string mapFileName)
	{
		string fullFilepath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapStatsExtension}";
		CreateStatFile(fullFilepath);
	}

	public static void CreateStatFileImportedMap(string mapFileName)
	{
		string fullFilepath = $"{m_importedMapsDirectory}/{mapFileName}.{m_mapStatsExtension}";
		CreateStatFile(fullFilepath);
	}

	private static void CreateStatFile(string fullFilepath)
	{
		StatsDictionary statsData = new StatsDictionary();

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
					statsData.Add($"{mode}{direction}{stat}", 0);
				}
			}
		}

		string jsonData = JsonUtility.ToJson(statsData);
		if (File.Exists(fullFilepath))
			File.SetAttributes(fullFilepath, FileAttributes.Normal);
		StreamWriter dictWriter = File.CreateText(fullFilepath);
		dictWriter.Write(jsonData);
		dictWriter.Close();
		File.SetAttributes(fullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(fullFilepath, FileAttributes.Hidden);
	}

	public static bool StatFileSaveRequired(float time, int lives, int moves, int items = -1)
	{
		string statFullFilepath = string.Empty;
		switch (LevelSelectData.MapType)
		{
			case EMapType.Game:
				statFullFilepath = $"{m_gameMapsDirectory}/{LevelSelectData.ThemeData.ThemeName}/{LevelSelectData.FileName}.{m_mapStatsExtension}";
				break;
			case EMapType.Custom:
				statFullFilepath = $"{m_customMapsDirectory}/{LevelSelectData.FileName}.{m_mapStatsExtension}";
				break;
			case EMapType.Imported:
				// [TODO][IMPORTANT] This will need changing if wanting to categorise by author!!!
				statFullFilepath = $"{m_importedMapsDirectory}/{LevelSelectData.FileName}.{m_mapStatsExtension}";
				break;
			default:
				break;
		}

		string statFile = File.ReadAllText(statFullFilepath);
		StatsDictionary statsData = JsonUtility.FromJson<StatsDictionary>(statFile);
		string statBase = $"{LevelSelectData.GameMode}{LevelSelectData.TurnDirection}";

		// [Q] Is this the order of priorities for what classes as a more successful level? Figure out in testing!
		// (changed to putting TIME last since very rarely will come down to the exact same time, so all other comparisons would be pointless)
		if (items >= 0)
		{
			// ITEMS game mode
			if (items > statsData[$"{statBase}{EStatsSection.Items}"])		return true;
			else if (items < statsData[$"{statBase}{EStatsSection.Items}"])	return false;
			
			else if (lives > statsData[$"{statBase}{EStatsSection.Lives}"])	return true;
			else if (lives < statsData[$"{statBase}{EStatsSection.Lives}"])	return false;
			
			else if (statsData[$"{statBase}{EStatsSection.Moves}"] == 0 ||
				moves < statsData[$"{statBase}{EStatsSection.Moves}"])		return true;
			else if (moves > statsData[$"{statBase}{EStatsSection.Moves}"])	return false;
			
			else if (statsData[$"{statBase}{EStatsSection.Time}"] == 0 ||
				time < statsData[$"{statBase}{EStatsSection.Time}"])		return true;
			else if (time > statsData[$"{statBase}{EStatsSection.Time}"])	return false;
		}
		else
		{
			// EXIT game mode
			if (lives > statsData[$"{statBase}{EStatsSection.Lives}"])		return true;
			else if (lives < statsData[$"{statBase}{EStatsSection.Lives}"])	return false;
			
			else if (statsData[$"{statBase}{EStatsSection.Moves}"] == 0 ||
				moves < statsData[$"{statBase}{EStatsSection.Moves}"])		return true;
			else if (moves > statsData[$"{statBase}{EStatsSection.Moves}"])	return false;
			
			else if (statsData[$"{statBase}{EStatsSection.Time}"] == 0 ||
				time < statsData[$"{statBase}{EStatsSection.Time}"])		return true;
			else if (time > statsData[$"{statBase}{EStatsSection.Time}"])	return false;
		}

		// [TODO] IMPLEMENT CHECKING, PRIORITY OF DIFFERENT THINGS
		// e.g. for exit mode: first number of lives, then amount of time, then number of moves
		// Currently always writing to the save file...
		return true;
	}

	public static void SaveStatFileInfo(int score, float time, int lives, int moves, int items = -1)
	{
		string statFullFilepath = string.Empty;
		switch (LevelSelectData.MapType)
		{
			case EMapType.Game:
				statFullFilepath = $"{m_gameMapsDirectory}/{LevelSelectData.ThemeData.ThemeName}/{LevelSelectData.FileName}.{m_mapStatsExtension}";
				break;
			case EMapType.Custom:
				statFullFilepath = $"{m_customMapsDirectory}/{LevelSelectData.FileName}.{m_mapStatsExtension}";
				break;
			case EMapType.Imported:
				// [TODO][IMPORTANT] This will need changing if wanting to categorise by author!!!
				statFullFilepath = $"{m_importedMapsDirectory}/{LevelSelectData.FileName}.{m_mapStatsExtension}";
				break;
			default:
				break;
		}

		string statFile = File.ReadAllText(statFullFilepath);
		StatsDictionary statsData = JsonUtility.FromJson<StatsDictionary>(statFile);
		string statBase = $"{LevelSelectData.GameMode}{LevelSelectData.TurnDirection}";

		statsData[$"{statBase}{EStatsSection.Score}"] = score;
		statsData[$"{statBase}{EStatsSection.Time}"] = time.RoundDP(2);
		statsData[$"{statBase}{EStatsSection.Lives}"] = lives;
		statsData[$"{statBase}{EStatsSection.Moves}"] = moves;

		if (items >= 0)
			statsData[$"{statBase}{EStatsSection.Items}"] = items;

		string jsonData = JsonUtility.ToJson(statsData);
		File.SetAttributes(statFullFilepath, FileAttributes.Normal);
		StreamWriter dictWriter = File.CreateText(statFullFilepath);
		dictWriter.Write(jsonData);
		dictWriter.Close();
		File.SetAttributes(statFullFilepath, FileAttributes.ReadOnly);
		//File.SetAttributes(statFullFilepath, FileAttributes.Hidden);
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
		// [TODO][IMPORTANT] This should need changing! LevelSelectData.MapType, not just m_customMapsDirectory
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


	public static float GetStatsInfo(string mapFileName, EStatsSection statType)
	{
		string fullFilepath = string.Empty;
		switch (LevelSelectData.MapType)
		{
			case EMapType.Game:
				fullFilepath += $"{m_gameMapsDirectory}/{LevelSelectData.ThemeData.ThemeName}/";
				break;
			case EMapType.Custom:
				fullFilepath += $"{m_customMapsDirectory}/";
				break;
			case EMapType.Imported:
				fullFilepath += $"{m_importedMapsDirectory}/";
				break;
			default:
				break;
		}
		fullFilepath += $"{mapFileName}.{m_mapStatsExtension}";
		string statsFile = File.ReadAllText(fullFilepath);
		MapmetaData<string, float> mapStats = JsonUtility.FromJson<MapmetaData<string, float>>(statsFile);
		return mapStats[$"{LevelSelectData.GameMode}{LevelSelectData.TurnDirection}{statType}"];
	}

	public static int GetTotalPoints(ThemesList themesList)
	{
		int totalPoints = 0;

		for (int i = 0; i < themesList.ThemesData.Length; ++i)
		{
			string[] allThemeFiles = Directory.GetFiles($"{m_gameMapsDirectory}/{themesList.ThemesData[i].ThemeName}");
			for (int j = 0; j < allThemeFiles.Length; ++j)
			{
				if (allThemeFiles[j].Contains($".{m_mapStatsExtension}"))
				{
					string statFile = File.ReadAllText(allThemeFiles[j]);
					MapmetaData<string, float> statsInfo = JsonUtility.FromJson<MapmetaData<string, float>>(statFile);

					for (int gm = 0; gm < Enum.GetValues(typeof(EGameMode)).Length; ++gm)
					{
						if ($"{(EGameMode)gm}".StartsWith(m_multiplayerGameModePrefix)) continue;
						for (int td = 0; td < Enum.GetValues(typeof(ETurnDirection)).Length; ++td)
						{
							totalPoints += (int)statsInfo[$"{(EGameMode)gm}{(ETurnDirection)td}{(EStatsSection.Score)}"];
						}
					}
				}
			}
		}

		return totalPoints;
	}


	// [TODO] Implement!
	public static string[] GetImportedMapFileNamesByAuthorName()
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

	//public string GetTextureHash(Texture2D texture)
	//{
	//	return $"{Hash128.Compute(texture.EncodeToPNG())}";
	//}
	#endregion
}
