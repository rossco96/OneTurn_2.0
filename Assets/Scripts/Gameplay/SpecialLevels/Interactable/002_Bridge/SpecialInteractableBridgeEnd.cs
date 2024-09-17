using UnityEngine;

public class SpecialInteractableBridgeEnd : SpecialInteractableBridge_Base
{
	protected override void OnMapPropertyDataSet() { }

	protected override void SetRotation()
	{
		if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Right))
		{
			FacingDirection = EFacingDirection.Right;
			transform.Rotate(0, 0, -90);
		}
		else if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Down))
		{
			FacingDirection = EFacingDirection.Down;
			transform.Rotate(0, 0, 180);
		}
		else if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Left))
		{
			FacingDirection = EFacingDirection.Left;
			transform.Rotate(0, 0, 90);
		}
	}

	protected override void PlayerEnter()
	{
		// [TODO] Move this to base.PlayerEnter() ???
		// Used by all!
		// But then may need to have PlayerEnter() return bool, so we know to not continue... Or add a bool in the base, so will need to be checked in here either way.
		if (m_playerController.GetPlayerLocalPosition().z != 0)
		{
			DestroyPlayer(m_playerController);
			return;
		}

		if (m_playerController.Stats.IsOnSpecial == false)
		{
			if (Mathf.RoundToInt(transform.rotation.eulerAngles.z) % 180 == Mathf.RoundToInt(m_playerController.GetPlayerRotationEuler().z) % 180)
			{
				m_playerController.Stats.IsOnSpecial = true;
			}
			else
			{
				// Bad practice to pass this!
				// And issues with multiplayer if on the same square as other player!
				//	(must have array of current controllers on the bridge piece, then can add and remove)
				DestroyPlayer(m_playerController);
			}
		}
	}

	// [Q] We should never need this doing anything for bridges, right? Since handled by BridgeDetection.cs?
	protected override void PlayerExit()
	{
		//m_playerController.Stats.IsOnSpecial = false;
	}
}
