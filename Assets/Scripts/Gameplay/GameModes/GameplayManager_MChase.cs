using UnityEngine;

// [TODO] Ensure we're playing through this twice. Once as P1 being the chaser, then as P2 being the chaser.
// Then can base winner off least number of chaser moves (or most number of the one being chased... Same thing)
// --> Allow timer as well (optional?) so can be timed out and not caught
// (how to score that? would we also need to make sure the turn based moves has a limit as well, otherwise can just not move and other player can't have their go!)

public class GameplayManager_MChase : GameplayManager
{
	public int MovesChaser = 0;
	public int MovesTarget = 0;

	protected override void Start()
	{
		InitInteractableBehaviour<Chaser>(OnPlayerInteractChaser);
		m_hudManager.MovesChaser = MovesChaser;
		m_hudManager.MovesTarget = MovesTarget;
		if (LevelSelectData.ChaseIsRoundTwo == false)
			m_hudManager.AssignMChaseNextRound(StartRoundTwo);

		base.Start();
		
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			m_controllers[i].AssignOnMove(CheckOnMove);
			m_controllers[i].ResetChaseMoves((m_controllers[i].Stats.IsChaser) ? MovesChaser : 0);
		}
	}

	protected override void UpdateTimer()
	{
		m_levelTimeElapsedFloat = Time.time - m_levelStartTime - m_totalTimePaused;
		if (Mathf.FloorToInt(m_levelTimeElapsedFloat) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_levelTimeElapsedFloat);
			m_hudManager.UpdateTimerTextCountUpP1(m_levelDisplayTimeInt);
			m_hudManager.UpdateTimerTextCountUpP2(m_levelDisplayTimeInt);
		}
	}


	// [TODO][IMPORTANT] Turn 'MovesText' into 'ExtraInfoText' and also use for counting number of remaining moves in this game mode
	protected override void InitHUD()
	{
		base.InitHUD();
		
		m_hudManager.SetMultiplayerStatsActive();

		m_hudManager.SetCountStatActiveP1(true);
		m_hudManager.InitChaseMovesP1(LevelSelectData.ChaseIsRoundTwo == false);
		m_hudManager.SetTimerSliderActiveP1(false);
		m_hudManager.UpdateTimerTextCountUpP1(0);

		m_hudManager.SetCountStatActiveP2(true);
		m_hudManager.InitChaseMovesP2(LevelSelectData.ChaseIsRoundTwo);
		m_hudManager.SetTimerSliderActiveP2(false);
		m_hudManager.UpdateTimerTextCountUpP2(0);
	}


	private void CheckOnMove(OTController controller)
	{
		if (controller.Index == 0)
			m_hudManager.UpdateChaseMovesP1(controller.Stats.ChaseMovesLeft);
		else //if (controller.Index == 1)
			m_hudManager.UpdateChaseMovesP2(controller.Stats.ChaseMovesLeft);

		if (controller.Stats.ChaseMovesLeft == 0)
		{
			for (int i = 0; i < m_controllers.Length; ++i)
			{
				if (m_controllers[i] == controller) continue;

				int moves = (m_controllers[i].Stats.IsChaser) ? MovesChaser : MovesTarget;
				m_controllers[i].ResetChaseMoves(moves);
				if (m_controllers[i].Index == 0)
					m_hudManager.InitChaseMovesP1(true);
				else //if (m_controllers[i].Index == 1)
					m_hudManager.InitChaseMovesP2(true);
			}
		}
	}

	private void OnPlayerInteractChaser(OTController controller, Interactable_Base interactable)
	{
		if (controller.Stats.IsChaser) return;

		// END GAME -- win (switch who is the chaser if it's the first round, otherwise END and compare number of moves!
		if (LevelSelectData.ChaseIsRoundTwo == false)
			EndRoundOne();
		else
			EndGameMultiplayer();
	}

	private void EndRoundOne()
	{
		base.EndGameMultiplayer();
	}

	private void StartRoundTwo()
	{
		LevelSelectData.ChaseIsRoundTwo = true;
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			if (m_controllers[i].Index == 0)
			{
				LevelSelectData.ChaseStatsP1Lives = m_controllers[i].Stats.Lives;
				LevelSelectData.ChaseStatsP1Moves = m_controllers[i].Stats.Moves;
				LevelSelectData.ChaseStatsP1Time = m_levelTimeElapsedFloat;
			}
		}
		m_hudManager.RetryLevel();
	}

	protected override void EndGameMultiplayer()
	{
		base.EndGameMultiplayer();
		
		OTController controllerP2 = null;
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			if (m_controllers[i].Index == 1)
				controllerP2 = m_controllers[i];
		}

		// NOTE need multiple nested IFs due to hierarchy of win-lose-draw condition

		int scoreP1 = GetTotalScore(LevelSelectData.ChaseStatsP1Lives, LevelSelectData.ChaseStatsP1Moves, LevelSelectData.ChaseStatsP1Time);
		int scoreP2 = GetTotalScore(controllerP2.Stats.Lives, controllerP2.Stats.Moves, m_countdownTimeRemainingFloat);
		m_hudManager.SetEndScreenStatsMultiP1(scoreP1, LevelSelectData.ChaseStatsP1Moves, LevelSelectData.ChaseStatsP1Lives);
		m_hudManager.SetEndScreenStatsMultiP2(scoreP2, controllerP2.Stats.Moves, controllerP2.Stats.Lives);
		PlayerPrefsSystem.MultiplayerAddScoreP1(scoreP1);
		PlayerPrefsSystem.MultiplayerAddScoreP2(scoreP2);

		EMultiplayerResult result = EMultiplayerResult.Draw;
		if (LevelSelectData.ChaseStatsP1Moves < controllerP2.Stats.Moves)
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
		}
		else if (LevelSelectData.ChaseStatsP1Moves > controllerP2.Stats.Moves)
		{
			result = EMultiplayerResult.P2;
			PlayerPrefsSystem.MultiplayerAddWinP2();
		}
		else if (LevelSelectData.ChaseStatsP1Lives > controllerP2.Stats.Lives)
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
		}
		else if (LevelSelectData.ChaseStatsP1Lives < controllerP2.Stats.Lives)
		{
			result = EMultiplayerResult.P2;
			PlayerPrefsSystem.MultiplayerAddWinP2();
		}
		else
		{
			PlayerPrefsSystem.MultiplayerAddDraw();
		}
		// If do NOT want to allow drawing, uncomment the below!
		/*
		else if (LevelSelectData.ChaseStatsP1Time < m_countdownTimeRemainingFloat)
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
		}
		else if (LevelSelectData.ChaseStatsP1Time > m_countdownTimeRemainingFloat)
		{
			result = EMultiplayerResult.P2;
			PlayerPrefsSystem.MultiplayerAddWinP2();
		}
		//*/

		m_hudManager.SetWinLoseTitleMulti(result);
	}


	// [TODO]
	// Check if the stats are better than the existing ones, then save file if so
	//	o Only want to update the exit time (or total score if multiple exits?) in exit mode

	// [TODO][IMPORTANT]
	// Work on the formula, based on actual player testing -- not just what *I* can achieve in a level!
	private int GetTotalScore(int lives, int moves, float time)
	{
		int maxTime = LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit;
		if (lives == 0 || time >= maxTime) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float timeLeft = maxTime - time;
		const int scoreMultiplier = 1;
		const int livesMultiplier = 500;

		int score = Mathf.RoundToInt(scoreMultiplier * gridRatio * ((lives * livesMultiplier) + (moves * 10.0f) + timeLeft));
		Debug.Log($"[NEW] {score} = Mathf.RoundToInt({scoreMultiplier} * {gridRatio} * (({lives} * {livesMultiplier}) + ({moves} * 10.0f) + {timeLeft}))");
		return Mathf.Max(0, score);
	}
}
