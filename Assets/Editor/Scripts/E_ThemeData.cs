#if FALSE
//#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThemeData))]
public class E_ThemeData : Editor
{
	private ThemeData m_themeData;
	private string[] m_propertiesToExclude;

	private void OnEnable()
	{
		m_themeData = (ThemeData)target;
		m_propertiesToExclude = new string[] { "IsSpecialLevel", "SpecialSprite", "SpecialLevelFunction" };
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		DrawPropertiesExcluding(serializedObject, m_propertiesToExclude);
		m_themeData.IsSpecialLevel = EditorGUILayout.Toggle("Is Special Level", m_themeData.IsSpecialLevel);
		if (m_themeData.IsSpecialLevel)
		{
			m_themeData.SpecialSprite = (Sprite)EditorGUILayout.ObjectField("Special Sprite", m_themeData.SpecialSprite, typeof(Sprite), false);
			m_themeData.SpecialLevelFunction = (SpecialLevel_Base)EditorGUILayout.ObjectField("Special Level Function", m_themeData.SpecialLevelFunction, typeof(SpecialLevel_Base), false);
		}
		else
		{
			m_themeData.SpecialSprite = null;
			m_themeData.SpecialLevelFunction = null;
		}

		//*
		if (GUI.changed)
		{
			EditorUtility.SetDirty(m_themeData);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		//*/
	}
}

#endif