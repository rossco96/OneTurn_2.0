using UnityEngine;

public class InputSwipeDirectional : Input_Base
{
	private const float k_screenPercentSwipe = 0.05f;
	private float m_minSwipeDistance = 0.0f;

	private EFacingDirection m_currentDirection = EFacingDirection.Up;

	private Touch m_touch;
	private Vector2 m_touchStartPosition = Vector2.zero;

	public InputSwipeDirectional()
	{
		Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
		m_minSwipeDistance = k_screenPercentSwipe * screenDimensions.magnitude;
		ResetCurrentDirection();
	}

	public void ResetCurrentDirection()
	{
		// [TODO][IMPORTANT] THINK MULTIPLAYER! Don't just want to select [0], must look at player index.
		m_currentDirection = (LevelSelectData.TurnDirection == ETurnDirection.Right)
			? LevelSelectData.MapData.PlayerSpawnDirectionRight[0]
			: LevelSelectData.MapData.PlayerSpawnDirectionLeft[0];
	}

	public override bool Check(out EMovement movement)
	{
		movement = EMovement.Forward;

		if (m_inputDisabled || Input.touchCount == 0)
			return false;

		// [TODO][Q] Do we need the IsTestingLevel check anymore? Think I can delete.
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
			{
				bool validSwipe = IsValidSwipe(out movement);
				return validSwipe;
			}
		}

		return false;
	}

	private bool IsValidSwipe(out EMovement movement)
	{
		movement = EMovement.Forward;

		Vector2 swipeVector = m_touch.position - m_touchStartPosition;
		EFacingDirection swipeDirection = EFacingDirection.Up;
		if (Mathf.Abs(swipeVector.y) > Mathf.Abs(swipeVector.x))
		{
			if (swipeVector.y > 0)	swipeDirection = EFacingDirection.Up;
			else					swipeDirection = EFacingDirection.Down;
		}
		else
		{
			if (swipeVector.x > 0)	swipeDirection = EFacingDirection.Right;
			else					swipeDirection = EFacingDirection.Left;
		}

		if (swipeDirection == m_currentDirection)
		{
			return true;
		}
		else
		{
			EFacingDirection validNewDirection = (LevelSelectData.TurnDirection == ETurnDirection.Right)
				? (EFacingDirection)(((int)m_currentDirection + 1) % 4)
				: (EFacingDirection)(((int)m_currentDirection - 1) % 4);

			if (swipeDirection == validNewDirection)
			{
				movement = EMovement.Turn;
				m_currentDirection = swipeDirection;
				return true;
			}
		}

		return false;
	}
}
