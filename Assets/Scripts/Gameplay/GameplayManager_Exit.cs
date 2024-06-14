using UnityEngine;

public class GameplayManager_Exit : GameplayManager
{
	private float m_levelTimeCountUp = 0.0f;
	private int m_levelTimeInt = 0;

	protected override void Start()
	{
		base.Start();
		InitInteractableBehaviour<Exit>(OnPlayerInteractExit);
	}

	protected override void UpdateTimer()
	{
		m_levelTimeCountUp = Time.time - m_levelStartTime - m_totalTimePaused;
		if (Mathf.FloorToInt(m_levelTimeCountUp) != m_levelTimeInt)
		{
			m_levelTimeInt = Mathf.FloorToInt(m_levelTimeCountUp);
			m_hudManager.UpdateTimerTextExit(m_levelTimeInt);
		}
	}

	protected override void InitHUD()
	{
		base.InitHUD();
		m_hudManager.SetItemsCountActive(false);
		m_hudManager.SetTimerSliderActive(false);
	}

	private void OnPlayerInteractExit(OTController controller)
	{
		// [IMPORTANT][TODO] Must see if player is facing the same way as the exit specifies!
		// If not, respawn (losing condition for lives == 0 in there)
		// Otherwise then yeah, obviously win condition

		// END GAME -- win
		EndGame(true, controller);
	}
}
