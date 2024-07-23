public class Bomb : Interactable_Base
{
	protected override void PlayerEnter()
	{
		m_canInteract = false;
		transform.SetParent(m_playerController.GetComponentInChildren<UnityEngine.BoxCollider2D>().transform);  // Feels hacky, once again...
		transform.localRotation = UnityEngine.Quaternion.identity;
	}

	protected override void PlayerExit()
	{
		m_canInteract = true;
	}
}
