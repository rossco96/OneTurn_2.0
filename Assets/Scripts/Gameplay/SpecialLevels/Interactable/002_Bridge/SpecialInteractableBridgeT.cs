//using UnityEngine;

public class SpecialInteractableBridgeT : SpecialInteractableBridge_Base
{
	protected override void OnMapPropertyDataSet() { }

	protected override void SetRotation()
	{
		if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Up) == false)
		{
			FacingDirection = EFacingDirection.Right;
			transform.Rotate(0, 0, -90);
		}
		else if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Right) == false)
		{
			FacingDirection = EFacingDirection.Down;
			transform.Rotate(0, 0, 180);
		}
		else if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Down) == false)
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

/*	
	{
		// Race condition issue with doing like this
		// Delete IsOnSpecial, and work only off sending a message up?
		// Will still need IsOnSpecial, surely, but disable ONLY WHEN LEAVING AN EXIT/SINGLE piece
		// (also determined by sending a message? then will only need a base SpecialInteractableBridge, and can spawn the specific piece type and rotation based on m_bridgePiece?)
		m_playerController.Stats.IsOnSpecial = false;
	}
//*/
}
