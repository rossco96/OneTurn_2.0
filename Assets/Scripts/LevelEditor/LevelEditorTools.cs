public static class LevelEditorTools
{
	public static int GetGridDimensionFromSliderValue(int sliderValue)
	{
		return (sliderValue * 2) + 7;
	}


	public static int GetItemsFromSliderValue(int sliderValue)
	{
		return (sliderValue * sliderValue) + sliderValue + 6;
	}

	public static int GetTimeLimitFromSliderValue(int sliderValue)
	{
		if (sliderValue == 1)
		{
			return 40;
		}
		return (30 * sliderValue);
	}


	public static int GetItemsFromGridDimension(int gridDimension)
	{
		int scaledValue = (gridDimension - 7) / 2;
		return (scaledValue * scaledValue) + scaledValue + 6;
	}

	public static int GetTimeLimitFromGridDimension(int gridDimension)
	{
		if (gridDimension == 9)
		{
			return 40;
		}
		return (15 * gridDimension) - 105;
	}

	// [TODO] Feels hacky replacing "/" and "\\", but it's needed and it works?
	public static string FullPathToCustomFilename(string fullFilepath, string customMapsDirectory)
	{
		string fileName = fullFilepath.Replace($"{customMapsDirectory}", "");
		fileName = fileName.Replace("/", "");
		fileName = fileName.Replace("\\", "");
		int extensionIndex = fileName.LastIndexOf(".");
		return fileName.Substring(0, extensionIndex);
	}

	//public string GetTextureHash(Texture2D texture)
	//{
	//	return $"{Hash128.Compute(texture.EncodeToPNG())}";
	//}
}
