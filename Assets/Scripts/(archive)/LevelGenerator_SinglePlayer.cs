/*
using UnityEngine;

public class LevelGenerator_SinglePlayer : LevelGenerator
{
	[Space]
	[SerializeField] protected GameObject m_playerPrefab;

	protected override void Awake()
	{
		SpawnPlayer();
		// [TODO] Feels hacky (possible race-condition?) to put base.Awake() at the end...
		base.Awake();
	}

	private void SpawnPlayer()
	{
		for (int x = 0; x < m_gridDimension; ++x)
		{
			// [NOTE] This will need changing if allowing rectangular levels!
			for (int y = 0; y < m_gridDimension; ++y)
			{
				Color color = m_mapData.GridLayout.GetPixel(x, y);
				if (color != m_colorSpawnPoint) continue;
				// can return here since only one spawn point for single player,
				// [NOTE] change this if allow spawning randomly from a choice of spawn points!
				PlaceOnGrid
				(
					m_playerSpawnPrefab,
					x, y,
					(m_turnDirection == ETurnDirection.Right) 
						? m_mapData.PlayerSpawnDirectionRight
						: m_mapData.PlayerSpawnDirectionLeft
				);
				return;
			}
		}
	}
}
//*/