using UnityEngine;

[CreateAssetMenu(fileName = "MapData_", menuName = "Data/Map Data")]
public class MapData : ScriptableObject
{
	// [TODO][IMPORTANT] THIS IS ONLY CORRECT FOR GAME MAPS! MUST FIGURE OUT HOW TO DO THIS IN GENERAL!
	// >>> Currently creating this asset in LevelSelectMenuManager... No idea how that's working tbh
	public string FileName => (LevelSelectData.MapType == EMapType.Game) ? $"{Hash128.Compute(GridLayout.EncodeToPNG())}" : CustomImportedMapFileName;

	// These are used for CustomImported maps only
	// [Q] Create child MapData SOs?
	[HideInInspector] public string CustomImportedMapFileName;
	[HideInInspector] public string MapName;

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
