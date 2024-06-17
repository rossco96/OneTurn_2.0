using UnityEngine;

public class OTController : MonoBehaviour
{
	const float k_screenPercentSwipe = 0.05f;
	
	private float m_minSwipeDistance = 0.0f;

	private bool m_inputDisabled = true;
	private bool m_inputDisabledIsDirty = false;
	private bool m_inputDisabledPendingValue = false;
	
	private Touch m_touch;
	private Vector2 m_touchStartPosition = Vector2.zero;

	private Bounds m_inputBounds;
	public void SetInputBounds(Bounds bounds) { m_inputBounds = bounds; }
	
	private float m_gridSizeMultiplier = 1.0f;

	private ETurnDirection m_turnDirection = ETurnDirection.Right;

	private bool m_isMultiplayer = false;

	private GameObject m_playerPrefab = null;
	private GameObject m_player = null;

	private InGameStats m_stats = new InGameStats();
	public InGameStats Stats => m_stats;



	private void Awake()
	{
		Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
		m_minSwipeDistance = k_screenPercentSwipe * screenDimensions.magnitude;
		m_gridSizeMultiplier = LevelSelectData.GridSizeMultiplier;
		m_isMultiplayer = (LevelEditorData.IsTestingLevel == false && LevelSelectData.IsMultiplayer);
		m_turnDirection = LevelSelectData.TurnDirection;
		m_stats.SetLives(LevelSelectData.LivesCount);					// [TODO][NOTE] Make sure we do NOT remove the life in GameplayManager_LevelEditor
	}

	private void Update()
    {
		CheckInput();
    }

	// [TODO] Do not like that I'm using LateUpdate for this!?
	private void LateUpdate()
	{
		if (m_inputDisabledIsDirty)
		{
			m_inputDisabledIsDirty = false;
			m_inputDisabled = m_inputDisabledPendingValue;
		}
	}



	private void CheckInput()
	{
		if (m_inputDisabled || Input.touchCount == 0)
			return;

		if (m_isMultiplayer)
		{
			if (GetValidMultiplayerTouch(out m_touch) == false)		// If multiplayer, touch must be within specific bounds
				return;
		}
		else
		{
			m_touch = Input.GetTouch(0);							// Single player allows touching the screen anywhere
		}

		if (m_touch.phase == TouchPhase.Began)
		{
			m_touchStartPosition = m_touch.position;
		}
		else if (m_touch.phase == TouchPhase.Ended)					// [TODO] Do we ever need to consider TouchPhase.Canceled?
		{
			if ((m_touch.position - m_touchStartPosition).sqrMagnitude > Mathf.Pow(m_minSwipeDistance, 2))
				Turn();
			else
				MoveForward();
		}
	}

	private bool GetValidMultiplayerTouch(out Touch touch)
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

	private void Turn()
	{
		if (m_turnDirection == ETurnDirection.Right)
			m_player.transform.Rotate(0, 0, -90);
		else
			m_player.transform.Rotate(0, 0, 90);
		MoveForward();
	}

	private void MoveForward()
	{
		m_player.transform.position += m_gridSizeMultiplier * m_player.transform.up;
		Stats.AddMove();
	}



	// [TODO][Q] Will eventually want to remove this method, and call via animation instead?
	public void SetInputDisabled(bool disabled)
	{
		// [TODO] THIS IS SUPER HACKY -- and don't like that I'm using LateUpdate() either...
		// ... But it seems this is how the touch input works?
		if (m_inputDisabled)
		{
			m_inputDisabledPendingValue = disabled;
			m_inputDisabledIsDirty = true;
		}
		else
		{
			m_inputDisabled = disabled;
		}
	}



	public void SetPlayerPrefab(GameObject player)
	{
		m_playerPrefab = player;
	}

	public void RespawnPlayer()
	{
		m_player = Instantiate(m_playerPrefab, transform);
	}

	public void DestroyPlayerGameObject()
	{
		// [NOTE] Moved to inside GameplayManager.OnPlayerInteractWall()
		//m_inputDisabled = true;							// [TODO] Need to call SetInputDisabled(true) ???

		// [TODO] Play death animation and sound
		// [Q] Should we play sound via GameplayManager->AudioManager ??? Or through here
		// [A] Through here - can have m_deathSound as var and pass to AudioManager to play the clip
		// ... Or, if animating death, play through there? Would need separate function in here anyway

		// [TEMP HERE?]
		Destroy(m_player);
		m_player = null;
	}
}
