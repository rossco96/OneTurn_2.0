using UnityEngine.Events;

public abstract class SpecialInteractableBridge_Base : SpecialInteractable_Base
{
	protected BridgeSquare m_bridgeSquare;
	public void SetBridgeSquare(BridgeSquare bridgeSquare)
	{
		m_bridgeSquare = bridgeSquare;
		SetRotation();
	}
	protected abstract void SetRotation();

	private UnityAction<bool, bool> m_endGameEvent;
	private UnityAction<bool, int> m_updateHUDLives;

	public override void OnPlayerCrash(UnityAction<bool, bool> endGameEvent, UnityAction<bool, int> updateHUDLives)
	{
		m_endGameEvent += endGameEvent;
		m_updateHUDLives += updateHUDLives;
	}

	// Made public so we can call SendMessageUpwards("DestroyPlayer") from BridgeDetection
	public void DestroyPlayer(OTController controller)
	{
		// Note. This can get called twice (e.g. by the overlap two separate bridge detections)
		// Is this the best way to solve it? Only enable bridge detections if on that specific piece??? THAT'S A WAY BETTER SOLUTION! DO THAT THEN CAN DELETE THIS! THANKS ROSS
		// TODO TODO TODO TODO TODO
		if (m_playerController == null || controller != m_playerController)
			return;

		UnityEngine.Debug.LogWarning("[SpecIntBridgeBASE] >>> DestroyPlayer()");

		m_playerController.SetInputDisabled(true);
		m_playerController.DestroyPlayerGameObject();

		m_playerController.Stats.Lives--;
		if (m_playerController.Stats.Lives == 0)
		{
			m_endGameEvent(LevelSelectData.IsMultiplayer, false);
		}
		else
		{
			// [NOTE] This should be done after the death animation is complete! Need another callback...
			// [NOTE] Need also a spawn animation, and THEN can resume control of player
			m_playerController.RespawnPlayer();

			// [TODO] DO THIS ELSEWHERE! Needs to be done after death animation complete
			m_playerController.SetInputDisabled(false);
		}

		m_updateHUDLives(m_playerController.Index == 0, m_playerController.Stats.Lives);
	}
}