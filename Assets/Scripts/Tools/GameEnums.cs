public enum EFacingDirection
{
	Up,
	Right,
	Down,
	Left
}

public enum EMapType
{
	Game,
	Custom,
	Imported
}

[System.Serializable]
public enum EGameMode
{
	Items,
	Exit,
	//Travel,						// New game mode idea! Cover every (possible?) square in the time limit.
	M_Bomb,
	M_Chase//,
	// Think of a similar multiplayer to travel where you have to turn all the squares to your colour
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
