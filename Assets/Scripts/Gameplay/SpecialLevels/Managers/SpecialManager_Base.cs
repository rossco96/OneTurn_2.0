using UnityEngine;

public abstract class SpecialManager_Base : MonoBehaviour
{
	[SerializeField] protected MapPropertyData m_mapPropertyData;
	[SerializeField] protected GameObject[] m_interactablePrefabs;
	
	protected SpecialInteractable_Base[] m_placedInteractables = new SpecialInteractable_Base[0];

	public virtual void Init()
	{
		for (int i = 0; i < m_interactablePrefabs.Length; ++i)
		{
			m_placedInteractables[i].SetMapPropertyData(m_mapPropertyData);
		}
	}

	public abstract GameObject GetInteractable(int posX, int posY);

	public void AddToPlacedInteractables(SpecialInteractable_Base interactable)
	{
		m_placedInteractables = m_placedInteractables.Add(interactable);
	}
}