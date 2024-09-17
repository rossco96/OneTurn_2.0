using UnityEngine;

public class SpecialManagerBooster : SpecialManager_Base
{
	[SerializeField] private float m_boosterSpeed;

	private int m_placementCount = 0;

	public override void Init()
	{
		base.Init();
		for (int i = 0; i < m_placedInteractables.Length; ++i)
		{
			((SpecialInteractableBooster)m_placedInteractables[i]).Speed = m_boosterSpeed;
		}
	}

	public override GameObject GetInteractable(int posX, int posY)
	{
		return m_interactablePrefabs[m_placementCount++];
	}
}