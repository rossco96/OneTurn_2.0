using UnityEngine;

[CreateAssetMenu(fileName = "MapPropertyColorData", menuName = "Data/Map Property Color Data")]
public class MapPropertyColorData : ScriptableObject
{
	public ColorData[] ColorDatas;

	public Color GetColorByName(EMapPropertyColorName name)
	{
		for (int i = 0; i < ColorDatas.Length; ++i)
		{
			if (ColorDatas[i].Name == name) return ColorDatas[i].Color;
		}
		Debug.LogError($"[MapPropertyColorData::GetColorByName] No color corrseponding to the name '{name}'. Returning Color.white.");
		return Color.white;
	}

	public EMapPropertyColorName GetNameByColor(Color color)
	{
		for (int i = 0; i < ColorDatas.Length; ++i)
		{
			if (ColorDatas[i].Color == color) return ColorDatas[i].Name;
		}
		Debug.LogError($"[MapPropertyColorData::GetColorByName] No color corrseponding to the name '{name}'. Returning ELevelGeneratorColorName.BlankSquare.");
		return EMapPropertyColorName.BlankSquare;
	}
}

[System.Serializable]
public struct ColorData
{
	public EMapPropertyColorName Name;
	public Color Color;
}
