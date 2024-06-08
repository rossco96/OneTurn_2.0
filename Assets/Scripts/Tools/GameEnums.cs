public enum EFacingDirection
{
	Up,
	Right,
	Down,
	Left
}

[System.Serializable]
public enum EGameMode
{
	Items,
	Exit,
	M_Bomb,
	M_Chase
}

[System.Serializable]
public enum ETurnDirection
{
	Right,
	Left
}

public enum EInputMode
{
	TapSwipe,
	Buttons
}

// [TODO] Consider renaming this! As used within the level editor as well. Perhaps EMapBlockType?
[System.Serializable]
public enum ELevelGeneratorColorName
{
	BlankSquare,
	Wall,
	Item,
	Exit,
	SpawnPointPrimary,
	SpawnPointSecondary,
	Special
}

public enum EStatsSection
{
	Score,
	Lives,
	Time,
	Moves,
	Items
}

public enum EMapmetaInfo
{
	CreationTime,
	UpdatedTime,
	AuthorName,
	MapName,
	Description,				// This is optional!
	GridDimension
}
