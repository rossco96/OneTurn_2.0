#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// NOTE THIS ENTIRE CLASS AS IT STANDS WILL NOT BE APPLICABLE TO RACING SPECIAL LEVEL AS CAN CHANGE WHICH WAY WE TURN. WAY MORE DIFFICULT TO WORK OUT!

public class W_TraversableGrid : EditorWindow
{
	private MapPropertyData m_mapPropertyData = null;
	private Texture2D m_gridLayoutTexture = null;
	private int GridDimension => m_gridLayoutTexture.width;

	[MenuItem("Auburn Zone/Traversable Grid")]
	public static void OpenWindow()
	{
		GetWindow(typeof(W_TraversableGrid));
	}

	private void OnGUI()
	{
		// [NOTE] rectGridLayoutTexture and rectMapPropertyData are just random values! Should put as readonly global vars..?
		Rect rectGridLayoutTexture = new Rect(5.0f, 5.0f, 200.0f, 200.0f);
		m_gridLayoutTexture = (Texture2D)EditorGUI.ObjectField(rectGridLayoutTexture, m_gridLayoutTexture, typeof(Texture2D), false);
		Rect rectMapPropertyData = new Rect(5.0f, 210.0f, 400.0f, 50.0f);
		m_mapPropertyData = (MapPropertyData)EditorGUI.ObjectField(rectMapPropertyData, m_mapPropertyData, typeof(MapPropertyData), false);
		if (m_mapPropertyData && m_gridLayoutTexture)
		{
			if (GUILayout.Button("Calculate"))
				Calculate();
		}
	}

	private void Calculate()
	{
		List<(int, int)> traversableSquares = new List<(int, int)>();
		List<(int, int)> nonTraversableSquares = new List<(int, int)>();
		for (int y = 0; y < GridDimension; ++y)
		{
			for (int x = 0; x < GridDimension; ++x)
			{
				Color gridColor = m_gridLayoutTexture.GetPixel(x, y);
				if (gridColor == m_mapPropertyData.GetColorByName(EMapPropertyName.Wall) || gridColor == m_mapPropertyData.GetColorByName(EMapPropertyName.Exit))
					continue;

				TraverseSquare startSquare = new TraverseSquare(x, y, m_mapPropertyData, m_gridLayoutTexture);
				if (startSquare.SurroundingWalls.Length == 0)
				{
					traversableSquares.Add((x, y));
					continue;
				}
				if (startSquare.SurroundingWalls.Length == 3 || startSquare.SurroundingWalls.Length == 4)
				{
					nonTraversableSquares.Add((x, y));
					continue;
				}

				Dictionary<(int, int), TraverseSquare> gridDepthTwo = GetGridDepthTwo(x, y, startSquare);
				if (TryTraverse(x, y, gridDepthTwo))
					traversableSquares.Add((x, y));
				else
					nonTraversableSquares.Add((x, y));
			}
		}
		for (int i = 0; i < nonTraversableSquares.Count; ++i)
		{
			Debug.Log($"[{i+1}/{nonTraversableSquares.Count+1}]{nonTraversableSquares[i]}");
		}
	}

