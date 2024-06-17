using UnityEngine;

public class GameplayManager_Exit : GameplayManager
{
	protected override void Start()
	{
		base.Start();
		InitInteractableBehaviour<Exit>(OnPlayerInteractExit);
	}

	protected override void UpdateTimer()
	{
		m_levelTimeFloat = Time.time - m_levelStartTime - m_totalTimePaused;
		if (Mathf.FloorToInt(m_levelTimeFloat) != m_levelTimeInt)
		{
			m_levelTimeInt = Mathf.FloorToInt(m_levelTimeFloat);
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



	protected override void EndGame(bool isWin, OTController controller)
	{
		base.EndGame(isWin, controller);

		int totalScore = GetTotalScore(m_levelTimeFloat, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives);
		m_hudManager.SetEndScreenStats(totalScore, m_levelTimeFloat, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives);

		if (SaveSystem.StatFileSaveRequired())
		{
			SaveSystem.SaveStatFileInfo(totalScore, m_controllers[0].Stats.Lives, m_levelTimeFloat, m_controllers[0].Stats.Moves);
		}
	}

	// [TODO]
	// Check if the stats are better than the existing ones, then save file if so
	//	o Only want to update the exit time (or total score if multiple exits?) in exit mode

	// [TODO][IMPORTANT]
	// Work on the formula, based on actual player testing -- not just what *I* can achieve in a level!
	private int GetTotalScore(float time, int moves, int lives)
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
