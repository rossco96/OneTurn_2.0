using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable_Base : MonoBehaviour, IInteractable
{
	public UnityAction<OTController, Interactable_Base> PlayerEnterEvent;
	
	protected OTController m_playerController = null;								// [TODO][Q] Is this required for children?

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			m_playerController = col.GetComponentInParent<OTController>();		// [NOTE] Don't like using GetComponentInParent... But think this is an okay instance to do it.
			if (PlayerEnterEvent != null)
			{
				PlayerEnterEvent(m_playerController, this);
			}
			PlayerEnter();
		}
	}

	protected abstract void PlayerEnter();
}

public interface IInteractable
{
}