	private Dictionary<(int, int), TraverseSquare> GetGridDepthTwo(int posX, int posY, TraverseSquare startSquare)
	{
		Dictionary<(int, int), TraverseSquare> gridDepthTwo = new Dictionary<(int, int), TraverseSquare>();
		List<(int, int)> gridDepthTwoKeys = new List<(int, int)>();														// Try delete!

		#region Depth Zero
		gridDepthTwo.Add((posX, posY), startSquare);
		gridDepthTwoKeys.Add((posX, posY));
		#endregion

		#region Depth One | Case 1 (returns)
		Dictionary<EFacingDirection, List<TraverseSquare>> rectangles = GetRectangles1D(posX, posY);

		if ((startSquare.SurroundingWalls.Length == 1 && startSquare.OppositeDiagonalWalls.Length == 2) ||
			(startSquare.SurroundingWalls.Length == 2 && (startSquare.OppositeDiagonalWalls.Length == 1 || Mathf.Abs((int)startSquare.SurroundingWalls[1] - (int)startSquare.SurroundingWalls[0]) == 2)))
		{
			// get 1d rects and return
			// don't like using var...
			var rectKeysEnumerator = rectangles.Keys.GetEnumerator();
			while (rectKeysEnumerator.MoveNext())
			{
				var key = rectKeysEnumerator.Current;
				for (int j = 0; j < rectangles[key].Count; ++j)
				{
					int rectanglePosX = rectangles[key][j].aX;
					int rectanglePosY = rectangles[key][j].aY;
					gridDepthTwo.Add((rectanglePosX, rectanglePosY), rectangles[key][j]);
				}
			}
			/*
			for (int i = 0; i < rectangles.Count; ++i)
			{
				for (int j = 0; j < rectangles[i].Count; ++j)
				{
					int rectanglePosX = rectangles[i][j].aX;
					int rectanglePosY = rectangles[i][j].aY;
					gridDepthTwo.Add((rectanglePosX, rectanglePosY), rectangles[i][j]);
				}
			}
			//*/
			return gridDepthTwo;
		}
		#endregion

		Debug.Log($"{rectangles}");

		// get 1 or 2x 2d rects
		// will then need to work out depth two grid
		// then return

		#region Depth One | Case 2
		//if (startSquare.SurroundingWalls.Length == 1)
		{
			int positiveDirection = ((int)startSquare.SurroundingWalls[0] + 1) % 4;					// This is not always good for when startSquare.SurroundingWalls.Length == 2 ???
			int negativeDirection = ((int)startSquare.SurroundingWalls[0] - 1) % 4;
			bool checkPositive = true;
			bool checkNegative = (startSquare.SurroundingWalls.Length == 1) ? true : false;			// [Q] Are we doing this for checkPositive or checkNegative???
			int positiveMinDepth = m_gridLayoutTexture.width;
			int negativeMinDepth = m_gridLayoutTexture.width;
			List<TraverseSquare> toAddZerothIterationPositive = new List<TraverseSquare>();
			List<TraverseSquare> toAddZerothIterationNegative = new List<TraverseSquare>();

			// HELP! rectangles[1] is not always the 'middle' one when count == 3

			EFacingDirection oppositeDirection = (EFacingDirection)(((int)startSquare.SurroundingWalls[0] + 2) % 4);

			for (int i = 0; i < rectangles[oppositeDirection].Count; ++i)
			{
				gridDepthTwo.Add((rectangles[oppositeDirection][i].aX, rectangles[oppositeDirection][i].aY), rectangles[oppositeDirection][i]);
				List<TraverseSquare> toAdd = new List<TraverseSquare>();

				if (checkPositive)
				{
					int xDiffPositive = (positiveDirection % 2 == 0) ? 0 : 1;
					int yDiffPositive = (positiveDirection % 2 == 0) ? 1 : 0;
					xDiffPositive *= (positiveDirection / 2 == 0) ? 1 : -1;
					yDiffPositive *= (positiveDirection / 2 == 0) ? 1 : -1;

					(int, int) currentCoords = (rectangles[oppositeDirection][i].aX, rectangles[oppositeDirection][i].aY);
					TraverseSquare nextSquare = new TraverseSquare(rectangles[oppositeDirection][i].aX, rectangles[oppositeDirection][i].aY, m_mapPropertyData, m_gridLayoutTexture);

					while (nextSquare.SurroundingWalls.Contains((EFacingDirection)positiveDirection) == false)
					{
						currentCoords = (currentCoords.Item1 + xDiffPositive, currentCoords.Item2 + yDiffPositive);
						nextSquare = new TraverseSquare(currentCoords.Item1, currentCoords.Item2, m_mapPropertyData, m_gridLayoutTexture);
						if (i == 0)
							toAddZerothIterationPositive.Add(nextSquare);
						else
							toAdd.Add(nextSquare);
						if (toAdd.Count >= positiveMinDepth)
							break;
					}

					if (i == 0)
					{
						positiveMinDepth = toAdd.Count;
					}
					else if (i == 1)
					{
						int limit = (toAdd.Count < positiveMinDepth) ? toAdd.Count : toAddZerothIterationPositive.Count;
						for (int j = 0; j < limit; ++j)
						{
							gridDepthTwo.Add((toAddZerothIterationPositive[j].aX, toAddZerothIterationPositive[j].aY), toAddZerothIterationPositive[j]);
						}
					}
				}

				if (i > 0)
				{
					if (toAdd.Count > 0)
					{
						for (int j = 0; j < toAdd.Count; ++j)
						{
							gridDepthTwo.Add((toAdd[j].aX, toAdd[j].aY), toAdd[j]);
						}
						toAdd = new List<TraverseSquare>();
					}
					else
					{
						checkPositive = false;
					}
				}



				if (checkNegative)
				{
					int xDiffNegative = (negativeDirection % 2 == 0) ? 0 : 1;
					int yDiffNegative = (negativeDirection % 2 == 0) ? 1 : 0;
					xDiffNegative *= (negativeDirection / 2 == 0) ? 1 : -1;
					yDiffNegative *= (negativeDirection / 2 == 0) ? 1 : -1;

					(int, int) currentCoords = (rectangles[oppositeDirection][i].aX, rectangles[oppositeDirection][i].aY);
					TraverseSquare nextSquare = new TraverseSquare(rectangles[oppositeDirection][i].aX, rectangles[oppositeDirection][i].aY, m_mapPropertyData, m_gridLayoutTexture);

					while (nextSquare.SurroundingWalls.Contains((EFacingDirection)negativeDirection) == false)
					{
						currentCoords = (currentCoords.Item1 + xDiffNegative, currentCoords.Item2 + yDiffNegative);
						nextSquare = new TraverseSquare(currentCoords.Item1, currentCoords.Item2, m_mapPropertyData, m_gridLayoutTexture);
						if (i == 0)
							toAddZerothIterationNegative.Add(nextSquare);
						else
							toAdd.Add(nextSquare);
						if (toAdd.Count >= negativeMinDepth)
							break;
					}

					if (i == 0)
					{
						negativeMinDepth = toAdd.Count;
					}
					else if (i == 1)
					{
						int limit = (toAdd.Count < negativeMinDepth) ? toAdd.Count : toAddZerothIterationNegative.Count;
						for (int j = 0; j < limit; ++j)
						{
							gridDepthTwo.Add((toAddZerothIterationNegative[j].aX, toAddZerothIterationNegative[j].aY), toAddZerothIterationNegative[j]);
						}
					}
				}

				if (i > 0)
				{
					if (toAdd.Count > 0)
					{
						for (int j = 0; j < toAdd.Count; ++j)
						{
							gridDepthTwo.Add((toAdd[j].aX, toAdd[j].aY), toAdd[j]);
						}
					}
					else
					{
						checkNegative = false;
					}
				}

				if (checkPositive == false && checkNegative == false)
					break;
			}
		}
		/*else
		{
			// startSquare.SurroundingWalls.Length == 2
		
		}//*/



		// DEBUG! DELETE!
		Debug.Log("asdf");
		
		if ((posX == 0 && posY == 4) || (posX == 8 && posY == 4) || (posX == 8 && posY == 6) || (posX == 8 && posY == 7))
		{
			Debug.Log($"BREAK {gridDepthTwo}");
		}

		return gridDepthTwo;


		// NOTE: THE BELOW BLOCK COMMENT DOES NOT HAVE A CORRESPONDING CLOSE (the close which it uses is used by another block comment, which needs to stay where it is)
		/*


		if (startSquare.SurroundingWalls.Length == 1)
		{	//															int
			// UP														0
			//	RD?		Y - 2D		DL?		Y - 2D							+1,-1	-1,-1
			//								N - only check UR
			//			N -			DL?		Y - 2D
			//								N - 3x 1D

			// RIGHT													1
			//	DL?		Y - 2D		LU?		Y - 2D							-1,-1	-1,+1
			//								N - only check UR
			//			N -			LU?		Y - 2D
			//								N - 3x 1D

			// DOWN														2
			//	LU?		Y - 2D		UR?		Y - 2D							-1,+1	+1,+1
			//								N - only check UR
			//			N -			UR?		Y - 2D
			//								N - 3x 1D

			// LEFT														3
			//	UR?		Y - 2D		RD?		Y - 2D							+1,+1	+1,-1
			//								N - only check UR
			//			N -			RD?		Y - 2D
			//								N - 3x 1D
			
			int surroundingWallInt = (int)startSquare.SurroundingWalls[0];

			int nextSquareDiffX = (surroundingWallInt == 1 || surroundingWallInt == 2) ? -1 : 1;
			int nextSquareDiffY = (surroundingWallInt / 2 == 0) ? -1 : 1;
			(int, int) nextSquareCoords = (posX + nextSquareDiffX, posY + nextSquareDiffY);

			// if (1) NOT 2D
			//		if (2) NOT 2D
			//			return GetRectangleCoords1D
			//		ELSE
			//			keep going with 2D version, based on (2) only

			// ELSE
			//		if (2) NOT 2D
			//			keep going with 2D version, based on (1) only
			//		ELSE
			//			keep going with 2D version of both

			if (m_mapPropertyData.GetNameByColor(m_gridLayoutTexture.GetPixel(nextSquareCoords.Item1, nextSquareCoords.Item2)) == EMapPropertyName.Wall)
			{
				nextSquareDiffX = (surroundingWallInt / 2 == 0) ? -1 : 1;
				nextSquareDiffY = (surroundingWallInt == 1 || surroundingWallInt == 2) ? 1 : -1;
				nextSquareCoords = (posX + nextSquareDiffX, posY + nextSquareDiffY);

				if (m_mapPropertyData.GetNameByColor(m_gridLayoutTexture.GetPixel(nextSquareCoords.Item1, nextSquareCoords.Item2)) == EMapPropertyName.Wall)
				{
					List<List<(int, int)>> gridDepthOneCoords = GetRectangleCoords1D(posX, posY);
					for (int i = 0; i < gridDepthOneCoords.Count; ++i)
					{
						TraverseSquare square = new TraverseSquare(gridDepthOneCoords[i].Item1, gridDepthOneCoords[i].Item2, m_mapPropertyData, m_gridLayoutTexture);
						gridDepthTwo.Add((gridDepthOneCoords[i].Item1, gridDepthOneCoords[i].Item2), square);
					}
					return gridDepthTwo;
				}
				else
				{
					// Get2D version, of nextSquareCoords2
				}
			}
			else
			{
				nextSquareDiffX = (surroundingWallInt / 2 == 0) ? -1 : 1;
				nextSquareDiffY = (surroundingWallInt == 1 || surroundingWallInt == 2) ? 1 : -1;
				nextSquareCoords = (posX + nextSquareDiffX, posY + nextSquareDiffY);

				if (m_mapPropertyData.GetNameByColor(m_gridLayoutTexture.GetPixel(nextSquareCoords.Item1, nextSquareCoords.Item2)) == EMapPropertyName.Wall)
				{
					// Get2D version, of nextSquareCoords2
				}
				else
				{
					// Get2D version, of BOTH
				}
			}
		}
		else
		{
			// Can only be 2 if we're inside this method.

			// UP and DOWN					depth1 is horizontal line		(return there)
			// RIGHT and LEFT				depth1 is vertical line			(return there)
			if ((startSquare.SurroundingWalls.Contains(EFacingDirection.Up) && startSquare.SurroundingWalls.Contains(EFacingDirection.Down)) ||
				(startSquare.SurroundingWalls.Contains(EFacingDirection.Right) && startSquare.SurroundingWalls.Contains(EFacingDirection.Left)))
			{
				List<(int, int)> gridDepthOneCoords = GetRectangleCoords1D(posX, posY);
				for (int i = 0; i < gridDepthOneCoords.Count; ++i)
				{
					TraverseSquare square = new TraverseSquare(gridDepthOneCoords[i].Item1, gridDepthOneCoords[i].Item2, m_mapPropertyData, m_gridLayoutTexture);
					gridDepthTwo.Add((gridDepthOneCoords[i].Item1, gridDepthOneCoords[i].Item2), square);
				}
				return gridDepthTwo;
			}

			//(NOT)
			//	UP and RIGHT				square UR?						Y - 2D
			//																N - return 2x 1D lines
			//	RIGHT and DOWN				square RD?						Y - 2D
			//																N - return 2x 1D lines
			//	DOWN and LEFT				square DL?						Y - 2D
			//																N - return 2x 1D lines
			//	LEFT and UP					square LU?						Y - 2D
			//																N - return 2x 1D lines

		}
		//*/
		#endregion

		#region Depth Two (INCOMPLETE)
		//if (maxSmallestDimension == 1)
		//{
		//	return gridDepthTwo;
		//}
		
		// For each square in gridDepthOne:
		//	o For each direction:
		//		- IF gridDepthOne.Contains the new coords	Do not add
		//		- ELSE IF the new coords is a wall			Do not add
		//		- ELSE										Add
		#endregion






		//return gridDepthTwo;
	}

