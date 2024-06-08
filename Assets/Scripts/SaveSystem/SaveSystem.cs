using System;
using System.IO;
using UnityEngine;

// [TODO][Refactor] Child classes - for map, mapmeta, and stat

public static class SaveSystem
{
	#region Vars
	private static readonly string m_initFullFilePath = $"{Application.persistentDataPath}/{Application.productName}.init";
	private static readonly string m_gameMapsDirectory = $"{Application.persistentDataPath}/Maps/Game";
	private static readonly string m_customMapsDirectory = $"{Application.persistentDataPath}/Maps/Custom";
	private static readonly string m_importedMapsDirectory = $"{Application.persistentDataPath}/Maps/Imported";

	private static readonly string m_mapExtension = "map";									// [TODO] Change this to "png"
	private static readonly string m_mapMetaExtension = "mapmeta";
	private static readonly string m_mapStatsExtension = "stat";

	private static Texture2D TEST_texture;													// [Q] This works! Go with this over custom .map (text) file?
	private static string m_mapFullFilePath = string.Empty;
	private static string m_mapFileContents = string.Empty;
	#endregion


	#region Init / Version Check
	public static void Init()																// [TODO][IMPORTANT] Ensure this is being called from GameStartup or Splash script!
	{
		InitCustomImportDirectories();														// [TODO] delete delete delete delete delete delete delete
		string currentVersion = Application.version;
		
		if (File.Exists(m_initFullFilePath))
		{
			string text = File.ReadAllText(m_initFullFilePath);
			// If it's the same version, no new levels (etc.)
			if (text == currentVersion)
				return;
			File.SetAttributes(m_initFullFilePath, FileAttributes.Normal);
		}
		else
		{
			InitCustomImportDirectories();
		}

		StreamWriter writer = File.CreateText(m_initFullFilePath);
		writer.Write(currentVersion);
		writer.Dispose();
		File.SetAttributes(m_initFullFilePath, FileAttributes.ReadOnly);
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
		Debug.Log($"????? [{m_customMapsDirectory}] [{m_importedMapsDirectory}]");
		Directory.CreateDirectory(m_customMapsDirectory);
		Directory.CreateDirectory(m_importedMapsDirectory);
	}
	#endregion


	#region Custom Map Files
	public static void CreateCustomMapFile(string mapFileName, int gridDimensions)
	{
		TEST_texture = new Texture2D(gridDimensions, gridDimensions);
		m_mapFileContents = string.Empty;
		m_mapFullFilePath = $"{m_customMapsDirectory}/{mapFileName}.{m_mapExtension}";
		if (File.Exists(m_mapFullFilePath))
		{
			// [TODO][IMPORTANT] Make sure to backup the file first!? Just in case! Can overwrite backups in the same session.
			//File.SetAttributes(m_mapFullFilePath, FileAttributes.Normal);
		}



		// [TODO]	This method!
		// [Q]		Can we save the sprite in Application.persistentDataPath? Rather than converting to a text file.

		/*
		int width = m_sprite.texture.width;
		int height = m_sprite.texture.height;
		string spriteString = string.Empty;

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				Color color = m_sprite.texture.GetPixel(x, y);
				string colorString = color.ToString();
				colorString = colorString.Replace("RGBA(", string.Empty);
				colorString = colorString.Replace(")", string.Empty);
				spriteString += colorString + ";";
			}
			spriteString = spriteString.Remove(spriteString.Length - 1);
			spriteString += "\n";
		}
		spriteString = spriteString.Remove(spriteString.Length - 1);
		spriteString = spriteString.Replace(" ", string.Empty);

		Debug.Log($"{System.DateTime.Now.Ticks}");
		Debug.Log($"{System.IO.Path.GetRandomFileName()}");

		return;

		//string mapPath = $"C:\\Users\\rossc\\Documents\\TEMP\\{m_sprite.texture.imageContentsHash}.map";
		string mapPath = $"{Application.persistentDataPath}\\{m_sprite.texture.imageContentsHash}.map";
		Debug.Log(mapPath);
		if (System.IO.File.Exists(mapPath))
			System.IO.File.SetAttributes(mapPath, System.IO.FileAttributes.Normal);
		System.IO.StreamWriter mapWriter = System.IO.File.CreateText(mapPath);
		mapWriter.Write(spriteString);
		mapWriter.Close();
		System.IO.File.SetAttributes(mapPath, System.IO.FileAttributes.ReadOnly);
		//System.IO.File.SetAttributes(mapPath, System.IO.FileAttributes.Hidden);
		//*/
	}

