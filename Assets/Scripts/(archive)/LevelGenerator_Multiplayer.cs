/*
using UnityEngine;

public class LevelGenerator_Multiplayer : LevelGenerator
{
	// [TODO]
	// Have m_numberOfPlayers to allow up to four players???

	[Space]
	[SerializeField] private GameObject[] m_playerPrefabs;
	
	private GameObject m_gameSpaceParent;

	protected override void Awake()
	{
		// [TODO] Once again, hacky...
		m_gameSpaceParent = GameObject.Find("GameSpaceParent");
		SpawnPlayers();
		PositionGameSpaceLocalMultiplayer();
		// [TODO] Feels hacky (possible race-condition?) to put base.Awake() at the end...
		base.Awake();
	}

	private void SpawnPlayers()
	{
		for (int x = 0; x < m_gridDimension; ++x)
		{
			// [NOTE] This will need changing if allowing rectangular levels!
			for (int y = 0; y < m_gridDimension; ++y)
			{
				Color color = m_mapData.GridLayout.GetPixel(x, y);
				if (color != m_colorSpawnPoint) continue;
				// [TODO] Implement a return function, so we can get the player spawn prefab back, then assign a player index?
				// OR
				// keep track of how many players have been spawned, and get the index that way from OTController itself?
				// ... might actually be able to merge these scripts back into one
				// (sleep on it)
				PlaceOnGrid(m_playerSpawnPrefab, x, y, (m_turnDirection == ETurnDirection.Right) ? m_mapData.PlayerSpawnDirectionRight : m_mapData.PlayerSpawnDirectionLeft);
			}
		}
	}

	private void PositionGameSpaceLocalMultiplayer()
	{
		float posY = Camera.main.orthographicSize * (Camera.main.aspect - 0.5f);
		m_gameSpaceParent.transform.position = new Vector3(0.0f, posY, 0.0f);
	}
}
//*/