	private Dictionary<EFacingDirection, List<TraverseSquare>> GetRectangles1D(int posX, int posY)
	{
		Dictionary<EFacingDirection, List<TraverseSquare>> rectangles = new Dictionary<EFacingDirection, List<TraverseSquare>>();
		
		for (int i = 0; i < System.Enum.GetValues(typeof(EFacingDirection)).Length; ++i)
		{
			List<TraverseSquare> currentRectangle = new List<TraverseSquare>();
			
			int xDiff = (i % 2 == 0) ? 0 : 1;
			int yDiff = (i % 2 == 0) ? 1 : 0;
			xDiff *= (i / 2 == 0) ? 1 : -1;
			yDiff *= (i / 2 == 0) ? 1 : -1;

			(int, int) currentCoords = (posX, posY);
			TraverseSquare nextSquare = new TraverseSquare(posX, posY, m_mapPropertyData, m_gridLayoutTexture);

			while (nextSquare.SurroundingWalls.Contains((EFacingDirection)i) == false)
			{
				currentCoords = (currentCoords.Item1 + xDiff, currentCoords.Item2 + yDiff);
				nextSquare = new TraverseSquare(currentCoords.Item1, currentCoords.Item2, m_mapPropertyData, m_gridLayoutTexture);
				currentRectangle.Add(nextSquare);
			}

			if (currentRectangle.Count > 0)
				rectangles.Add((EFacingDirection)i, currentRectangle);
		}

		return rectangles;
	}

