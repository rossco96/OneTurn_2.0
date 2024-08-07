using UnityEngine;

public class SpecialInteractableBooster : SpecialInteractable_Base
{
	private const float k_distanceFromDestination = 0.5f;

	/*[HideInInspector]*/ public float Speed = 5.0f;						// [HII] commented out for testing, to ensure value change
	//public UnityEngine.Events.UnityAction OnPlayerEnter;

	private bool m_isMovingPlayer = false;
	private Vector3 m_destination = Vector3.zero;

	protected override void OnMapPropertyDataSet()
	{
		SetDestination();
	}

	public override void PlayerEnter()
	{
		//OnPlayerEnter();
		m_playerController.SetInputDisabled(true);
		m_isMovingPlayer = true;
	}

	private void Update()												// [Q] Should put into FixedUpdate? And move the rigidbody2d rather than the transform?
	{
		if (m_isMovingPlayer)
			MovePlayer();
	}

	private void MovePlayer()
	{
		Debug.Log($"([{m_playerController.GetPlayerPosition()}] - [{m_destination}]) . GetMagnitude2D() = [{(m_playerController.GetPlayerPosition() - m_destination).GetMagnitude2D()}] ... < {k_distanceFromDestination} ???");
		if ((m_playerController.GetPlayerPosition() - m_destination).GetMagnitude2D() < k_distanceFromDestination)
		{
			Debug.LogWarning("STOP");
			m_isMovingPlayer = false;
			m_playerController.SetInputDisabled(false);
			m_playerController.SetPlayerPosition(m_destination);
			return;
		}
		m_playerController.ForceMovePlayer(Speed * Time.deltaTime * transform.up);
	}



	private void SetDestination()
	{
		int i = (int)FacingDirection;
		int xDiff = (i % 2 == 0) ? 0 : 1;
		int yDiff = (i % 2 == 0) ? 1 : 0;
		xDiff *= (i / 2 == 0) ? 1 : -1;
		yDiff *= (i / 2 == 0) ? 1 : -1;

		(int, int) currentCoords = (m_gridPosX, m_gridPosY);
		TraverseSquare nextSquare = new TraverseSquare(m_gridPosX, m_gridPosY, m_mapPropertyData, LevelSelectData.MapData.GridLayout);

		int count = 0;
		while (nextSquare.SurroundingWalls.Contains((EFacingDirection)i) == false)
		{
			currentCoords = (currentCoords.Item1 + xDiff, currentCoords.Item2 + yDiff);
			nextSquare = new TraverseSquare(currentCoords.Item1, currentCoords.Item2, m_mapPropertyData, LevelSelectData.MapData.GridLayout);
			count++;
		}

		//float gridSizeMultiplier = (Camera.main.aspect * Camera.main.orthographicSize * 2.0f) / (LevelSelectData.ThemeData.LevelPlayInfo.GridDimension + 0.2f);
		m_destination = transform.localPosition + new Vector3(count * xDiff, count * yDiff, 0.0f);
		//m_destinationOffset = new Vector3(count * xDiff, count * yDiff, 0.0f);
	}
}
