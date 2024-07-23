public static class LevelSelectData
{
	public static string FileName = string.Empty;
	public static EMapType MapType = EMapType.Game;

	public static ThemeData ThemeData;
	public static MapData MapData;
	public static void SetMapData(MapData mapData)
	{
		MapData = mapData;
		GridDimension = MapData.GridLayout.width;
		// [NOTE][IMPORTANT] 0.2f at the end is because the border is *CURRENTLY* 1/10th the width of the walls (multiplied by two lots of borders, one each side of the screen)
		// [TODO][Q] Make sure we're calculating the difference?? Ask an artist about import settings??
		// Border should always be the same size, regardless of grid dimnsion!
		GridSizeMultiplier = (UnityEngine.Camera.main.aspect * UnityEngine.Camera.main.orthographicSize * 2.0f) / (GridDimension + 0.2f);
	}
	public static int ChosenMapIndex;

	public static EGameMode GameMode = EGameMode.Items;
	public static ETurnDirection TurnDirection = ETurnDirection.Right;

	public static int LivesCount = 3;										// [TODO] Implement properly!

	public static bool IsMultiplayer = false;
	// [TODO] This is needed for deciding which end level popups to show and if we're adding to stats and change of buttons etc.
	public static bool IsCustomMap = false;
	public static bool IsInGame = false;

	public static int GridDimension = 9;
	public static float GridSizeMultiplier = 0.0f;

	public static bool ChaseIsRoundTwo = false;
	public static int ChaseStatsP1Lives = 0;
	public static int ChaseStatsP1Moves = 0;
	public static float ChaseStatsP1Time = 0.0f;
}