using UnityEngine;

public class GameplayManager_Exit : GameplayManager
{
	protected override void Start()
	{
		InitInteractableBehaviour<Exit>(OnPlayerInteractExit);
		base.Start();
	}

	protected override void UpdateTimer()
	{
		m_levelTimeElapsedFloat = Time.time - m_levelStartTime - m_totalTimePaused;
		if (Mathf.FloorToInt(m_levelTimeElapsedFloat) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_levelTimeElapsedFloat);
			m_hudManager.UpdateTimerTextCountUpP1(m_levelDisplayTimeInt);
			if (LevelSelectData.IsMultiplayer)
			{
				m_hudManager.UpdateTimerTextCountDownP2(m_levelDisplayTimeInt);
			}
		}
	}

	protected override void InitHUD()
	{
		base.InitHUD();
		m_hudManager.SetItemsCountActiveP1(false);
		m_hudManager.SetTimerSliderActiveP1(false);
		if (LevelSelectData.IsMultiplayer)
		{
			m_hudManager.SetMultiplayerStatsActive();
			m_hudManager.SetItemsCountActiveP2(false);
			m_hudManager.SetTimerSliderActiveP2(false);
		}
	}


	private void OnPlayerInteractExit(OTController controller)
	{
		// [IMPORTANT][TODO] Must see if player is facing the same way as the exit specifies!
		// If not, respawn (losing condition for lives == 0 in there)
		// Otherwise then yeah, obviously win condition

		// END GAME -- win
		if (LevelSelectData.IsMultiplayer)
		{
			controller.Stats.IsAtExit = true;
			EndGameMultiplayer();
		}
		else
		{
			EndGame(true);
		}
	}


	protected override void EndGame(bool isWin)
	{
		base.EndGame(isWin);

		int totalScore = GetTotalScore(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves);
		m_hudManager.SetEndScreenStatsSingle(totalScore, m_levelTimeElapsedFloat, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives);

		if (SaveSystem.StatFileSaveRequired(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves))
		{
			SaveSystem.SaveStatFileInfo(totalScore, m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves);
		}
	}

	protected override void EndGameMultiplayer()
	{
		base.EndGameMultiplayer();

		OTController controllerP1 = null;
		OTController controllerP2 = null;
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			if (m_controllers[i].Index == 0)
				controllerP1 = m_controllers[i];
			else
				controllerP2 = m_controllers[i];
		}

		EMultiplayerResult result = EMultiplayerResult.Draw;
		if (controllerP1.Stats.IsAtExit)
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
		}
		else //if (m_winningMultiplayerController == controllerP2)			// [NOTE] else-if here in case we're adding P3 and P4
		{
			result = EMultiplayerResult.P2;
			PlayerPrefsSystem.MultiplayerAddWinP2();
		}
		// NOTE not currently any drawing in exit mode as no time-out

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
