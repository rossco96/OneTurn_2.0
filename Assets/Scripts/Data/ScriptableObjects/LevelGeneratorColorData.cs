using UnityEngine;

[CreateAssetMenu(fileName = "LevelGeneratorColorData", menuName = "Data/Level Generator Color Data")]
public class LevelGeneratorColorData : ScriptableObject
{
	public ColorData[] ColorDatas;

	public Color GetColorByName(ELevelGeneratorColorName name)
	{
		for (int i = 0; i < ColorDatas.Length; ++i)
		{
			if (ColorDatas[i].Name == name) return ColorDatas[i].Color;
		}
		Debug.LogError($"[LevelGeneratorColorData::GetColorByName] No color corrseponding to the name '{name}'.");
		return Color.white;
	}
}

[System.Serializable]
public struct ColorData
{
	public ELevelGeneratorColorName Name;
	public Color Color;
}
