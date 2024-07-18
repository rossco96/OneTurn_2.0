using UnityEngine;

// [TODO] Create child for GameplayManager_MItems (multiplayer version, which refernces all the P2 stuff)

public class GameplayManager_Items : GameplayManager
{
	protected override void Start()
	{
		InitInteractableBehaviour<Item>(OnPlayerInteractItem);
		m_timeLimit = LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit;
		base.Start();
	}



	protected override void UpdateTimer()
	{
		// Still want m_levelTimeElapsed for calculating score at the end
		// ... Or could just reverse engineer rather than have the extra calculation per frame?
		m_levelTimeElapsedFloat = Time.time - m_levelStartTime - m_totalTimePaused;
		m_countdownTimeRemainingFloat = m_timeLimit - m_levelTimeElapsedFloat;
		
		if (m_countdownTimeRemainingFloat < 10.0f)
		{
			if (m_countdownTimeRemainingFloat <= 0.0f)
			{
				m_hudManager.UpdateTimerTextCountDownP1(0.0f);
				m_hudManager.UpdateTimerSliderP1(0.0f);
				if (LevelSelectData.IsMultiplayer)
				{
					m_hudManager.UpdateTimerTextCountDownP2(0.0f);
					m_hudManager.UpdateTimerSliderP2(0.0f);
				}
				// END GAME -- lose

				Debug.Log($"[a] {LevelSelectData.IsMultiplayer}");
				
				if (LevelSelectData.IsMultiplayer)
					EndGameMultiplayer();
				else
					EndGame(false);
				return;
			}
			m_hudManager.UpdateTimerTextCountDownP1(m_countdownTimeRemainingFloat.RoundDP(2));
			if (LevelSelectData.IsMultiplayer)
			{
				m_hudManager.UpdateTimerTextCountDownP2(m_countdownTimeRemainingFloat.RoundDP(2));
			}
		}
		else if (Mathf.FloorToInt(m_countdownTimeRemainingFloat) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_countdownTimeRemainingFloat);
			m_hudManager.UpdateTimerTextCountDownP1(m_levelDisplayTimeInt);
			if (LevelSelectData.IsMultiplayer)
			{
				m_hudManager.UpdateTimerTextCountDownP2(m_levelDisplayTimeInt);
			}
		}

