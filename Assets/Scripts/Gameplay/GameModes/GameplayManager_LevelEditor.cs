using UnityEngine;

public class GameplayManager_LevelEditor : GameplayManager
{
	#region Start / Init / Update
	protected override void Start()
	{
		base.Start();

		InitInteractableBehaviour<Border>(OnPlayerInteractWallLevelEditor);
		if (LevelEditorData.AllowMoveThroughWalls == false)
			InitInteractableBehaviour<Wall>(OnPlayerInteractWallLevelEditor);

		if (LevelSelectData.GameMode == EGameMode.Items)
			InitInteractableBehaviour<Item>(OnPlayerInteractItem);
		else if (LevelSelectData.GameMode == EGameMode.Items)
			InitInteractableBehaviour<Exit>(OnPlayerInteractExit);
	}

	protected override void InitHUD()
	{
		base.InitHUD();
		m_hudManager.SetItemsCountActiveP1(false);
		m_hudManager.SetTimerSliderActiveP1(false);
	}

	protected override void UpdateTimer()
	{
		m_levelTimeElapsedFloat = Time.time - m_levelStartTime - m_totalTimePaused;
		if (Mathf.FloorToInt(m_levelTimeElapsedFloat) != m_levelDisplayTimeInt)
		{
			m_levelDisplayTimeInt = Mathf.FloorToInt(m_levelTimeElapsedFloat);
			m_hudManager.UpdateTimerTextExitP1(m_levelDisplayTimeInt);
		}
	}
	#endregion


	#region OnPlayerInteracts
	private void OnPlayerInteractWallLevelEditor(OTController controller)
	{
		controller.SetInputDisabled(true);
		controller.DestroyPlayerGameObject();
		m_hudManager.UpdateLivesCountP1(controller.Stats.Lives);

		// [NOTE] This should be done after the death animation is complete! Need another callback...
		// [NOTE] Need also a spawn animation, and THEN can resume control of player
		controller.RespawnPlayer();

		// [TODO] DO THIS ELSEWHERE! Needs to be done after death animation complete
		controller.SetInputDisabled(false);
	}

	private void OnPlayerInteractItem(OTController controller)
	{
		controller.Stats.Items++;
		m_hudManager.UpdateItemsCountP1(controller.Stats.Items);

		// [TODO][IMPORTANT] Use InGameStats to increase the individual count... But still keep track here for when level cleared?
		m_itemCount++;
		if (m_itemCount == LevelSelectData.ThemeData.LevelPlayInfo.TotalItems)
		{
			//EndGame(true, controller);
			// [TODO][IMPORTANT] Show the level editor finish popup -- prompt if wanting to continue, reset, or return to the level editor
		}
	}

	private void OnPlayerInteractExit(OTController controller)
	{
		// [TODO]
		// SHOW A LEVEL_EDITOR POPUP
		// --> Say win/lose and ask if they'd like to continue playing, go again, or return to the editor

		// [IMPORTANT][TODO] Must see if player is facing the same way as the exit specifies!
		// If not, respawn (losing condition for lives == 0 in there)
		// Otherwise then yeah, obviously win condition

		// END GAME -- win
		//EndGame(true, controller);
	}
	#endregion
}
