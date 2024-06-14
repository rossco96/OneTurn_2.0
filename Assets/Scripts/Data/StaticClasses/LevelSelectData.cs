public static class LevelSelectData
{
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

	public static EGameMode GameMode = EGameMode.Items;
	public static ETurnDirection TurnDirection = ETurnDirection.Right;

	public static int LivesCount = 3;										// [TODO] Implement properly!

	public static bool IsMultiplayer = false;

	//public static bool IsCustomMap = false;								// [Q] Do we need this?

	public static int GridDimension = 9;
	public static float GridSizeMultiplier = 0.0f;
}