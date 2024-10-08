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

		m_hudManager.SetCountStatActiveP1(true);
		m_hudManager.UpdateItemsCountP1(0);
		m_hudManager.SetTimerSliderActiveP1(true);
		m_hudManager.UpdateTimerTextCountDownP1(m_timeLimit);

		if (LevelSelectData.IsMultiplayer)
		{
			m_hudManager.SetMultiplayerStatsActive();
			m_hudManager.SetCountStatActiveP2(true);
			m_hudManager.UpdateItemsCountP2(0);
			m_hudManager.SetTimerSliderActiveP2(true);
			m_hudManager.UpdateTimerTextCountDownP2(m_timeLimit);
		}
	}

	private void OnPlayerInteractItem(OTController controller, Interactable_Base interactable)
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

		int totalScore = GetTotalScore(m_controllers[0].Stats.Items, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_countdownTimeRemainingFloat);
		m_hudManager.SetEndScreenStatsSingle(totalScore, m_levelTimeElapsedFloat.RoundDP(2), m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Items);
		
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
			else //if (m_controllers[i].Index == 1)
				controllerP2 = m_controllers[i];
		}

		int scoreP1 = GetTotalScore(controllerP1.Stats.Items, controllerP1.Stats.Lives, controllerP1.Stats.Moves, m_countdownTimeRemainingFloat);
		int scoreP2 = GetTotalScore(controllerP2.Stats.Items, controllerP2.Stats.Lives, controllerP2.Stats.Moves, m_countdownTimeRemainingFloat);
		m_hudManager.SetEndScreenStatsMultiP1(scoreP1, controllerP1.Stats.Moves, controllerP1.Stats.Lives, controllerP1.Stats.Items);
		m_hudManager.SetEndScreenStatsMultiP2(scoreP2, controllerP2.Stats.Moves, controllerP2.Stats.Lives, controllerP2.Stats.Items);
		PlayerPrefsSystem.MultiplayerAddScoreP1(scoreP1);
		PlayerPrefsSystem.MultiplayerAddScoreP2(scoreP2);

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
	}

	// [TODO]
	// Check if the stats are better than the existing ones, then save file if so
	//	o Items mode will have heirarchy of what to base on. E.g. items collected, then lives lost, then time, then moves

	// [TODO][IMPORTANT]
	// Work on the formula! Base on actual player testing -- not just what *I* can achieve in a level!
	// Also note that none of this needs to be passed. Only leaving like this for now to highlight that #moves are not yet taken into account

	// [TODO] Make abstract in base? Virtual in base as always same formula, just have ITEMS and TRAVEL multiplying by a set ratio??
	private int GetTotalScore(int items, int lives, int moves, float time)
	{
		if (m_countdownTimeRemainingFloat == 0.0f || lives == 0 || items == 0) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float itemsRatio = (float)Mathf.Pow(items / LevelSelectData.ThemeData.LevelPlayInfo.TotalItems, 2);
		int maxMoves = LevelSelectData.GridDimension * LevelSelectData.GridDimension * 5;
		const int scoreMultiplier = 1;
		const int livesMultiplier = 500;

		int score = Mathf.RoundToInt(scoreMultiplier * gridRatio * itemsRatio * ((lives * livesMultiplier) + Mathf.Max(0, maxMoves - moves) + time));
		Debug.Log($"[NEW] {score} = Mathf.RoundToInt({scoreMultiplier} * {gridRatio} * {itemsRatio} * (({lives} * {livesMultiplier}) + Mathf.Max(0, {maxMoves - moves}) + {time}))");
		return Mathf.Max(0, score);
	}
}
