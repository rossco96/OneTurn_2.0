using UnityEngine;

[CreateAssetMenu(fileName = "MapData_", menuName = "Data/Map Data")]
public class MapData : ScriptableObject
{
	// [TODO] THIS IS ONLY CORRECT FOR GAME MAPS! MUST FIGURE OUT HOW TO DO THIS IN GENERAL!
	public string FileName => $"{Hash128.Compute(GridLayout.EncodeToPNG())}";

	public Texture2D GridLayout;
	
	// [NOTE] The below feels somewhat hacky but, if generating using pixel grids, how else to do it?
	// (same with exit direction)
	public EFacingDirection[] PlayerSpawnDirectionRight;
	public EFacingDirection[] PlayerSpawnDirectionLeft;

	// [TODO] Turn these into arrays too, as above, in case of special levels with multiple exits
	// [NOTE] WHEN CONSIDERING THE ORDER OF THE ARRAY, the level generator reads vertically from bottom left to top right
	public EFacingDirection ExitFacingDirectionRight;
	public EFacingDirection ExitFacingDirectionLeft;
}
