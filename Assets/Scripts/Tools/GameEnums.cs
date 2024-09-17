public enum EFacingDirection
{
	// IMPORTANT - keep this as a 'clockface' (going clockwise, starting from 'up'
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
	// [TODO][IMPORTANT] Need to create new save data -- cannot load for Travel as it didn't exist when creating the save system!
	Items,
	Exit,
	Travel,						// New game mode idea! Cover every (possible?) square in the time limit. Multiplayer version you have to turn all or as many of the squares to your colour
	M_Bomb,
	M_Chase//,
	//M_Tanks						// New game mode idea! Like the classic Tank game. Could do 1 shot per move, 3 or 5 lives, either reset or not upon being hit
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

public enum EMovement
{
	Forward,
	Turn
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
	Items,
	Area
}

public enum EMapmetaInfo
{
	CreationTime,
	UpdatedTime,
	AuthorName,
	MapName,
	Description,				// This is optional!
	GridDimension,
	Theme
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

// If implementing a "3x max travel" feature, then can include here too?
// Idea: could be worth more points if travelled to multiple times, but on the fourth time it'll turn into a wall (removed from total score and makes level more difficult)
// [TODO] Merge (and rename) the above and below enums
public enum ETravelSquareState
{
	NONE,
	P1,
	P2
}

public enum EBridgePiece
{
	Single,
	End,
	Mid,
	Corner,
	T,
	Cross
}
