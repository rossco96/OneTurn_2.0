public static class LevelSelectData
{
    private static ThemeData m_themeData;
	public static ThemeData ThemeData => m_themeData;
	private static MapData m_mapData;
	public static MapData MapData => m_mapData;

	private static EGameMode m_gameMode = EGameMode.Items;
	public static EGameMode GameMode => m_gameMode;
	private static ETurnDirection m_turnDirection = ETurnDirection.Right;
	public static ETurnDirection TurnDirection => m_turnDirection;

	private static int m_livesCount = 3;
	public static int LivesCount => m_livesCount;

	private static bool m_isMultiplayer = false;
	public static bool IsMultiplayer => m_isMultiplayer;

	private static bool m_isCustomMap = false;
	public static bool IsCustomMap => m_isCustomMap;

	private static int m_gridDimension = 9;
	public static int GridDimension => m_gridDimension;
	private static float m_gridSizeMultiplier = 0.0f;
	public static float GridSizeMultiplier => m_gridSizeMultiplier;


	public static void SetIsCustomMap(bool isCustomMap)
	{
		m_isCustomMap = isCustomMap;
	}


	public static void SetThemeData(ThemeData themeData)
	{
		m_themeData = themeData;
	}
	public static void SetMapData(MapData mapData)
	{
		m_mapData = mapData;
		m_gridDimension = m_mapData.GridLayout.width;
		// [NOTE][IMPORTANT] 0.2f at the end is because the border is *CURRENTLY* 1/10th the width of the walls (multiplied by two lots of borders, one each side of the screen)
		// [TODO][Q] Make sure we're calculating the difference?? Ask an artist about import settings??
		// Border should always be the same size, regardless of grid dimnsion!
		m_gridSizeMultiplier = (UnityEngine.Camera.main.aspect * UnityEngine.Camera.main.orthographicSize * 2.0f) / (m_gridDimension + 0.2f);
	}


	public static void SetGameMode(EGameMode gameMode)
	{
		m_gameMode = gameMode;
	}
	public static void SetTurnDirection(ETurnDirection turnDirection)
	{
		m_turnDirection = turnDirection;
	}

	public static void SetIsMultiplayer(bool isMultiplayer)
	{
		m_isMultiplayer = isMultiplayer;
	}


	public static void SetLivesCount(int lives)
	{
		// [TODO] IMPLEMENT PROPERLY!
		m_livesCount = 3;
	}
}