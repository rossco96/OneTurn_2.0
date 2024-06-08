#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class W_GridToMap : EditorWindow
{
	// Too hacky? What if we want to change this?
	// Same concern with if we ever want height and width to be different (i.e. rectangular and not square)
	private readonly List<int> m_validGridSizes = new List<int>() { 9, 11, 13, 15, 17 };

	private Texture2D m_gridLayoutTexture = null;
	
	private GameObject m_wallPrefab = null;
	private GameObject m_borderPrefabStraight = null;
	private GameObject m_borderPrefabCorner = null;

	private int m_gridDimension = 0;
	private float m_gridSizeMultiplier = 1.0f;

	private Transform m_borderParent = null;
	private Transform m_wallsParent = null;



	[MenuItem("Auburn Zone/Grid To Map")]
	public static void OpenWindow()
	{
		GetWindow(typeof(W_GridToMap));
	}

	private void OnGUI()
	{
		// [NOTE] rect1 and rect2 are just random values! Should put them as readonly global vars...

		Rect rectGridLayoutTexture = new Rect(5.0f, 5.0f, 200.0f, 200.0f);
		m_gridLayoutTexture = (Texture2D)EditorGUI.ObjectField(rectGridLayoutTexture, m_gridLayoutTexture, typeof(Texture2D), false);

		Rect rectWallPrefab = new Rect(5.0f, 210.0f, 200.0f, 200.0f);
		m_wallPrefab = (GameObject)EditorGUI.ObjectField(rectWallPrefab, m_wallPrefab, typeof(GameObject), false);

		Rect rectBorderPrefabStraight = new Rect(5.0f, 415.0f, 200.0f, 200.0f);
		m_borderPrefabStraight = (GameObject)EditorGUI.ObjectField(rectBorderPrefabStraight, m_borderPrefabStraight, typeof(GameObject), false);

		Rect rectBorderPrefabCorner = new Rect(210.0f, 415.0f, 200.0f, 200.0f);
		m_borderPrefabCorner = (GameObject)EditorGUI.ObjectField(rectBorderPrefabCorner, m_borderPrefabCorner, typeof(GameObject), false);

		if (m_gridLayoutTexture && m_wallPrefab && m_borderPrefabStraight && m_borderPrefabCorner)
		{
			// Ensure width == height, and both equal to correct dimensions (9, 11, 13, 15, or 17)
			// [TODO] What about having multiple 9x9 e.g. in Computer?
			// Just specify first one, and then load the next one once landing on a travel pad?
			if (GUILayout.Button("Generate") && SetGridMultiplier())
			{
				Generate();
			}
		}
	}

	private void Generate()
	{
		// Should ideally nullcheck here...
		// [NOTE] Parenting is optional!
		m_borderParent = GameObject.Find("BorderParent").transform;
		m_wallsParent = GameObject.Find("WallsParent").transform;

		GenerateBorder();

		for (int x = 0; x < m_gridDimension; ++x)
		{
			for (int y = 0; y < m_gridDimension; ++y)					// [NOTE] This will need changing if allowing rectangular levels!
			{
				Color color = m_gridLayoutTexture.GetPixel(x, y);
				if (color == Color.white)		continue;
				else if (color == Color.black)	PlaceWall(x, y);
				// [TODO]
				//else if (color == Color.red)	PlaceSpawnPoint(x, y);
				//else if (color == Color.blue)	PlaceItem(x, y);
				//else if (color == Color.grey)	PlaceExit(x, y);
				//else if (color == Color.green)	PlaceSpecialItem(x, y);
			}
		}
	}

	private bool SetGridMultiplier()
	{
		if (m_gridLayoutTexture.width != m_gridLayoutTexture.height)		return false;
		if (m_validGridSizes.Contains(m_gridLayoutTexture.width) == false)	return false;

		m_gridDimension = m_gridLayoutTexture.width;
		// [NOTE][IMPORTANT] 0.2f at the end is because the border is *CURRENTLY* 1/10th the width of the walls (multiplied by two lots of borders, one each side of the screen)
		// [TODO] Make sure we're calculating the difference?? Ask an artist about import settings??
		// Border should always be the same size, regardless of grid size!
		m_gridSizeMultiplier = (Camera.main.aspect * Camera.main.orthographicSize * 2.0f) / (m_gridDimension + 0.2f);
		Debug.Log($">>>>> {m_gridSizeMultiplier}");
		return true;
	}

	private void GenerateBorder()
	{
		// 0.05 is half of the width
		// 0.15 composed of the width plus half of the width
		// 0.6 is half the border height plus 1 full width

		float gridDimensionPlusBorder = m_gridDimension + 0.15f;

		// Set corners:
		Vector2 pos1 = m_gridSizeMultiplier * 0.05f * Vector2.one;
		GameObject c1 = Instantiate(m_borderPrefabCorner, pos1, Quaternion.identity, m_borderParent);
		c1.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);

		Vector2 pos2 = m_gridSizeMultiplier * new Vector2(gridDimensionPlusBorder, 0.05f);
		GameObject c2 = Instantiate(m_borderPrefabCorner, pos2, Quaternion.identity, m_borderParent);
		c2.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);

		Vector2 pos3 = m_gridSizeMultiplier * new Vector2(0.05f, gridDimensionPlusBorder);
		GameObject c3 = Instantiate(m_borderPrefabCorner, pos3, Quaternion.identity, m_borderParent);
		c3.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);

		Vector2 pos4 = m_gridSizeMultiplier * new Vector2(gridDimensionPlusBorder, gridDimensionPlusBorder);
		GameObject c4 = Instantiate(m_borderPrefabCorner, pos4, Quaternion.identity, m_borderParent);
		c4.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);


		for (int i = 0; i < m_gridDimension; ++i)
		{
			// Set vertical sides:
			Vector2 posHorNear = m_gridSizeMultiplier * new Vector2(0.05f, i + 0.6f);
			GameObject wallHorNear = Instantiate(m_borderPrefabStraight, posHorNear, Quaternion.identity, m_borderParent);
			wallHorNear.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);

			Vector2 posHorFar = m_gridSizeMultiplier * new Vector2(gridDimensionPlusBorder, i + 0.6f);
			GameObject wallHorFar = Instantiate(m_borderPrefabStraight, posHorFar, Quaternion.identity, m_borderParent);
			wallHorFar.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);

			// Set horizontal sides:
			Vector2 posVerNear = m_gridSizeMultiplier * new Vector2(i + 0.6f, 0.05f);
			GameObject wallVerNear = Instantiate(m_borderPrefabStraight, posVerNear, Quaternion.identity, m_borderParent);
			wallVerNear.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);
			wallVerNear.transform.Rotate(0, 0, 90);

			Vector2 posVerFar = m_gridSizeMultiplier * new Vector2(i + 0.6f, gridDimensionPlusBorder);
			GameObject wallVerFar = Instantiate(m_borderPrefabStraight, posVerFar, Quaternion.identity, m_borderParent);
			wallVerFar.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);
			wallVerFar.transform.Rotate(0, 0, 90);
		}
	}

	private void PlaceWall(int x, int y)
	{
		// [TODO] Will eventually want to distinguish between 
		// Straight, Corner, T, and Cross pieces...
		// (I, L, T, +)
		// And the correct directions to face them!
		// ... Just implement as plain blocks for now -- prefabs! With colliders on them

		// 0.6f below because of 0.5f * of our wall width plus our border width 0.1f
		Vector2 position = m_gridSizeMultiplier * new Vector2(x + 0.6f, y + 0.6f);
		GameObject wall = Instantiate(m_wallPrefab, position, Quaternion.identity, m_wallsParent);
		wall.transform.localScale = new Vector3(m_gridSizeMultiplier, m_gridSizeMultiplier, 1.0f);
	}
}

#endif