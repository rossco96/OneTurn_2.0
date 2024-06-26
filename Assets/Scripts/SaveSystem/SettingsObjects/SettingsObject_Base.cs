using UnityEngine;

public abstract class SettingsObject_Base : MonoBehaviour
{
	[SerializeField] protected SettingsData_Base m_data;

	private void OnEnable()
	{
		ApplyExistingValue();
	}

	public abstract void ApplyExistingValue();
	public abstract void SetNewValue();
}
