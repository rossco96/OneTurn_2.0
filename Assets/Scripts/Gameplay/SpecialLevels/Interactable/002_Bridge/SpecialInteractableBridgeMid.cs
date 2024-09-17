using UnityEngine;

public class SpecialInteractableBridgeMid : SpecialInteractableBridge_Base
{
	protected override void OnMapPropertyDataSet() { }

	protected override void SetRotation()
	{
		if (m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Left) && m_bridgeSquare.SurroundingBridges.Contains(EFacingDirection.Right))
		{
			FacingDirection = EFacingDirection.Right;
			transform.Rotate(0, 0, -90);
		}
	}

	protected override void PlayerEnter()
	{
		if (m_playerController.GetPlayerLocalPosition().z != 0)
		{
			DestroyPlayer(m_playerController);
			return;
		}

		if (Mathf.RoundToInt(transform.rotation.eulerAngles.z) % 180 != Mathf.RoundToInt(m_playerController.GetPlayerRotationEuler().z) % 180)
		{
			Vector3 playerPosition = m_playerController.GetPlayerPosition();
			playerPosition += 10.0f * Vector3.forward;
			m_playerController.SetPlayerPosition(playerPosition);
		}
	}

	protected override void PlayerExit()
	{
		if (Mathf.RoundToInt(transform.rotation.eulerAngles.z) % 180 != Mathf.RoundToInt(m_playerController.GetPlayerRotationEuler().z) % 180)
		{
			Vector3 playerPosition = m_playerController.GetPlayerPosition();
			playerPosition -= 10.0f * Vector3.forward;
			m_playerController.SetPlayerPosition(playerPosition);
		}
	}
}
