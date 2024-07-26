using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData_", menuName = "Data/Theme Data (Base)")]
public class ThemeData : ScriptableObject
{
	public string ThemeName;
	public MapData[] Maps;

	public LevelPlayInfo LevelPlayInfo;

	// Can we not just create LevelSpriteInfo SO and put here? Will have same problem accessing info regardless... Right? Difference will be negligible

	public Sprite BackgroundSprite;
	public Sprite PlayerSprite;
	public Sprite WallSprite;               // [TODO] Eventually want different sprites for straight, corner, T, and + walls. AND A LONE SQUARE! Create struct.
	public Sprite ItemSprite;
	public Sprite ExitSprite;
}