	private bool TryTraverse(int posX, int posY, Dictionary<(int, int), TraverseSquare> gridDepthTwo)
	{
		int currentPosX = posX;
		int currentPosY = posY;

		TraverseSquare startSquare = gridDepthTwo[(posX, posY)];



		// I'm getting tired so can't think of implementing this too clearly.
		// May need to keep track of keys (int pairs) so can reset all Visit bools upon exhausting or completing a route?
		// Would this also make it easier to keep track of the branching routes themselves?
		//	>>>	Would need to check each key twice - once for going forward, once for turning.
		List<(int, int)> currentRoute = new List<(int, int)>();
		Dictionary<List<(int, int)>, bool> allRoutes = new Dictionary<List<(int, int)>, bool>();							// THIS IS MAYHEM WHAT (but it just might work) Ross go to sleep



		// Right hand turns
		bool canTraverseRight = false;
		for (int i = 0; i < startSquare.ValidStartFacingDirections.Length; ++i)
		{
			EFacingDirection facingDirection = startSquare.ValidStartFacingDirections[i];

		}

		// Left hand turns
		// >>>	Is there a way we can do both in the same for loop? or is it easiest or equal or whatever to do two separate loops?
		//		Turning means something different, but will have already calculated the routes once so would be a waste to not test turning both directions...
		//		Though turning right could (would?) yield a different set of branching routes to turning left, rendering any left route useless to testing if we can turn right and vice versa.
		bool canTraverseLeft = false;

		return (canTraverseRight && canTraverseLeft);
	}
}

#endif