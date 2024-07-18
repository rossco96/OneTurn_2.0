using UnityEngine;

public class TravelSquare : Interactable_Base
{
	[SerializeField] private SpriteRenderer m_renderer;
	[SerializeField] private Color m_colorP1;
	[SerializeField] private Color m_colorP2;

	protected override void PlayerEnter()
	{
		if (m_playerController.Index == 0)
			m_renderer.color = m_colorP1;
		else
			m_renderer.color = m_colorP1;
	}
}
