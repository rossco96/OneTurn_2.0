using UnityEngine;

[CreateAssetMenu(fileName = "LevelPlayInfo_", menuName = "Data/Level Play Info")]
public class LevelPlayInfo : ScriptableObject
{
	public int GridDimension;
	public int TotalItems;
	public int ItemTimeLimit;
	// [Q] Any other info?
}
