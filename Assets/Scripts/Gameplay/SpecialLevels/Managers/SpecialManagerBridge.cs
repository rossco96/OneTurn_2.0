using UnityEngine;

public class SpecialManagerBridge : SpecialManager_Base
{
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < m_placedInteractables.Length; ++i)
		{
			SpecialInteractableBridge_Base bridgeBase = m_placedInteractables[i].GetComponent<SpecialInteractableBridge_Base>();
			BridgeSquare bridgeSquare = new BridgeSquare(bridgeBase.GridPosX, bridgeBase.GridPosY, m_mapPropertyData, LevelSelectData.MapData.GridLayout);
			bridgeBase.SetBridgeSquare(bridgeSquare);
		}
	}

	public override GameObject GetInteractable(int posX, int posY/*, out EFacingDirection dir*/)
	{
		BridgeSquare bridgeSquare = new BridgeSquare(posX, posY, m_mapPropertyData, LevelSelectData.MapData.GridLayout);
		int index = 0;
		switch (bridgeSquare.PieceType)
		{
			case EBridgePiece.Single:	index = 0;		break;
			case EBridgePiece.End:		index = 1;		break;
			case EBridgePiece.Mid:		index = 2;		break;
			case EBridgePiece.Corner:	index = 3;		break;
			case EBridgePiece.T:		index = 4;		break;
			case EBridgePiece.Cross:	index = 5;		break;
			default:
				break;
		}
		//GameObject bridgeInteractable = m_interactablePrefabs[index];
		//bridgeInteractable.GetComponentInChildren<SpecialInteractableBridge_Base>().SetBridgeSquare(bridgeSquare);
		//dir = bridgeInteractable.GetComponentInChildren<SpecialInteractableBridge_Base>().FacingDirection;
		//return bridgeInteractable;
		return m_interactablePrefabs[index];
	}
}
