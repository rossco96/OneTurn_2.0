using UnityEngine;

public class TEST_SaveDataCode : MonoBehaviour
{
	[SerializeField] private Sprite m_sprite;

	[SerializeField] private SpriteRenderer m_targetSpriteRenderer;

	[SerializeField] private string m_author;
	[SerializeField] private string m_name;



    [ContextMenu("Generate Color Text File")]
	public void GenerateColorTextFile()
	{
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

		string metaPath = $"C:\\Users\\rossc\\Documents\\TEMP\\{m_sprite.texture.imageContentsHash}.mapmeta";
		if (System.IO.File.Exists(metaPath))
			System.IO.File.SetAttributes(metaPath, System.IO.FileAttributes.Normal);
		System.IO.StreamWriter metaWriter = System.IO.File.CreateText(metaPath);
		metaWriter.Write($"[Author,{m_author}],[Name,{m_name}],[Date,{System.DateTime.Now.Ticks}]");
		metaWriter.Close();
		System.IO.File.SetAttributes(metaPath, System.IO.FileAttributes.ReadOnly);
		//System.IO.File.SetAttributes(metaPath, System.IO.FileAttributes.Hidden);

		//System.DateTime dt = new System.DateTime(System.DateTime.Now.Ticks);						// [NOTE] Do this if wanting to display creation date! Want also last edited date?
	}

	[ContextMenu("Create Sprite From Text File")]
	public void CreateSpriteFromTextFile()
	{
		string textFile = System.IO.File.ReadAllText($"C:\\Users\\rossc\\Documents\\TEMP\\{m_sprite.texture.imageContentsHash}.map");
		string[] textFileArray = textFile.Split('\n');
		int dimension = textFileArray.Length;
		// [NOTE] Can just do textFileArray.Length for both, since only ever dealing with square grids.
		// [IMPORTANT] Will have to change if going rectangular, then no other grid shapes (e.g. L) are possible with this method!
		// (though could just do a larger grid and wall off one corner) (same for e.g. a triangular shape)
		string[,] textFileGrid = new string[dimension, dimension];
		for (int i = 0; i < dimension; ++i)
		{
			string[] currentRowString = textFileArray[i].Split(';');
			for (int j = 0; j < dimension; ++j)
			{
				textFileGrid[i, j] = currentRowString[j];
			}
		}

		Texture2D texture = new Texture2D(dimension, dimension);
		texture.filterMode = FilterMode.Point;
		for (int x = 0; x < dimension; ++x)
		{
			for (int y = 0; y < dimension; ++y)
			{
				string[] colorStringArray = textFileGrid[x, y].Split(',');
				string r = colorStringArray[0];
				string g = colorStringArray[1];
				string b = colorStringArray[2];
				string a = colorStringArray[3];
				Color setColor = new Color(float.Parse(r), float.Parse(g), float.Parse(b), float.Parse(a));
				texture.SetPixel(x, y, setColor);
			}
		}
		texture.Apply();
		Vector2 textureVector2 = new Vector2(dimension, dimension);

		Rect rect = new Rect(Vector2.zero, textureVector2);
		rect.center = 0.5f * textureVector2;
		Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

		m_targetSpriteRenderer.sprite = sprite;
	}



