//using UnityEngine;

public class SpecialInteractableBridgeCorner : SpecialInteractableBridge_Base
{
	protected override void OnMapPropertyDataSet() { }

	protected override void SetRotation()
	{
		if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Right) && m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Down))
		{
			FacingDirection = EFacingDirection.Right;
			transform.Rotate(0, 0, -90);
		}
		else if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Down) && m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Left))
		{
			FacingDirection = EFacingDirection.Down;
			transform.Rotate(0, 0, 180);
		}
		else if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Left) && m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Up))
		{
			FacingDirection = EFacingDirection.Left;
			transform.Rotate(0, 0, 90);
		}
	}

	protected override void PlayerEnter()
	{
		if (m_playerController.GetPlayerLocalPosition().z != 0)
		{
			DestroyPlayer(m_playerController);
			return;
		}

		/*
		if (Mathf.RoundToInt(transform.rotation.eulerAngles.z) % 180 == Mathf.RoundToInt(m_playerController.GetPlayerRotationEuler().z) % 180)
			m_playerController.Stats.IsOnSpecial = true;
		else
			m_playerController.DestroyPlayerGameObject(true);
		//*/
	}

	protected override void PlayerExit() { }
}
