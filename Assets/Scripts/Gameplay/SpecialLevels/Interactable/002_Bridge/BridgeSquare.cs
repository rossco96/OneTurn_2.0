using UnityEngine;

public class BridgeSquare
{
	public EFacingDirection[] SurroundingBridges = new EFacingDirection[0];
	public EBridgePiece PieceType;
	
	public BridgeSquare(int posX, int posY, MapPropertyData mapPropertyData, Texture2D gridLayoutTexture)
	{
		SetBridges(posX, posY, mapPropertyData, gridLayoutTexture);
		SetPiece();
	}

	private void SetBridges(int posX, int posY, MapPropertyData mapPropertyData, Texture2D gridLayoutTexture)
	{
		if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX, posY + 1)) == EMapPropertyName.Special)
			SurroundingBridges = SurroundingBridges.Add(EFacingDirection.Up);
		
		if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX + 1, posY)) == EMapPropertyName.Special)
			SurroundingBridges = SurroundingBridges.Add(EFacingDirection.Right);
		
		if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX, posY - 1)) == EMapPropertyName.Special)
			SurroundingBridges = SurroundingBridges.Add(EFacingDirection.Down);
		
		if (mapPropertyData.GetNameByColor(gridLayoutTexture.GetPixel(posX - 1, posY)) == EMapPropertyName.Special)
			SurroundingBridges = SurroundingBridges.Add(EFacingDirection.Left);
	}

	private void SetPiece()
	{
		if (SurroundingBridges.Length == 0)
			PieceType = EBridgePiece.Single;
		else if (SurroundingBridges.Length == 1)
			PieceType = EBridgePiece.End;
		else if (SurroundingBridges.Length == 3)
			PieceType = EBridgePiece.T;
		else if (SurroundingBridges.Length == 4)
			PieceType = EBridgePiece.Cross;
		else
		{
			// SurroundingBridges.Length == 2 (therefore is either mid or corner section)
			int dir0 = (int)SurroundingBridges[0];
			int dir1 = (int)SurroundingBridges[1];
			if (dir1 - dir0 == 2)
				PieceType = EBridgePiece.Mid;
			else
				PieceType = EBridgePiece.Corner;
		}
	}
}