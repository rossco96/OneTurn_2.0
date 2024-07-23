using UnityEngine;

public class GameplayManager_MBomb : GameplayManager
{
	private OTController m_controllerP1 = null;
	private OTController m_controllerP2 = null;

	private float m_levelTimeElapsedP1 = 0.0f;
	private float m_levelTimeElapsedP2 = 0.0f;

	protected override void Start()
	{
		InitInteractableBehaviour<Bomb>(OnPlayerInteractBomb);
		m_timeLimit = LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit / 2.0f;
		base.Start();

		for (int i = 0; i < m_controllers.Length; ++i)
		{
			m_controllers[i].Stats.BombTimeLeft = m_timeLimit;
			if (m_controllers[i].Index == 0)
				m_controllerP1 = m_controllers[i];
			else //if (m_controllers[i].Index == 1)
				m_controllerP2 = m_controllers[i];
		}
	}

	protected override void UpdateTimer()
	{
		if (m_controllerP1.Stats.HasBomb)
			UpdateTimerP1();
		else //if (m_controllerP2.Stats.HasBomb)
			UpdateTimerP2();
	}

	private void UpdateTimerP1()
	{
		m_controllerP2.Stats.BombPauseTime += Time.deltaTime;

		m_levelTimeElapsedP1 = Time.time - m_levelStartTime - m_totalTimePaused - m_controllerP1.Stats.BombPauseTime;
		m_controllerP1.Stats.BombTimeLeft = m_timeLimit - m_levelTimeElapsedP1;

		if (m_controllerP1.Stats.BombTimeLeft < 10.0f)
		{
			if (m_controllerP1.Stats.BombTimeLeft <= 0.0f)
			{
				m_hudManager.UpdateTimerTextCountDownP1(0.0f);
				m_hudManager.UpdateTimerSliderP1(0.0f);
				// END GAME (p2 win)
				EndGameMultiplayer();
				return;
			}
			m_hudManager.UpdateTimerTextCountDownP1(m_controllerP1.Stats.BombTimeLeft.RoundDP(2));
		}
		else if (Mathf.FloorToInt(m_controllerP1.Stats.BombTimeLeft) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_controllerP1.Stats.BombTimeLeft);
			m_hudManager.UpdateTimerTextCountDownP1(m_levelDisplayTimeInt);
		}

		m_hudManager.UpdateTimerSliderP1(m_controllerP1.Stats.BombTimeLeft * 2.0f);
	}

	private void UpdateTimerP2()
	{
		m_controllerP1.Stats.BombPauseTime += Time.deltaTime;

		m_levelTimeElapsedP2 = Time.time - m_levelStartTime - m_totalTimePaused - m_controllerP2.Stats.BombPauseTime;
		m_controllerP2.Stats.BombTimeLeft = m_timeLimit - m_levelTimeElapsedP2;

		if (m_controllerP2.Stats.BombTimeLeft < 10.0f)
		{
			if (m_controllerP2.Stats.BombTimeLeft <= 0.0f)
			{
				m_hudManager.UpdateTimerTextCountDownP2(0.0f);
				m_hudManager.UpdateTimerSliderP2(0.0f);
				// END GAME (p1 win)
				EndGameMultiplayer();
				return;
			}
			m_hudManager.UpdateTimerTextCountDownP2(m_controllerP2.Stats.BombTimeLeft.RoundDP(2));
		}
		else if (Mathf.FloorToInt(m_controllerP2.Stats.BombTimeLeft) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_controllerP2.Stats.BombTimeLeft);
			m_hudManager.UpdateTimerTextCountDownP2(m_levelDisplayTimeInt);
		}

		m_hudManager.UpdateTimerSliderP2(m_controllerP2.Stats.BombTimeLeft * 2.0f);
	}


	protected override void InitHUD()
	{
		base.InitHUD();
		
		m_hudManager.SetCountStatActiveP1(false);
		m_hudManager.SetTimerSliderActiveP1(true);
		m_hudManager.UpdateTimerSliderP1(m_timeLimit * 2.0f);
		m_hudManager.UpdateTimerTextCountDownP1(m_timeLimit);

		m_hudManager.SetCountStatActiveP2(false);
		m_hudManager.SetTimerSliderActiveP2(true);
		m_hudManager.UpdateTimerSliderP2(m_timeLimit * 2.0f);
		m_hudManager.UpdateTimerTextCountDownP2(m_timeLimit);
	}

	private void OnPlayerInteractBomb(OTController controller, Interactable_Base interactable)
	{
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			if (m_controllers[i] != controller)
			{
				m_controllers[i].Stats.HasBomb = false;
			}
		}
		controller.Stats.HasBomb = true;
	}

	protected override void EndGameMultiplayer()
	{
		base.EndGameMultiplayer();

		EMultiplayerResult result = EMultiplayerResult.Draw;
		if (m_controllerP1.Stats.HasBomb)
		{
			result = EMultiplayerResult.P2;
			PlayerPrefsSystem.MultiplayerAddWinP2();
			m_hudManager.SetEndScreenStatsMultiP1(0, m_controllerP1.Stats.Moves, m_controllerP1.Stats.Lives);
			int scoreP2 = GetTotalScore(m_controllerP2.Stats.BombTimeLeft, m_controllerP2.Stats.Lives, m_controllerP2.Stats.Moves);
			m_hudManager.SetEndScreenStatsMultiP2(scoreP2, m_controllerP2.Stats.Moves, m_controllerP2.Stats.Lives);
		}
		else //if (m_controllerP2.Stats.HasBomb)			// [NOTE] else-if here in case we're adding P3 and P4
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
			int scoreP1 = GetTotalScore(m_controllerP1.Stats.BombTimeLeft, m_controllerP1.Stats.Lives, m_controllerP1.Stats.Moves);
			m_hudManager.SetEndScreenStatsMultiP1(scoreP1, m_controllerP1.Stats.Moves, m_controllerP1.Stats.Lives);
			m_hudManager.SetEndScreenStatsMultiP2(0, m_controllerP2.Stats.Moves, m_controllerP2.Stats.Lives);
		}

		m_hudManager.SetWinLoseTitleMulti(result);
		m_hudManager.ShowEndScreen();
	}


	// [TODO]
	// Check if the stats are better than the existing ones, then save file if so
	//	o Only want to update the exit time (or total score if multiple exits?) in exit mode

	// [TODO][IMPORTANT]
	// Work on the formula, based on actual player testing -- not just what *I* can achieve in a level!
	private int GetTotalScore(float time, int lives, int moves)
	{
		int maxTime = LevelSelectData.GridDimension * LevelSelectData.GridDimension;
		if (lives == 0 || time > maxTime) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float timeRatio = (maxTime - time) / maxTime;
		const int scoreMultiplier = 10000;
		const int livesMultiplier = 1000;

		// [TODO] How to involve #moves? If at all?

		int score = Mathf.RoundToInt((gridRatio * timeRatio * scoreMultiplier) - ((LevelSelectData.LivesCount - lives) * livesMultiplier));
		Debug.Log($"{score} = Mathf.RoundToInt(({gridRatio} * {timeRatio} * {scoreMultiplier}) - (({LevelSelectData.LivesCount} - {lives}) * {livesMultiplier}))");
		return Mathf.Max(0, score);
	}
}
