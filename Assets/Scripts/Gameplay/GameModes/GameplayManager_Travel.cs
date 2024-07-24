using UnityEngine;

public class GameplayManager_Travel : GameplayManager
{
	private TravelSquare[] m_travelSquares;
	private int m_squaresCovered = 0;

	protected override void Start()
	{
		InitInteractableBehaviour<TravelSquare>(OnPlayerInteractTravelSquare);
		m_timeLimit = LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit;
		
		if (LevelSelectData.IsMultiplayer == false)
			m_hudManager.AssignTravelButtonP1(OnTravelModeStoppedP1);
		
		m_travelSquares = FindObjectsOfType<TravelSquare>();

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

	private void OnPlayerInteractTravelSquare(OTController controller, Interactable_Base interactable)
	{
		TravelSquare travelSquare = (TravelSquare)interactable;

		// [TODO][Q] Cache this?
		OTController controllerP1 = null;
		OTController controllerP2 = null;
		for (int i = 0; i < m_controllers.Length; ++i)
		{
			if (m_controllers[i].Index == 0)
				controllerP1 = m_controllers[i];
			else //if (m_controllers[i].Index == 1)
				controllerP2 = m_controllers[i];
		}

		switch (travelSquare.CurrentState)
		{
			case ETravelSquareState.NONE:
				controller.Stats.TravelSquares++;
				float newTravelPercent = ((100.0f * controller.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
				if (controller == controllerP1)
					m_hudManager.UpdateTravelSquaresPercentP1(newTravelPercent);
				else
					m_hudManager.UpdateTravelSquaresPercentP2(newTravelPercent);
				m_squaresCovered++;
				break;

			case ETravelSquareState.P1:
				if (LevelSelectData.IsMultiplayer && controller == controllerP2)
				{
					controllerP1.Stats.TravelSquares--;
					controller.Stats.TravelSquares++;
					float newTravelPercentP1 = ((100.0f * controllerP1.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
					float newTravelPercentP2 = ((100.0f * controller.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
					m_hudManager.UpdateTravelSquaresPercentP1(newTravelPercentP1);
					m_hudManager.UpdateTravelSquaresPercentP2(newTravelPercentP2);
				}
				break;

			case ETravelSquareState.P2:
				if (controller == controllerP1)
				{
					controller.Stats.TravelSquares++;
					controllerP2.Stats.TravelSquares--;
					float newTravelPercentP1 = ((100.0f * controller.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
					float newTravelPercentP2 = ((100.0f * controllerP2.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
					m_hudManager.UpdateTravelSquaresPercentP1(newTravelPercentP1);
					m_hudManager.UpdateTravelSquaresPercentP2(newTravelPercentP2);
				}
				break;

			default:
				break;
		}

		if (m_squaresCovered == m_travelSquares.Length)
		{
			// END GAME -- win
			if (LevelSelectData.IsMultiplayer)
			{
				EndGameMultiplayer();
			}
			else
			{
				EndGame(true);
			}
		}
	}


	// [TODO][Q] Implement more properly than this?? Regardless, need to implement EndGame methods
	private void OnTravelModeStoppedP1()
	{
		EndGame(true);
	}


	protected override void EndGame(bool isWin)
	{
		base.EndGame(isWin);

		float percentCovered = ((100.0f * m_squaresCovered) / m_travelSquares.Length).RoundDP(2);
		int totalScore = GetTotalScore(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, percentCovered);
		m_hudManager.SetEndScreenStatsSingle(totalScore, m_levelTimeElapsedFloat, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives, percentCovered);

		if (PlayerPrefsSystem.ScoreDisablingCheatsEnabled())
			return;

		if (SaveSystem.StatFileSaveRequired(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, percentCovered))
		{
			SaveSystem.SaveStatFileInfo(totalScore, m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, percentCovered);
		}
	}

	protected override void EndGameMultiplayer()
	{
		// [NOTE] Since we can (and have to) end the game using buttons, both players must press it to end the game
		// ... Or just disable the stop button for multiplayer, and play until timer runs out. Yeah. Let's do that!

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

		float percentCoveredP1 = ((100.0f * controllerP1.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
		float percentCoveredP2 = ((100.0f * controllerP2.Stats.TravelSquares) / m_travelSquares.Length).RoundDP(2);
		int scoreP1 = GetTotalScoreMultiplayer(controllerP1.Stats.Lives, controllerP1.Stats.Moves, percentCoveredP1);
		int scoreP2 = GetTotalScoreMultiplayer(controllerP2.Stats.Lives, controllerP2.Stats.Moves, percentCoveredP2);
		m_hudManager.SetEndScreenStatsMultiP1(scoreP1, controllerP1.Stats.Moves, controllerP1.Stats.Lives, percentCoveredP1);
		m_hudManager.SetEndScreenStatsMultiP2(scoreP2, controllerP2.Stats.Moves, controllerP2.Stats.Lives, percentCoveredP2);
		PlayerPrefsSystem.MultiplayerAddScoreP1(scoreP1);
		PlayerPrefsSystem.MultiplayerAddScoreP2(scoreP2);

		EMultiplayerResult result = EMultiplayerResult.Draw;
		if (percentCoveredP1 > percentCoveredP2)
		{
			result = EMultiplayerResult.P1;
			PlayerPrefsSystem.MultiplayerAddWinP1();
		}
		else if (percentCoveredP1 < percentCoveredP2)
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
	//	o Only want to update the exit time (or total score if multiple exits?) in exit mode

	// [TODO][IMPORTANT]
	// Work on the formula, based on actual player testing -- not just what *I* can achieve in a level!
	private int GetTotalScore(float time, int lives, int moves, float covered)
	{
		int maxTime = LevelSelectData.GridDimension * LevelSelectData.GridDimension;
		if (lives == 0 || time >= maxTime) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float timeRatio = (maxTime - time) / maxTime;
		const int scoreMultiplier = 10000;
		const int livesMultiplier = 1000;

		// [TODO] How to involve #moves? If at all?

		int score = Mathf.RoundToInt((gridRatio * timeRatio * scoreMultiplier) - ((LevelSelectData.LivesCount - lives) * livesMultiplier));
		Debug.Log($"{score} = Mathf.RoundToInt(({gridRatio} * {timeRatio} * {scoreMultiplier}) - (({LevelSelectData.LivesCount} - {lives}) * {livesMultiplier}))");
		return Mathf.Max(0, score);
	}

	private int GetTotalScoreMultiplayer(int lives, int moves, float covered)
	{
		if (lives == 0) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		const int scoreMultiplier = 10000;
		const int livesMultiplier = 1000;

		// [TODO] How to involve #moves? If at all?

		int score = Mathf.RoundToInt((gridRatio * scoreMultiplier) - ((LevelSelectData.LivesCount - lives) * livesMultiplier));
		Debug.Log($"{score} = Mathf.RoundToInt(({gridRatio} * {scoreMultiplier}) - (({LevelSelectData.LivesCount} - {lives}) * {livesMultiplier}))");
		return Mathf.Max(0, score);
	}
}
