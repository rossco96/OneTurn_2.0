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
	_LevelEditor,
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

[System.Serializable]
public enum EMapPropertyName
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
