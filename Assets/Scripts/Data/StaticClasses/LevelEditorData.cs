using UnityEngine;

public static class LevelEditorData
{
	public static Texture2D GridTexture;

	public static EGameMode GameMode = EGameMode.Items;
	public static ETurnDirection TurnDirection = ETurnDirection.Right;

	public static bool IsTestingLevel = false;
	public static bool LoadExistingLevel = false;
	public static string ExistingFileName = string.Empty;

	public static bool StartAtSecondSpawnPoint = false;
	public static bool AllowMoveThroughWalls = false;						// Implement? If so, need to distinguish between Walls and Borders
}