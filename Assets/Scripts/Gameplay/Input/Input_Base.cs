using UnityEngine;

public abstract class Input_Base
{
	protected Bounds m_inputBounds;
	public void SetInputBounds(Bounds bounds) { m_inputBounds = bounds; }

	protected bool m_inputDisabled = true;
	public void SetInputDisabled(bool disabled) { m_inputDisabled = disabled; }

	protected EFacingDirection m_currentDirection = EFacingDirection.Up;
	public void ResetCurrentDirection(EFacingDirection direction){ m_currentDirection = direction; }


	public abstract bool Check(out EMovement movement);

	protected bool GetValidMultiplayerTouch(out Touch touch)
	{
		touch = default;
		bool validTouch = false;
		for (int i = 0; i < Input.touchCount; ++i)
		{
			if (m_inputBounds.Contains(Input.GetTouch(i).position))
			{
				touch = Input.GetTouch(i);
				validTouch = true;
				break;
			}
		}
		return validTouch;
	}
}
