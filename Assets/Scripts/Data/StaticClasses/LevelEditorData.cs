using UnityEngine;

public static class LevelEditorData
{
	public static Texture2D GridTexture;

	public static string CustomMapFileName = string.Empty;
	public static bool LoadExistingLevel = false;

	public static bool IsTestingLevel = false;								// [Q] Need this anymore? Rename to LevelEditorOverride?
	
	public static bool StartAtSecondSpawnPoint = false;						// [Q] Allow as a 'cheat' for single player as well?
	public static bool AllowMoveThroughWalls = false;

	public static void ResetGridTexture(int gridDimension)
	{
		GridTexture = new Texture2D(gridDimension, gridDimension);
	}
	public static void AddToGridTexture(Color color, int x, int y)
	{
		GridTexture.SetPixel(x, y, color);
	}
}