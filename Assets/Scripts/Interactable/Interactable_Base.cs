using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable_Base : MonoBehaviour
{
	public UnityAction<OTController> PlayerEnterEvent;

	protected OTController m_playerController = null;				// [TODO][Q] Is this required for children?

	private void OnTriggerEnter2D(Collider2D col)
	{
		//return;														// For the love of everything good, please uncomment and absolutely do NOT commit this
		if (col.CompareTag("Player"))
		{
			m_playerController = col.GetComponentInParent<OTController>();		// [NOTE] Don't like using GetComponentInParent... But think this is an okay instance to do it.
			if (PlayerEnterEvent != null)
			{
				PlayerEnter();
				PlayerEnterEvent(m_playerController);
			}
		}
	}

	protected abstract void PlayerEnter();
}
