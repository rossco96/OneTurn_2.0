using UnityEngine;
//using UnityEngine.Events;

public abstract class SpecialInteractable_Base : MonoBehaviour
{
	protected MapPropertyData m_mapPropertyData;
	public void SetMapPropertyData(MapPropertyData mapPropertyData)
	{
		m_mapPropertyData = mapPropertyData;
		OnMapPropertyDataSet();
	}
	protected abstract void OnMapPropertyDataSet();

	public void SetGridPos(int x, int y) { m_gridPosX = x; m_gridPosY = y; }
	protected int m_gridPosX = 0;
	protected int m_gridPosY = 0;

	public EFacingDirection FacingDirection = EFacingDirection.Up;

	//public UnityAction<OTController, Interactable_Base> PlayerEnterEvent;
	protected OTController m_playerController = null;
	//protected bool m_canInteract = true;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player")/* && m_canInteract*/)
		{
			m_playerController = col.GetComponentInParent<OTController>();      // [NOTE] Don't like using GetComponentInParent... But think this is an okay instance to do it.
			//if (PlayerEnterEvent != null)
			//{
			//	PlayerEnterEvent(m_playerController, this);
			//}
			PlayerEnter();
		}
	}

	public abstract void PlayerEnter();
}