		m_hudManager.UpdateTimerSliderP1(m_countdownTimeRemainingFloat);
		if (LevelSelectData.IsMultiplayer)
		{
			m_hudManager.UpdateTimerSliderP2(m_countdownTimeRemainingFloat);
		}
	}



	protected override void InitHUD()
	{
		base.InitHUD();

		m_hudManager.SetItemsCountActiveP1(true);
		m_hudManager.UpdateItemsCountP1(0);
		m_hudManager.SetTimerSliderActiveP1(true);
		m_hudManager.UpdateTimerTextCountDownP1(m_timeLimit);

		if (LevelSelectData.IsMultiplayer)
		{
			m_hudManager.SetMultiplayerStatsActive();
			m_hudManager.SetItemsCountActiveP2(true);
			m_hudManager.UpdateItemsCountP2(0);
			m_hudManager.SetTimerSliderActiveP2(true);
			m_hudManager.UpdateTimerTextCountDownP2(m_timeLimit);
		}
	}

	private void OnPlayerInteractItem(OTController controller)
	{
		controller.Stats.Items++;

		// [TODO] THIS IS NOT NECESSARILY EQUAL TO P1-P2 RESPECTIVELY??? MUST DO SIMILAR TO IN EndGameMultiplayer() ???
		if (controller == m_controllers[0])
			m_hudManager.UpdateItemsCountP1(controller.Stats.Items);
		else
			m_hudManager.UpdateItemsCountP2(controller.Stats.Items);
		
		// [TODO][IMPORTANT] Use InGameStats to increase the individual count... But still keep track here for when level cleared?
		m_itemCount++;
		if (m_itemCount == LevelSelectData.ThemeData.LevelPlayInfo.TotalItems)	// [TODO] Bear in mind if we disable allowing draws, then will need to remove one item. Set TotalItems differently in that case
		{

			Debug.Log($"[b] {LevelSelectData.IsMultiplayer}");

			if (LevelSelectData.IsMultiplayer)
				EndGameMultiplayer();
			else
				EndGame(true);										// (END GAME -- win)
		}
	}



	protected override void EndGame(bool isWin)
	{
		base.EndGame(isWin);

		int totalScore = GetTotalScore(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Items);
		m_hudManager.SetEndScreenStatsSingle(totalScore, m_levelTimeElapsedFloat.RoundDP(2), m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives, true, m_controllers[0].Stats.Items);
		m_hudManager.ShowEndScreen();

		if (PlayerPrefsSystem.ScoreDisablingCheatsEnabled())
			return;

		if (SaveSystem.StatFileSaveRequired(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Items))
		{
			SaveSystem.SaveStatFileInfo(totalScore, m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Items);
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

		int totalScoreP1 = GetTotalScoreMultiplayer(controllerP1.Stats.Lives, controllerP1.Stats.Moves, controllerP1.Stats.Items);
		int totalScoreP2 = GetTotalScoreMultiplayer(controllerP2.Stats.Lives, controllerP2.Stats.Moves, controllerP2.Stats.Items);
		m_hudManager.SetEndScreenStatsMultiP1(totalScoreP1, controllerP1.Stats.Moves, controllerP1.Stats.Lives, true, controllerP1.Stats.Items);
		m_hudManager.SetEndScreenStatsMultiP2(totalScoreP2, controllerP2.Stats.Moves, controllerP2.Stats.Lives, true, controllerP2.Stats.Items);

		// [TODO][Q] Currently only basing it on #items collected! Want to base also on other stats or total score?
		// Work out in testing! See how different the scores can be, and people's reactions to if they want it to be based off everything or solely items
		// ... Or have that as an option for items mode?

		EMultiplayerResult result = EMultiplayerResult.Draw;
		if (controllerP1.Stats.Items > controllerP2.Stats.Items)
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
		}
		else if (controllerP1.Stats.Items < controllerP2.Stats.Items)
		{
			result = EMultiplayerResult.P2;
			PlayerPrefsSystem.MultiplayerAddWinP2();
		}
		else
		{
			PlayerPrefsSystem.MultiplayerAddDraw();
		}

		m_hudManager.SetWinLoseTitleMulti(result);
		m_hudManager.ShowEndScreen();
	}

	// [TODO]
	// Check if the stats are better than the existing ones, then save file if so
	//	o Items mode will have heirarchy of what to base on. E.g. items collected, then lives lost, then time, then moves

	// [TODO][IMPORTANT]
	// Work on the formula! Base on actual player testing -- not just what *I* can achieve in a level!
	// Also note that none of this needs to be passed. Only leaving like this for now to highlight that #moves are not yet taken into account

	// [TODO] Make abstract in base?
	private int GetTotalScore(float time, int lives, int moves, int items)
	{
		if (m_countdownTimeRemainingFloat == 0.0f || lives == 0 || items == 0) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float itemsRatio = (float)items / LevelSelectData.ThemeData.LevelPlayInfo.TotalItems;
		float timeRatio = time / LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit;
		const int scoreMultiplier = 10000;
		const int livesMultiplier = 1000;

		// [TODO] Work out how to involve #moves!
		// Take away the number of moves time a const?
		// or multiply by e.g. max number of moves (= e.g. 1000) minus moves taken?

		int score = Mathf.RoundToInt((gridRatio * itemsRatio * timeRatio * scoreMultiplier) - ((LevelSelectData.LivesCount - lives) * livesMultiplier));
		Debug.Log($"{score} = Mathf.RoundToInt(({gridRatio} * {itemsRatio} * {timeRatio} * {scoreMultiplier}) - (({LevelSelectData.LivesCount} - {lives}) * {livesMultiplier}))");
		return Mathf.Max(0, score);
	}

	private int GetTotalScoreMultiplayer(int lives, int moves, int items)
	{
		if (m_countdownTimeRemainingFloat == 0.0f || lives == 0 || items == 0) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float itemsRatio = (float)items / LevelSelectData.ThemeData.LevelPlayInfo.TotalItems;
		const int scoreMultiplier = 10000;
		const int livesMultiplier = 1000;

		// [TODO] How to involve #moves? If at all?

		int score = Mathf.RoundToInt((gridRatio * itemsRatio * scoreMultiplier) - ((LevelSelectData.LivesCount - lives) * livesMultiplier));
		Debug.Log($"{score} = Mathf.RoundToInt(({gridRatio} * {itemsRatio} * {scoreMultiplier}) - (({LevelSelectData.LivesCount} - {lives}) * {livesMultiplier}))");
		return Mathf.Max(0, score);
	}
}
