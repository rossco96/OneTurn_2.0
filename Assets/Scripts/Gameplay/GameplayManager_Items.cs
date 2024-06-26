using UnityEngine;

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
		m_itemTimeRemainingFloat = m_timeLimit - m_levelTimeElapsedFloat;
		
		if (m_itemTimeRemainingFloat < 10.0f)
		{
			if (m_itemTimeRemainingFloat <= 0.0f)
			{
				m_hudManager.UpdateTimerTextItems(0.0f);
				m_hudManager.UpdateTimerSlider(0.0f);
				// END GAME -- lose
				EndGame(false, null);											// [TODO] Ideally don't want to be passing null to this???
				return;
			}
			m_hudManager.UpdateTimerTextItems(m_itemTimeRemainingFloat.RoundDP(2));
		}
		else if (Mathf.FloorToInt(m_itemTimeRemainingFloat) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_itemTimeRemainingFloat);
			m_hudManager.UpdateTimerTextItems(m_levelDisplayTimeInt);
		}

		m_hudManager.UpdateTimerSlider(m_itemTimeRemainingFloat);
	}



	protected override void InitHUD()
	{
		base.InitHUD();
		if (LevelSelectData.IsMultiplayer)
		{
			// [TODO] Set TWO lots of HUD item scores
		}
		else
		{
			m_hudManager.UpdateItemsCount(0);
		}
		m_hudManager.UpdateTimerTextItems(m_timeLimit);
	}

	private void OnPlayerInteractItem(OTController controller)
	{
		controller.Stats.AddItem();
		if (LevelSelectData.IsMultiplayer)
		{
			// [TODO] Set TWO lots of HUD item scores
		}
		else
		{
			m_hudManager.UpdateItemsCount(controller.Stats.Items);
		}
		
		// [TODO][IMPORTANT] Use InGameStats to increase the individual count... But still keep track here for when level cleared?
		m_itemCount++;
		if (m_itemCount == LevelSelectData.ThemeData.LevelPlayInfo.TotalItems)
		{
			if (LevelSelectData.IsMultiplayer)
			{
				// END GAME -- win
				int index = 0;
				int mostItems = 0;
				for (int i = 0; i < m_controllers.Length; ++i)
				{
					if (m_controllers[i].Stats.Items > mostItems)
					{
						index = i;
						mostItems = m_controllers[i].Stats.Items;
					}
				}
				EndGame(true, m_controllers[index]);
			}
			else
			{
				EndGame(true, controller);
			}
		}
	}



	protected override void EndGame(bool isWin, OTController controller)
	{
		base.EndGame(isWin, controller);

		int totalScore = GetTotalScore(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Items);
		m_hudManager.SetEndScreenStats(totalScore, m_levelTimeElapsedFloat.RoundDP(2), m_controllers[0].Stats.Moves, m_controllers[0].Stats.Lives, true, m_controllers[0].Stats.Items);

		if (SaveSystem.StatFileSaveRequired(m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Items))
		{
			SaveSystem.SaveStatFileInfo(totalScore, m_levelTimeElapsedFloat, m_controllers[0].Stats.Lives, m_controllers[0].Stats.Moves, m_controllers[0].Stats.Items);
		}
	}

	// [TODO]
	// Check if the stats are better than the existing ones, then save file if so
	//	o Items mode will have heirarchy of what to base on. E.g. items collected, then lives lost, then time, then moves

	// [TODO][IMPORTANT]
	// Work on the formula! Base on actual player testing -- not just what *I* can achieve in a level!
	// Also note that none of this needs to be passed. Only leaving like this for now to highlight that #moves are not yet taken into account
	private int GetTotalScore(float time, int lives, int moves, int items)
	{
		if (m_itemTimeRemainingFloat == 0.0f || lives == 0 || items == 0) return 0;

		float gridRatio = (float)(LevelSelectData.GridDimension * LevelSelectData.GridDimension) / (17 * 17);
		float itemsRatio = (float)items / LevelSelectData.ThemeData.LevelPlayInfo.TotalItems;
		float timeRatio = time / LevelSelectData.ThemeData.LevelPlayInfo.ItemTimeLimit;
		const int scoreMultiplier = 10000;
		const int livesMultiplier = 1000;

		// [TODO] How to involve #moves? If at all?

		int score = Mathf.RoundToInt((gridRatio * itemsRatio * timeRatio * scoreMultiplier) - ((LevelSelectData.LivesCount - lives) * livesMultiplier));
		Debug.Log($"{score} = Mathf.RoundToInt(({gridRatio} * {itemsRatio} * {timeRatio} * {scoreMultiplier}) - (({LevelSelectData.LivesCount} - {lives}) * {livesMultiplier}))");
		return Mathf.Max(0, score);
	}
}
