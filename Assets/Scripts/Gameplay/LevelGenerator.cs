using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
	// Too hacky? What if we want to change this?
	// Same concern with if we ever want height and width to be different (i.e. rectangular and not square)
	private readonly List<int> m_validGridSizes = new List<int>() { 9, 11, 13, 15, 17 };

	[SerializeField] private MapPropertyData m_mapPropertyData;

	[Space]
	[SerializeField] private HUDManager m_hudManager;
	[SerializeField] private Transform m_gameSpaceParent;

	// [TODO][Important]
	// Only have one prefab here, but have an array of sprites within m_themeData which is edited
	// (as with SetPrefabs) when spawning each OTController in GenerateMap()
	[SerializeField] private GameObject m_playerControllerPrefab;
	[SerializeField] private GameObject[] m_playerPrefabs;

	[SerializeField] private GameObject m_wallPrefab;
	[SerializeField] private GameObject m_itemPrefab;
	[SerializeField] private GameObject m_exitPrefab;
	//[SerializeField] private GameObject m_specialPrefab;				// [TODO] REMOVE FROM HERE!

	private bool m_levelEditorOverride = false;
	private ThemeData m_themeData;
	private MapData m_mapData;
	private EGameMode m_gameMode;
	private ETurnDirection m_turnDirection;
	private bool m_isMultiplayer = false;

	private int m_multiplayerSpawnIndex = 1;
	private Bounds[] m_multiplayerBounds;

	private int m_gridDimension = 0;
	private float m_gridSizeMultiplier = 1.0f;



	private void Awake()
	{
		m_levelEditorOverride = LevelEditorData.IsTestingLevel;

		m_themeData = LevelSelectData.ThemeData;
		m_mapData = LevelSelectData.MapData;
		m_gameMode = LevelSelectData.GameMode;
		m_turnDirection = LevelSelectData.TurnDirection;

		m_isMultiplayer = (m_levelEditorOverride == false && LevelSelectData.IsMultiplayer);

		if (m_isMultiplayer)
		{
			InitLocalMultiplayerBounds();
			PositionGameSpaceLocalMultiplayer();
		}

		SetPrefabs();
		if (SetGridInfo() == false)
			return;

		GenerateMap();

		if (m_themeData.IsSpecialLevel)
		{
			// [TODO]
			// Spawn SpecialLevel script!
			// Ensure it spawns the correct prefabs in the correct locations and sets up the function of them
			// (or is the function on the prefab itself? which implementation would be better?)
		}

		InitGameplayManager();
	}



	private void SetPrefabs()
	{
		// [TODO][Q] Allow player selectin their own character sprite? Rather than forcing it for each theme

		//m_playerPrefab.GetComponent<SpriteRenderer>().sprite = m_themeData.PlayerSprite;			// COMMENTED OUT FOR TESTING ONLY
		m_wallPrefab.GetComponent<SpriteRenderer>().sprite = m_themeData.WallSprite;

		switch (m_gameMode)
		{
			case EGameMode.Items:
				m_itemPrefab.GetComponent<SpriteRenderer>().sprite = m_themeData.ItemSprite;
				break;
			case EGameMode.Exit:
				m_exitPrefab.GetComponent<SpriteRenderer>().sprite = m_themeData.ExitSprite;
				break;
			case EGameMode.M_Bomb:
				break;
			case EGameMode.M_Chase:
				break;
			default:
				break;
		}

		// [TODO][IMPORTANT] Do not do this here! Should spawn special script and handle through that
		// e.g. may have more than one type of special sprite - like different bridge components, or two types of portals
		//if (m_themeData.IsSpecialLevel)
		//	m_specialPrefab.GetComponent<SpriteRenderer>().sprite = m_themeData.SpecialSprite;
	}

	private bool SetGridInfo()
	{
		if (m_mapData.GridLayout.width != m_mapData.GridLayout.height || 
			m_validGridSizes.Contains(m_mapData.GridLayout.width) == false)
		{
			return false;
		}

		m_gridDimension = m_mapData.GridLayout.width;
		// [NOTE][IMPORTANT] 0.2f at the end is because the border is *CURRENTLY* 1/10th the width of the walls (multiplied by two lots of borders, one each side of the screen)
		// [TODO][Q] Make sure we're calculating the difference?? Ask an artist about import settings??
		// Border should always be the same size, regardless of grid dimnsion!
		m_gridSizeMultiplier = (Camera.main.aspect * Camera.main.orthographicSize * 2.0f) / (m_gridDimension + 0.2f);

		m_gameSpaceParent.localScale = m_gridSizeMultiplier * Vector3.one;

		return true;
	}


	private void GenerateMap()
	{
		for (int x = 0; x < m_gridDimension; ++x)
		{
			// [NOTE] This will need changing if allowing rectangular levels!
			for (int y = 0; y < m_gridDimension; ++y)
			{
				EMapPropertyName colorName = m_mapPropertyData.GetNameByColor(m_mapData.GridLayout.GetPixel(x, y));
				switch (colorName)
				{
					case EMapPropertyName.BlankSquare:
						// Do nothing
						break;

					case EMapPropertyName.Wall:
						PlaceWall(x, y);
						break;

					case EMapPropertyName.Item:
						if (m_gameMode == EGameMode.Items)
							PlaceOnGrid(m_itemPrefab, x, y);
						break;

					case EMapPropertyName.Exit:
						if (m_gameMode == EGameMode.Exit)
							PlaceOnGrid(m_exitPrefab, x, y, (m_turnDirection == ETurnDirection.Right) ? m_mapData.ExitFacingDirectionRight : m_mapData.ExitFacingDirectionLeft);
						else
							// [NOTE] Ensure when desigining that we're taking this into account!
							// ... Or would it be better to just have it in its own place?
							PlaceWall(x, y);
						break;

					case EMapPropertyName.SpawnPointPrimary:
						if (m_levelEditorOverride && LevelEditorData.StartAtSecondSpawnPoint)
							break;
						GameObject playerControllerPrimary = PlaceOnGrid
						(
							m_playerControllerPrefab, x, y,
							(m_turnDirection == ETurnDirection.Right)
								? m_mapData.PlayerSpawnDirectionRight[0]
								: m_mapData.PlayerSpawnDirectionLeft[0]
						);
						playerControllerPrimary.GetComponent<OTController>().SetPlayerPrefab(m_playerPrefabs[0]);
						if (m_isMultiplayer)
							playerControllerPrimary.GetComponent<OTController>().SetInputBounds(m_multiplayerBounds[0]);
						break;

					case EMapPropertyName.SpawnPointSecondary:
						if ((m_levelEditorOverride && LevelEditorData.StartAtSecondSpawnPoint) || m_isMultiplayer)
						{
							GameObject playerControllerSecondary = PlaceOnGrid
							(
								m_playerControllerPrefab, x, y,
								(m_turnDirection == ETurnDirection.Right)
									? m_mapData.PlayerSpawnDirectionRight[m_multiplayerSpawnIndex]
									: m_mapData.PlayerSpawnDirectionLeft[m_multiplayerSpawnIndex]
							);
							playerControllerSecondary.GetComponent<OTController>().SetPlayerPrefab(m_playerPrefabs[m_multiplayerSpawnIndex]);
							if (m_isMultiplayer)
							{
								playerControllerSecondary.GetComponent<OTController>().SetInputBounds(m_multiplayerBounds[m_multiplayerSpawnIndex]);
								m_multiplayerSpawnIndex++;
							}
						}
						break;

					case EMapPropertyName.Special:
						// [NOTE] DO NOT DEAL WITH IN HERE?
						// Or have reference to the script, if it's a special level, and call Script.Spawn()?
						// ... As in, spawn the script here? And INIT
						break;

					default:
						break;
				}

				// [NOTE][IMPORTANT]
				// For special, doing things such as moving walls which contain routes,
				// or bridges where there are multiple different types of bridge component to place...
				// --> May need separate generator script with separate additional grid texture (think of as an overlay?)
			}
		}
	}

	private void PlaceWall(int posX, int posY)
	{
		// [TODO] Will eventually want to distinguish between 
		// Straight, Corner, T, and Cross pieces...
		// (I, L, T, +)
		// And the correct directions to face them!
		// ... Just implement as plain blocks for now -- prefabs! With colliders on them

		// 0.6f below because of 0.5f * of our wall width plus our border width 0.1f
		GameObject wall = Instantiate(m_wallPrefab, Vector2.zero, Quaternion.identity, m_gameSpaceParent);
		Vector2 position = new Vector2(posX + 0.6f, posY + 0.6f);
		wall.transform.localPosition = position;
	}

	private void PlaceOnGrid(GameObject objectToPlace, int posX, int posY)
	{
		// 0.6f below because of 0.5f * of our wall width plus our border width 0.1f
		GameObject placedObject = Instantiate(objectToPlace, Vector2.zero, Quaternion.identity, m_gameSpaceParent);
		Vector2 position = new Vector2(posX + 0.6f, posY + 0.6f);
		placedObject.transform.localPosition = position;
	}

	// [NOTE] This is now returning a GameObject as we will need to initialise PlayerSpawnPoints once spawned
	private GameObject PlaceOnGrid(GameObject objectToPlace, int posX, int posY, EFacingDirection direction)
	{
		// 0.6f below because of 0.5f * of our wall width plus our border width 0.1f
		GameObject placedObject = Instantiate(objectToPlace, Vector2.zero, Quaternion.identity, m_gameSpaceParent);
		Vector2 position = new Vector2(posX + 0.6f, posY + 0.6f);
		placedObject.transform.localPosition = position;

		int zRotation = 0;
		switch (direction)
		{
			case EFacingDirection.Right:
				zRotation = -90;	break;
			case EFacingDirection.Down:
				zRotation = 180;	break;
			case EFacingDirection.Left:
				zRotation = 90;		break;
			default:
				break;
		}
		placedObject.transform.Rotate(0, 0, zRotation);
		return placedObject;
	}



	private void InitGameplayManager()
	{
		GameObject gmGameObject = new GameObject("GameplayManager");
		GameplayManager gameplayManager = default;
		if (LevelEditorData.IsTestingLevel)
		{
			gameplayManager = gmGameObject.AddComponent<GameplayManager_LevelEditor>();
		}
		else
		{
			switch (m_gameMode)
			{
				case EGameMode.Items:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_Items>();
					break;
				case EGameMode.Exit:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_Exit>();
					break;
				case EGameMode.M_Bomb:
					break;
				case EGameMode.M_Chase:
					break;
				default:
					break;
			}
		}
		gameplayManager.SetHUDManager(m_hudManager);
	}



	private void InitLocalMultiplayerBounds()
	{
		m_multiplayerBounds = new Bounds[2];
		m_multiplayerBounds[0].center = new Vector3(Screen.width * 0.5f, Screen.height * 0.25f, 0.0f);
		m_multiplayerBounds[0].size = new Vector3(Screen.width, Screen.height * 0.5f, 0.0f);
		m_multiplayerBounds[1].center = new Vector3(Screen.width * 0.5f, Screen.height * 0.75f, 0.0f);
		m_multiplayerBounds[1].size = new Vector3(Screen.width, Screen.height * 0.5f, 0.0f);
	}
	
	private void PositionGameSpaceLocalMultiplayer()
	{
		// [TODO] If this works, rename the vars... Why does it work like this?
		Vector3 screenCentreY = new Vector3(0.0f, Screen.height * 0.5f, 0.0f);
		Vector3 worldPointCentreY = Camera.main.ScreenToWorldPoint(screenCentreY) / (Camera.main.aspect * 0.5f);
		m_gameSpaceParent.position = new Vector3(0.0f, -worldPointCentreY.y, 0.0f);
	}
}