	[ContextMenu("Generate SAV File")]
	public void GenerateSaveFile()
	{
		StatsData<StatKey<string, string, string>, float> statsData = new StatsData<StatKey<string, string, string>, float>();

		string[] gameModes = System.Enum.GetNames(typeof(EGameMode));
		string[] turnDirection = System.Enum.GetNames(typeof(ETurnDirection));
		string[] statsSection = System.Enum.GetNames(typeof(EStatsSection));

		for (int a = 0; a < gameModes.Length; ++a)
		{
			if (a >= 2) continue;
			string mode = gameModes[a];
			for (int b = 0; b < turnDirection.Length; ++b)
			{
				string direction = turnDirection[b];
				for (int c = 0; c < statsSection.Length; ++c)
				{
					if (mode == $"{EGameMode.Exit}" && statsSection[c] == $"{EStatsSection.Items}") continue;
					string stat = statsSection[c];
					StatKey<string, string, string> statKey = new StatKey<string, string, string>(mode, direction, stat);
					if (a == 0 && b == 1)
					{
						if (c == 0) statsData.Add(statKey, 1234);
						else if (c == 1) statsData.Add(statKey, 3);
						else if (c == 2) statsData.Add(statKey, 43.21f);
						else if (c == 3) statsData.Add(statKey, 50);
						else statsData.Add(statKey, 9);
					}
					else
						statsData.Add(statKey, 0);
				}
			}
		}

		string dictjson = JsonUtility.ToJson(statsData);
		string statPath = $"C:\\Users\\rossc\\Documents\\TEMP\\{m_sprite.texture.imageContentsHash}.stat";
		if (System.IO.File.Exists(statPath))
			System.IO.File.SetAttributes(statPath, System.IO.FileAttributes.Normal);
		System.IO.StreamWriter dictWriter = System.IO.File.CreateText(statPath);
		dictWriter.Write(dictjson);
		dictWriter.Close();
		System.IO.File.SetAttributes(statPath, System.IO.FileAttributes.ReadOnly);
		//System.IO.File.SetAttributes(statPath, System.IO.FileAttributes.Hidden);
	}

	[ContextMenu("DEBUG (view save file)")]
	public void DebugViewSaveFile()
	{
		string s = System.IO.File.ReadAllText($"C:\\Users\\rossc\\Documents\\TEMP\\{m_sprite.texture.imageContentsHash}.stat");

		// [NOTE][IMPORTANT] This throws an error but still functions! Error below...
		StatsData<StatKey<string, string, string>, float> saveData = JsonUtility.FromJson<StatsData<StatKey<string, string, string>, float>>(s);

		// ArgumentException: An item with the same key has already been added. Key: (, , )
		// System.Collections.Generic.Dictionary`2[TKey, TValue].TryInsert(TKey key, TValue value, System.Collections.Generic.InsertionBehavior behavior)(at < 695d1cc93cca45069c528c15c9fdd749 >:0)
		// System.Collections.Generic.Dictionary`2[TKey, TValue].Add(TKey key, TValue value)(at < 695d1cc93cca45069c528c15c9fdd749 >:0)
		// SaveData`2[k, v].OnAfterDeserialize()(at Assets / Scripts / Tools / SaveData.cs:33)
		// UnityEngine.JsonUtility:FromJson(String)
		// TEST_SaveDataCode: DebugViewSaveFile()(at Assets / Scripts / Archive / TEST_SaveDataCode.cs:148)

		StatKey<string, string, string> dataKey = new StatKey<string, string, string>($"{EGameMode.Items}", $"{ETurnDirection.Right}", $"{EStatsSection.Score}");		// Value = 0
		float dataValue = saveData.GetValue(dataKey);
		Debug.Log($"{dataValue}");
		
		dataKey = new StatKey<string, string, string>($"{EGameMode.Exit}", $"{ETurnDirection.Left}", $"{EStatsSection.Moves}");											// Value = 0
		dataValue = saveData.GetValue(dataKey);
		Debug.Log($"{dataValue}");
		
		dataKey = new StatKey<string, string, string>($"{EGameMode.Items}", $"{ETurnDirection.Left}", $"{EStatsSection.Score}");										// Value = 1234
		dataValue = saveData.GetValue(dataKey);
		Debug.Log($"{dataValue}");
		
		dataKey = new StatKey<string, string, string>($"{EGameMode.Items}", $"{ETurnDirection.Left}", $"{EStatsSection.Time}");											// Value = 43.21
		dataValue = saveData.GetValue(dataKey);
		Debug.Log($"{dataValue}");
	}




	[ContextMenu("Debug -- InitSaveSystem", false, 1000)]
	private void InitSaveSystem()
	{
		SaveSystem.Init();
	}

	[ContextMenu("Debug -- GenerateMapFile", false, 1000)]
	private void GenerateMapFile()
	{
		//SaveSystem.TESTGenerateMapFile("");
	}
}