	public static void AddToCustomMapFile(Color color, bool endOfRow, int x, int y)
	{
		string colorString = color.ToString();
		colorString = colorString.Replace("RGBA(", string.Empty);
		colorString = colorString.Replace(")", string.Empty);
		m_mapFileContents += colorString;
		m_mapFileContents += (endOfRow) ? "\n" : ";";

		TEST_texture.SetPixel(x, y, color);
	}

	public static void SaveCustomMapFile()
	{
		m_mapFileContents = m_mapFileContents.Replace(" ", "");								// [NOTE] Not really needed I guess? Saves minimal space
		m_mapFileContents = m_mapFileContents.Substring(0, m_mapFileContents.Length - 1);	// Remove final \n character
		if (File.Exists(m_mapFullFilePath))
		{
			// [TODO][IMPORTANT] Make sure to backup the file first!? Just in case! Can overwrite backups in the same session.
			File.SetAttributes(m_mapFullFilePath, FileAttributes.Normal);
		}
		StreamWriter writer = File.CreateText(m_mapFullFilePath);
		writer.Write(m_mapFileContents);
		writer.Dispose();
		File.SetAttributes(m_mapFullFilePath, FileAttributes.ReadOnly);
		//File.SetAttributes(m_mapFullFilePath, FileAttributes.Hidden);

		//FileStream pngWriter = File.Create($"{m_customMapsDirectory}/abcd1234.png");
		FileStream pngWriter = new FileStream($"{m_customMapsDirectory}/abcd1234.png", FileMode.Create, FileAccess.Write, FileShare.None);
		//while (pngWriter.)
		pngWriter.Write(TEST_texture.EncodeToPNG(), 0, TEST_texture.EncodeToPNG().Length);
		pngWriter.Dispose();
	}
	#endregion


	#region Mapmeta Files
	// [TODO][IMPORTANT] Implement this similar to Custom Map Files region above!
	public static string CreateCustomMapmetaFile()
	{
		//string fileName = Path.GetRandomFileName();										// [TODO] Commented out for testing! Use this!!!
		string fileName = "abcd1234";
		string fullPath = $"{m_customMapsDirectory}/{fileName}.{m_mapMetaExtension}";

		MapmetaData<string, string> data = new MapmetaData<string, string>();
		data.Add($"{EMapmetaInfo.CreationTime}", $"{DateTime.Now.Ticks}");
		data.Add($"{EMapmetaInfo.UpdatedTime}", $"{DateTime.Now.Ticks}");
		data.Add($"{EMapmetaInfo.MapName}", "Test_Map_01");
		data.Add($"{EMapmetaInfo.AuthorName}", "Rocco");

		//for (int i = 0; i < Enum.GetValues(typeof(EMapmetaInfo)).Length; ++i)
		//{
		//	data.Add($"{(EMapmetaInfo)i}", );
		//}

		string dataJson = JsonUtility.ToJson(data);

		StreamWriter metaWriter = File.CreateText(fullPath);
		metaWriter.Write(dataJson);
		metaWriter.Close();
		File.SetAttributes(fullPath, FileAttributes.ReadOnly);

		return fileName;
	}

	public static void AddToCustomMapmetaFile()
	{

	}

	public static void SaveCustomMapmetaFile()
	{

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
	// [TODO] GetStatData , GetMetaData
	// [Q] Also need GetMapData? Or if implementing via saving the sprite itself, GetMap?
	public static string[] GetCustomMapmetaFilepaths()
	{
		string[] allFiles = Directory.GetFiles(m_customMapsDirectory);
		string[] mapmetaFiles = new string[0];
		for (int i = 0; i < allFiles.Length; ++i)
		{
			if (allFiles[i].Contains(m_mapMetaExtension))
				mapmetaFiles = mapmetaFiles.Add(allFiles[i]);
		}
		return mapmetaFiles;
	}

	public static MapmetaData<string, string> GetMapmetaContents(string filepath)
	{
		string mapmetaFile = File.ReadAllText(filepath);
		return JsonUtility.FromJson<MapmetaData<string, string>>(mapmetaFile);
	}


	public static string[] GetImportedMapmetaFilepaths()
	{
		return new string[0];
	}
	#endregion
}
