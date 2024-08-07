using UnityEngine;

public class SpecialManagerBooster : SpecialManager_Base
{
	[SerializeField] private float m_boosterSpeed;

	public override void Init()
	{
		base.Init();
		for (int i = 0; i < m_placedInteractables.Length; ++i)
		{
			((SpecialInteractableBooster)m_placedInteractables[i]).Speed = m_boosterSpeed;
		}
	}
}