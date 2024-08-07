using UnityEngine;

public abstract class SpecialManager_Base : MonoBehaviour
{
	[SerializeField] private MapPropertyData m_mapPropertyData;
	[SerializeField] private GameObject[] m_interactablePrefabs;
	
	protected SpecialInteractable_Base[] m_placedInteractables = new SpecialInteractable_Base[0];

	public virtual void Init()
	{
		for (int i = 0; i < m_interactablePrefabs.Length; ++i)
		{
			m_placedInteractables[i].SetMapPropertyData(m_mapPropertyData);
		}
	}

	public GameObject GetInteractablePrefabAtIndex(int index)
	{
		return m_interactablePrefabs[index];
	}

	public void AddToPlacedInteractables(SpecialInteractable_Base interactable)
	{
		m_placedInteractables = m_placedInteractables.Add(interactable);
	}



	public void SpawnInteractable(int index, int posX, int posY, Transform gameSpaceParent)
	{
		GameObject placedInteractable = Instantiate(m_interactablePrefabs[index], new Vector2(posX, posY), Quaternion.identity, gameSpaceParent);
		int zRotation = 0;
		switch (m_interactablePrefabs[index].GetComponent<SpecialInteractable_Base>().FacingDirection)
		{
			case EFacingDirection.Right:
				zRotation = -90; break;
			case EFacingDirection.Down:
				zRotation = 180; break;
			case EFacingDirection.Left:
				zRotation = 90; break;
			default:
				break;
		}
		placedInteractable.transform.Rotate(0, 0, zRotation);
	}
}