/*

using UnityEngine;
using UnityEngine.Events;

// [TODO][Q] What to do if multiple spawn points?
// Have SpawnPointManager / MultiSpawnPointManager / MultiSpawnPoint ???

public class SpawnPoint : MonoBehaviour
{
	[SerializeField] private GameObject m_playerPrefab;

	public UnityAction<OTController/*, int* /> OnPlayerRespawn;

	// [TODO] This should still be relevant as, if multiplayer, we can respawn at our corresponding SpawnPoint
	private int m_playerIndex = 0;
	public void SetPlayerIndex(int index)
	{
		m_playerIndex = index;
		if (m_playerController != null)
			m_playerController.SetPlayerIndex(index);
	}
	public int PlayerIndex => m_playerIndex;

	private Bounds m_playerInputBounds;
	public void SetPlayerBounds(Bounds bounds)
	{
		m_playerInputBounds = bounds;
		if (m_playerController != null)
			m_playerController.SetInputBounds(bounds);
	}

	private OTController m_playerController = null;
	public void SpawnPlayerController()
	{
		GameObject playerGameObject = Instantiate(m_playerPrefab, transform);

		m_playerController = playerGameObject.GetComponent<OTController>();
		//m_playerController.SetPlayerIndex(m_playerIndex);								// Delete?
		m_playerController.SetInputBounds(m_playerInputBounds);

		InGameStats stats = playerGameObject.GetComponent<InGameStats>();
		//stats.SetLives(LevelSelectData.LivesCount);									// Implement!

		//if (OnPlayerRespawn != null)													// Delete? Or re-imagine? Or just keep??
		//	OnPlayerRespawn(m_playerController/*, m_playerIndex* /);
	}

	
	
	/*
	private OTController m_playerController = null;

	private void Awake()
	{
		RespawnPlayer();
	}

	public void RespawnPlayer()
	{
		GameObject playerGameObject = Instantiate(m_playerPrefab, transform);

		m_playerController = playerGameObject.GetComponent<OTController>();
		m_playerController.SetPlayerIndex(m_playerIndex);
		m_playerController.SetInputBounds(m_playerInputBounds);
		
		if (OnPlayerRespawn != null)
			OnPlayerRespawn(m_playerController/*, m_playerIndex* /);
	}
//* /
}

//*/
