using UnityEngine;

public class InputTapSwipe : Input_Base
{
	private const float k_screenPercentSwipe = 0.05f;
	private float m_minSwipeDistance = 0.0f;

	private Touch m_touch;
	private Vector2 m_touchStartPosition = Vector2.zero;
	

	public InputTapSwipe()
	{
		Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
		m_minSwipeDistance = k_screenPercentSwipe * screenDimensions.magnitude;
	}

	public override bool Check(out EMovement movement)
	{
		movement = EMovement.Forward;

		if (m_inputDisabled || Input.touchCount == 0)
			return false;

		// [TODO][Q] Do we need the IsTestingLevel check anymore?
		if (LevelEditorData.IsTestingLevel == false && LevelSelectData.IsMultiplayer)
		{
			if (GetValidMultiplayerTouch(out m_touch) == false)     // If multiplayer, touch must be within specific bounds
				return false;
		}
		else
		{
			m_touch = Input.GetTouch(0);                            // Single player allows touching the screen anywhere
		}

		if (m_touch.phase == TouchPhase.Began)
		{
			m_touchStartPosition = m_touch.position;
			return false;
		}
		else if (m_touch.phase == TouchPhase.Ended)                 // [Q] Do we ever need to consider TouchPhase.Canceled?
		{
			// 'else' case is keeping movement as Forward (as defined above)
			if ((m_touch.position - m_touchStartPosition).sqrMagnitude > Mathf.Pow(m_minSwipeDistance, 2))
				movement = EMovement.Turn;
			return true;
		}

		return false;
	}
}
