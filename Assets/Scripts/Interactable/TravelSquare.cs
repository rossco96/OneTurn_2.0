using UnityEngine;

public class TravelSquare : Interactable_Base
{
	[SerializeField] private SpriteRenderer m_renderer;
	[SerializeField] private Color m_colorP1;
	[SerializeField] private Color m_colorP2;

	public ETravelSquareState CurrentState = ETravelSquareState.NONE;

	protected override void PlayerEnter()
	{
		if (m_playerController.Index == 0)
		{
			CurrentState = ETravelSquareState.P1;
			m_renderer.color = m_colorP1;
		}
		else
		{
			CurrentState = ETravelSquareState.P2;
			m_renderer.color = m_colorP1;
		}
	}
}
