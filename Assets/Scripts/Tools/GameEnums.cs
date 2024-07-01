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
	//Travel,						// New game mode idea! Cover every (possible?) square in the time limit. Multiplayer version you have to turn all or as many of the squares to your colour
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
	SwipeDirectional,
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

// [TODO] Should really consider separate classes for best practice. And for some of the above, too.
public enum ECheatType
{
	UnlimitedLives,
	UnlimitedTime,
	ExitAnyDirection,
	WallTravel,
	SecondarySpawn,				// Have this as an option rather than a cheat?
	PlayerVFX					// Add unnecessary animations/particles upon moving
}

public enum EMultiplayerResult
{
	P1,
	P2,
	Draw
}
