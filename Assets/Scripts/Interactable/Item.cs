public class Item : Interactable_Base
{
	protected override void PlayerEnter()
	{
		Destroy(gameObject);
	}
}
