using UnityEngine;

public class BridgeDetection : MonoBehaviour
{
	[SerializeField] private bool m_entrance;
	[SerializeField] private bool m_underpass;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") == false)
			return;

		OTController controller = col.GetComponentInParent<OTController>();         // "InParent" feels hacky
		if (controller == null)														// ... This also feels hacky!
			return;

		if (controller.Stats.IsOnSpecial)
		{
			if (m_entrance)
			{
				//Debug.Log("[bd_COL] M ENTRANCE -- isOnSpecial = false");
				controller.Stats.IsOnSpecial = false;
			}
			else
			{
				//Debug.Log("[bd_COL] not an entrance -- DESTROY");
				//controller.DestroyPlayerGameObject(true);                           // [Q][IMPORTANT] Always passing true?
				SendMessageUpwards("DestroyPlayer", controller);					// Is everything I do too hacky? Like, this method exists for a reason. It can't be TOO bad?
			}
		}
		else
		{
			//Debug.Log("[bd_COL] not already on special");
		}
	}
}
