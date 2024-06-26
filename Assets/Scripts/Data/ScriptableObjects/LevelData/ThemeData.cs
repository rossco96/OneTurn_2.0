using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData_", menuName = "Data/Theme Data")]
public class ThemeData : ScriptableObject
{
	public string ThemeName;
	public MapData[] Maps;

	public int PointsToUnlock;
	public LevelPlayInfo LevelPlayInfo;

	// Do NOT set true on first release - only set true on any updates.
	// (either new themes or new maps within an existing theme)
	// ('new' is only removed once any level is played once. Do not put 'new' for any individual levels?)
	// NOTE if a theme has a 'completed' tick over it, the 'IsNew' will override that (obviously, since it is no longer complete)
	// [TODO] CANNOT DO THIS ON SCRIPTABLE OBJECT! MOVE ELSEWHERE!
	public bool IsNew;
	public bool IsComplete;

	public Sprite LevelSelectIcon;
	public Sprite BackgroundSprite;

	public Sprite PlayerSprite;
	public Sprite WallSprite;               // [TODO] Eventually want different sprites for straight, corner, T, and + walls. AND A LONE SQUARE! Create struct.
	public Sprite ItemSprite;
	public Sprite ExitSprite;

	public bool IsSpecialLevel;
	public Sprite SpecialSprite;
	public SpecialLevel_Base SpecialLevelFunction;
}
