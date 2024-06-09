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
	public static void CreateCustomMapFile(string mapFileName/*, int gridDimension*/)
	{
		// [NOTE] *Creating* a new map file is always 9x9
		// Decide if we're using this method for *Updating* a map file as well...
		// ... In which case, will want to pass gridDimensions!
		//m_customMapTexture = new Texture2D(gridDimension, gridDimension);
		m_customMapTexture = new Texture2D(9, 9);											// [TODO] Store as const k_initialGridDimension ?
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

		return m_mapmetaFullFilepath;
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
	private static void GenerateStatFile(string filePath)
	{
		StatsData<StatKey<string, string, string>, float> statsData = new StatsData<StatKey<string, string, string>, float>();
		string[] gameModes = Enum.GetNames(typeof(EGameMode));
		string[] turnDirection = Enum.GetNames(typeof(ETurnDirection));
		string[] statsSection = Enum.GetNames(typeof(EStatsSection));

		for (int a = 0; a < gameModes.Length; ++a)
		{
			if ((EGameMode)a == EGameMode.M_Bomb || (EGameMode)a == EGameMode.M_Chase) continue;
			string mode = gameModes[a];
			for (int b = 0; b < turnDirection.Length; ++b)
			{
				string direction = turnDirection[b];
				for (int c = 0; c < statsSection.Length; ++c)
				{
					if ((EGameMode)a == EGameMode.Exit && (EStatsSection)c == EStatsSection.Items) continue;
					string stat = statsSection[c];
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
	public static Texture2D GetCustomMapTexture(string mapmetaFullFilepath)
	{
		int gridDimension = int.Parse(GetMapmetaInfo(mapmetaFullFilepath, EMapmetaInfo.GridDimension));
		string mapFilepath = mapmetaFullFilepath.Replace($".{m_mapMetaExtension}", $".{m_mapExtension}");

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

	// [TODO] GetStatData , GetMetaData
	// [Q] Also need GetMapData? Or if implementing via saving the sprite itself, GetMap?
	public static string[] GetCustomMapmetaFilepaths()
	{
		string[] allFiles = Directory.GetFiles(m_customMapsDirectory);
		string[] mapmetaFiles = new string[0];
		for (int i = 0; i < allFiles.Length; ++i)
		{
			if (allFiles[i].Contains($".{m_mapMetaExtension}"))
				mapmetaFiles = mapmetaFiles.Add(allFiles[i]);
		}
		return mapmetaFiles;
	}

	public static MapmetaData<string, string> GetMapmetaContents(string fullFilepath)
	{
		string mapmetaFile = File.ReadAllText(fullFilepath);
		return JsonUtility.FromJson<MapmetaData<string, string>>(mapmetaFile);
	}

	public static string GetMapmetaInfo(string fullFilepath, EMapmetaInfo infoType)
	{
		MapmetaData<string, string> mapmetaInfo = GetMapmetaContents(fullFilepath);
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


	public static string[] GetImportedMapmetaFilepaths()
	{
		return new string[0];
	}
	#endregion
}
