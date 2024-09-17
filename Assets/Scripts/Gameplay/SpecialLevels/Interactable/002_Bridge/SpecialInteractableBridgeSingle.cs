using UnityEngine;

public class SpecialInteractableBridgeSingle : SpecialInteractableBridge_Base
{
	protected override void OnMapPropertyDataSet() { }

	protected override void SetRotation()
	{
		// [TODO][IMPORTANT]
		// This is actually far more complicated to work out for single than any others!
		//	o Must check if near edges or walls
		//	o Otherwise can just randomly decide which way to put it, based off x,y coords perhaps??
	}

	protected override void PlayerEnter()
	{
		if (Mathf.RoundToInt(transform.rotation.eulerAngles.z) % 180 == Mathf.RoundToInt(m_playerController.GetPlayerRotationEuler().z) % 180)
		{
			Debug.Log("[bridge_single] --> isOnSpecial = true");
			m_playerController.Stats.IsOnSpecial = true;
		}
		else
		{
			Debug.Log("[bridge_single] --> DESTROY");
			// TEMP LIKE THIS! As per, will want to implement respawn properly
			DestroyPlayer(m_playerController);
		}
	}

	protected override void PlayerExit()
	{
		Debug.Log("[bridge_single] (on exit / do nothing)");
	}
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
