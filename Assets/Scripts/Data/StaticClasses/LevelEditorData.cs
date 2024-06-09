using UnityEngine;

public static class LevelEditorData
{
	public static Texture2D GridTexture;

	public static string CustomMapFullFilepath = string.Empty;
	public static bool LoadExistingLevel = false;
	public static int GridDimension = 9;

	public static bool IsTestingLevel = false;
	public static EGameMode GameMode = EGameMode.Items;
	public static ETurnDirection TurnDirection = ETurnDirection.Right;

	public static bool StartAtSecondSpawnPoint = false;						// [Q] Allow as a 'cheat' for single player as well?
	public static bool AllowMoveThroughWalls = false;						// [Q] Implement? If so, need to distinguish between Walls and Borders
}