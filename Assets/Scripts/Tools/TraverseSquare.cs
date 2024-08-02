using UnityEngine;

public class TraverseSquare
{
	public int aX = 0;                      // DEBUGGING
	public int aY = 0;                      // ONLY!

	public bool Visited = false;
	public Color GridColor = Color.white;
	// [TODO] Rename this to SurroundingWalls, then don't need bools below or count above, and can still figure out valid ValidStartFacingDirections based off that
	public EFacingDirection[] SurroundingWalls = new EFacingDirection[0];
	public EFacingDirection[] ValidStartFacingDirections = new EFacingDirection[0];

	// Only want the relevant walls here. This will be the diagonal zero, one, or two opposite the (one or two) adjacent walls
	public (int, int)[] OppositeDiagonalWalls = new (int, int)[0];

	public TraverseSquare(int posX, int posY, MapPropertyData mapPropertyData, Texture2D gridLayoutTexture)
	{
		// DELETE DEBUG ONLY
		aX = posX;
		aY = posY;

		GridColor = gridLayoutTexture.GetPixel(posX, posY);
		SetWallsAndDirections(posX, posY, mapPropertyData, gridLayoutTexture);
		SetCornerWalls(posX, posY, mapPropertyData, gridLayoutTexture);

		//if ((posX == 0 && posY == 0) || (posX == 7 && posY == 7))
		//{
		//	Debug.Log($"{OppositeDiagonalWalls}");
		//}
	}

	private void SetWallsAndDirections(int posX, int posY, MapPropertyData mapPropertyData, Texture2D gridLayoutTexture)
	{
		if (posY + 1 >= gridLayoutTexture.width || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX, posY + 1)) == EMapPropertyName.Exit)
			SurroundingWalls = SurroundingWalls.Add(EFacingDirection.Up);
		else
			ValidStartFacingDirections = ValidStartFacingDirections.Add(EFacingDirection.Down);

		if (posX + 1 >= gridLayoutTexture.width || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY)) == EMapPropertyName.Exit)
			SurroundingWalls = SurroundingWalls.Add(EFacingDirection.Right);
		else
			ValidStartFacingDirections = ValidStartFacingDirections.Add(EFacingDirection.Left);

		if (posY - 1 < 0 || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX, posY - 1)) == EMapPropertyName.Exit)
			SurroundingWalls = SurroundingWalls.Add(EFacingDirection.Down);
		else
			ValidStartFacingDirections = ValidStartFacingDirections.Add(EFacingDirection.Up);

		if (posX - 1 < 0 || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY)) == EMapPropertyName.Exit)
			SurroundingWalls = SurroundingWalls.Add(EFacingDirection.Left);
		else
			ValidStartFacingDirections = ValidStartFacingDirections.Add(EFacingDirection.Right);
	}

	private void SetCornerWalls(int posX, int posY, MapPropertyData mapPropertyData, Texture2D gridLayoutTexture)
	{
		if (SurroundingWalls.Length == 1)
		{
			/*
			int surroundingWallDirectionInt = (int)SurroundingWalls[0];
			// check directions between (swdi+1 and swdi+2) AND (swdi+2 and swdi+3)

			int xDiff = (surroundingWallDirectionInt % 2 == 0) ? 0 : 1;
			int yDiff = (surroundingWallDirectionInt % 2 == 0) ? 1 : 0;
			xDiff *= (surroundingWallDirectionInt / 2 == 0) ? 1 : -1;
			yDiff *= (surroundingWallDirectionInt / 2 == 0) ? 1 : -1;

			(int, int) currentCoords = (posX + xDiff, posY + yDiff);
			//*/

			// WALL		Check		Check
			//	0		(1, -1)		(-1, -1)
			//	1		(-1, -1)	(-1, 1)
			//	2		(-1, 1)		(1, 1)
			//	3		(1, 1)		(1, -1)

			// [TODO] Try and automate the above based on the direction of the one existing wall!
			// (same when 2x surrounding walls, below)

			switch ((int)SurroundingWalls[0])
			{
				case 0:
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX + 1, posY - 1));
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX - 1, posY - 1));
					break;

				case 1:
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX - 1, posY - 1));
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX - 1, posY + 1));
					break;

				case 2:
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX - 1, posY + 1));
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX + 1, posY + 1));
					break;

				default:
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX + 1, posY + 1));
					if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Exit)
						OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX + 1, posY - 1));
					break;
			}
		}
		else if (SurroundingWalls.Length == 2 && Mathf.Abs((int)SurroundingWalls[1] - (int)SurroundingWalls[0]) == 1)
		{
			if (SurroundingWalls.Contains(EFacingDirection.Up) && SurroundingWalls.Contains(EFacingDirection.Right))
			{
				if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Exit)
					OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX - 1, posY - 1));
			}
			else if (SurroundingWalls.Contains(EFacingDirection.Right) && SurroundingWalls.Contains(EFacingDirection.Down))
			{
				if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Exit)
					OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX - 1, posY + 1));
			}
			else if (SurroundingWalls.Contains(EFacingDirection.Down) && SurroundingWalls.Contains(EFacingDirection.Left))
			{
				if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Exit)
					OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX + 1, posY + 1));
			}
			else
			{
				// contains Left and Up
				if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Exit)
					OppositeDiagonalWalls = OppositeDiagonalWalls.Add((posX + 1, posY - 1));
			}
		}

		/*
		if (posY - 1 < 0)
		{
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY - 1));
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY - 1));
		}
		else
		{
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY - 1));
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY - 1));
		}

		if (posY + 1 >= gridLayoutTexture.width)
		{
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY + 1));
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY + 1));
		}
		else
		{
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY + 1));
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY + 1));
		}

		if (posX - 1 < 0)
		{
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY - 1));
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY + 1));
		}
		else
		{
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY - 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY - 1));
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY + 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX - 1, posY + 1));
		}

		if (posX + 1 > gridLayoutTexture.width)
		{
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY - 1));
			OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY + 1));
		}
		else
		{
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY - 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY - 1));
			if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Wall || mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY + 1)) == EMapPropertyName.Exit)
				OppositeDiagonalWalls = OppositeDiagonalWalls.TryAdd((posX + 1, posY + 1));
		}
		//*/
	}
}
