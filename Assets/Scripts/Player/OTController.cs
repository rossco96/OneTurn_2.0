using UnityEngine;

public class OTController : MonoBehaviour
{
	private bool m_inputDisabled = true;
	private bool m_inputDisabledIsDirty = false;
	private bool m_inputDisabledPendingValue = false;

	[SerializeField] private SettingsDataString m_inputSettingsKey;
	private EInputMode m_inputMode = EInputMode.TapSwipe;
	private Input_Base m_input = null;

	public void SetInputBounds(Bounds bounds)
	{
		if (m_input == null)
		{
			Debug.LogError("[OTController::SetInputBounds] m_input is null!");
			return;
		}
		m_input.SetInputBounds(bounds);
	}
	
	private float m_gridSizeMultiplier = 1.0f;

	private ETurnDirection m_turnDirection = ETurnDirection.Right;

	private bool m_isMultiplayer = false;

	private GameObject m_playerPrefab = null;
	private GameObject m_player = null;

	private InGameStats m_stats = new InGameStats();
	public InGameStats Stats => m_stats;

	public int Index = 0;



	private void Awake()
	{
		Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
		m_gridSizeMultiplier = LevelSelectData.GridSizeMultiplier;
		m_isMultiplayer = (LevelEditorData.IsTestingLevel == false && LevelSelectData.IsMultiplayer);
		m_turnDirection = LevelSelectData.TurnDirection;
		m_stats.SetLives(LevelSelectData.LivesCount);                   // [TODO][NOTE] Make sure we do NOT remove the life in GameplayManager_LevelEditor

		InitInput();
	}

	private void InitInput()
	{
		string inputSettings = SettingsSystem.GetValue(m_inputSettingsKey.Key);
		for (int i = 0; i < System.Enum.GetValues(typeof(EInputMode)).Length; ++i)
		{
			if ($"{(EInputMode)i}" == inputSettings)
			{
				m_inputMode = (EInputMode)i;
				break;
			}
		}

		switch (m_inputMode)
		{
			case EInputMode.TapSwipe:			m_input = new InputTapSwipe();			break;
			case EInputMode.SwipeDirectional:	m_input = new InputSwipeDirectional();	break;
			case EInputMode.Buttons:			m_input = new InputButtons();			break;
			default:																	break;
		}
	}


	private void Update()
    {
		if (m_input.Check(out EMovement movement))
		{
			switch (movement)
			{
				case EMovement.Forward:	MoveForward();	break;
				case EMovement.Turn:	Turn();			break;
				default:								break;
			}
		}
    }

	// [TODO] Do not like that I'm using LateUpdate for this!?
	private void LateUpdate()
	{
		if (m_inputDisabledIsDirty)
		{
			m_inputDisabledIsDirty = false;
			m_inputDisabled = m_inputDisabledPendingValue;
			m_input.SetInputDisabled(m_inputDisabled);
		}
	}


	// UI 'buttons' input style
	public void TurnButton()
	{
		Turn();
	}
	// UI 'buttons' input style
	public void MoveForwardButton()
	{
		MoveForward();
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
			m_input.SetInputDisabled(disabled);
		}
	}



	public void SetPlayerPrefab(GameObject player)
	{
		m_playerPrefab = player;
	}

	public void RespawnPlayer()
	{
		m_player = Instantiate(m_playerPrefab, transform);
		if (m_inputMode == EInputMode.SwipeDirectional)
		{
			((InputSwipeDirectional)m_input).ResetCurrentDirection();
		}
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
