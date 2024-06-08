using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData_", menuName = "Data/Theme Data")]
public class ThemeData : ScriptableObject
{
	public string ThemeName;
	public MapData[] Maps;

	public int PointsToUnlock;
	public int TimeLimit;

	public Sprite LevelSelectIcon;
	public Sprite BackgroundSprite;

	public Sprite PlayerSprite;
	public Sprite WallSprite;               // [TODO] Eventually want different sprites for straight, corner, T, and + walls
	public Sprite ItemSprite;
	public Sprite ExitSprite;

	public bool IsSpecialLevel;
	public Sprite SpecialSprite;
	public SpecialLevel_Base SpecialLevelFunction;
}
