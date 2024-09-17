public class SpecialInteractableBridgeCross : SpecialInteractableBridge_Base
{
	protected override void OnMapPropertyDataSet() { }
	protected override void SetRotation() { }
	protected override void PlayerEnter() 
	{
		if (m_playerController.GetPlayerLocalPosition().z != 0)
		{
			DestroyPlayer(m_playerController);
			return;
		}
	}
	protected override void PlayerExit() { }
}
