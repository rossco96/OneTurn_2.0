/*
using UnityEngine;

public class GameTypeGenerator : MonoBehaviour
{
	[SerializeField] private HUDManager m_hudManager;
	[SerializeField] private GameObject m_levelGeneratorSinglePrefab;
	[SerializeField] private GameObject m_levelGeneratorMultiPrefab;

	private void Awake()
	{
		InitLevelGenerator();
		//InitGameplayManager();									// DELETE ???
	}

	private void InitLevelGenerator()
	{
		if (LevelSelectData.IsMultiplayer)
			Instantiate(m_levelGeneratorMultiPrefab);
		else
			Instantiate(m_levelGeneratorSinglePrefab);
		// [TODO][DELETE]
		/*
		GameObject levelGenerator = new GameObject("LevelGenerator");
		if (LevelSelectData.IsMultiplayer)
			levelGenerator.AddComponent<LevelGenerator_Multiplayer>();
		else
			levelGenerator.AddComponent<LevelGenerator_SinglePlayer>();
		//* /
	}

	private void InitGameplayManager()
	{
		GameObject gmGameObject = new GameObject("GameplayManager");
		GameplayManager gameplayManager = default;
		switch (LevelSelectData.GameMode)
		{
			case EGameMode.Items:
				gameplayManager = gmGameObject.AddComponent<GameplayManager_Items>();
				break;
			case EGameMode.Exit:
				gameplayManager = gmGameObject.AddComponent<GameplayManager_Exit>();
				break;
			case EGameMode.Bomb:
				break;
			case EGameMode.Chase:
				break;
			default:
				break;
		}
		gameplayManager.SetHUDManager(m_hudManager);
	}
}
//*/