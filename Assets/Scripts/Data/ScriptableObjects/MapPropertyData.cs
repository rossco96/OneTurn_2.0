using UnityEngine;

[CreateAssetMenu(fileName = "MapPropertyData", menuName = "Data/Map Property Data")]
public class MapPropertyData : ScriptableObject
{
	public PropertyInfo[] PropertyInfos;

	public Color GetColorByName(EMapPropertyName name)
	{
		for (int i = 0; i < PropertyInfos.Length; ++i)
		{
			if (PropertyInfos[i].Name == name) return PropertyInfos[i].Color;
		}
		Debug.LogError($"[PropertyInfo::GetColorByName] No color corrseponding to the name '{name}'. Returning Color.white.");
		return Color.white;
	}

	public Sprite GetDropdownSpriteByName(EMapPropertyName name)
	{
		for (int i = 0; i < PropertyInfos.Length; ++i)
		{
			if (PropertyInfos[i].Name == name) return PropertyInfos[i].DropdownSprite;
		}
		Debug.LogError($"[PropertyInfo::GetDropdownSpriteByName] No color corrseponding to the name '{name}'. Returning null.");
		return null;
	}

	public EMapPropertyName GetNameByColor(Color color)
	{
		for (int i = 0; i < PropertyInfos.Length; ++i)
		{
			if (PropertyInfos[i].Color == color) return PropertyInfos[i].Name;
		}
		Debug.LogError($"[PropertyInfo::GetColorByName] No color corrseponding to the name '{name}'. Returning EMapPropertyName.BlankSquare.");
		return EMapPropertyName.BlankSquare;
	}
}

[System.Serializable]
public struct PropertyInfo
{
	public EMapPropertyName Name;
	public Color Color;
	public Sprite DropdownSprite;			// DropdownSprite is generic and NOT related to the level theme
}
