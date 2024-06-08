using UnityEngine;

public class GameplayManager_Items : GameplayManager
{
	private int m_timeLimit = 0;
	private float m_levelTimeCountDown = 0.0f;
	private int m_levelTimeInt = 0;

	private int m_totalItems = 0;
	private int m_itemCount = 0;



	protected override void Start()
	{
		InitInteractableBehaviour<Item>(OnPlayerInteractItem);
		m_timeLimit = LevelSelectData.ThemeData.TimeLimit;
		m_totalItems = FindObjectsOfType<Item>().Length;                        // [TODO] Change... Is this too hacky?
		base.Start();

		// [TODO][DELETE] TIMER TESTING ONLY!
		//m_timeLimit = 15;
	}



	protected override void UpdateTimer()
	{
		m_levelTimeCountDown = m_timeLimit - (Time.time - m_levelStartTime - m_totalTimePaused);
		
		if (m_levelTimeCountDown < 10.0f)
		{
			if (m_levelTimeCountDown <= 0.0f)
			{
				m_hudManager.UpdateTimerTextItems(0.0f);
				m_hudManager.UpdateTimerSlider(0.0f, m_timeLimit);
				// END GAME -- lose
				EndGame(false, null);											// [TODO] Ideally don't want to be passing null to this???
				return;
			}

			m_hudManager.UpdateTimerTextItems(m_levelTimeCountDown.RoundDP(2));
		}
		else if (Mathf.CeilToInt(m_levelTimeCountDown) != m_levelTimeInt)
		{
			m_levelTimeInt = Mathf.CeilToInt(m_levelTimeCountDown);
			m_hudManager.UpdateTimerTextItems(m_levelTimeInt);
		}

		m_hudManager.UpdateTimerSlider(m_levelTimeCountDown, m_timeLimit);
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
			m_hudManager.UpdateItemsCount(0, m_totalItems);
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
			m_hudManager.UpdateItemsCount(controller.Stats.Items, m_totalItems);
		}
		
		m_itemCount++;													// [TODO][IMPORTANT] Use InGameStats to increase the individual count... But still keep track here for when cleared?
		if (m_itemCount == m_totalItems)
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
}
