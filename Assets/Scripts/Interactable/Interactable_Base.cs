using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable_Base : MonoBehaviour
{
	public UnityAction<OTController, Interactable_Base> PlayerEnterEvent;
	
	protected OTController m_playerController = null;
	protected bool m_canInteract = true;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && m_canInteract)
		{
			m_playerController = col.GetComponentInParent<OTController>();		// [NOTE] Don't like using GetComponentInParent... But think this is an okay instance to do it.
			if (PlayerEnterEvent != null)
			{
				PlayerEnterEvent(m_playerController, this);
			}
			PlayerEnter();
		}
	}

	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			PlayerExit();
		}
	}

	protected abstract void PlayerEnter();
	protected virtual void PlayerExit() { }
}
