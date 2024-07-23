using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
	private const float k_multiplayerGamespaceScreenPercentage = 0.4f;
	private const float k_multiplayerStatsScreenPercentage = 0.2f;

	// [TODO]
	// Too hacky? What if we want to change this?
	// Same concern with if we ever want height and width to be different (i.e. rectangular and not square)
	// [Q]
	// Do we want to keep this, but attach total number of items and time limit to each?
	// --> Create scriptable object!
	// --> Or not bother with this at all and just put that info in the ThemeData...
	private readonly List<int> m_validGridSizes = new List<int>() { 9, 11, 13, 15, 17 };

	[SerializeField] private MapPropertyData m_mapPropertyData;

	[Space]
	[SerializeField] private HUDManager m_hudManager;
	[SerializeField] private Transform m_statsParentP1;					// This is also referenced in HUDManager! What do? ... Would also need to change buttons parent too, right? Maybe not. Test!
	[SerializeField] private Transform m_gameSpaceParent;
	[SerializeField] private Transform m_borderParent;

	// [TODO][Important]
	// Only have one prefab here, but have an array of sprites within m_themeData which is edited
	// (as with SetPrefabs) when spawning each OTController in GenerateMap()
	[SerializeField] private GameObject m_playerControllerPrefab;
	[SerializeField] private GameObject[] m_playerPrefabs;

	[SerializeField] private GameObject m_wallPrefab;
	[SerializeField] private GameObject m_itemPrefab;
	[SerializeField] private GameObject m_exitPrefab;
	[SerializeField] private GameObject m_travelSquarePrefab;			// [TODO] IMPLEMENT!					<<<<<
	//[SerializeField] private GameObject m_specialPrefab;				// [TODO] REMOVE FROM HERE!
	[SerializeField] private GameObject m_bombPrefab;
	//[SerializeField] private GameObject m_chaserPrefab;					// [TODO] Delete? Can just spawn the script?

	[Space]
	[SerializeField] private GameObject m_inputButtonsP1;
	[SerializeField] private GameObject m_inputButtonsP2;
	[SerializeField] private UnityEngine.UI.Button m_buttonMoveForwardP1;
	[SerializeField] private UnityEngine.UI.Button m_buttonTurnP1;
	[SerializeField] private UnityEngine.UI.Button m_buttonMoveForwardP2;
	[SerializeField] private UnityEngine.UI.Button m_buttonTurnP2;

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
		// DELETE
		// LAPTOP TESTING ONLY
		m_borderParent.localScale = new Vector3(1.111f, 1.111f, 1.0f);
		// ^^^ DELETE ^^^

		m_themeData = LevelSelectData.ThemeData;
		m_mapData = LevelSelectData.MapData;
		m_gameMode = LevelSelectData.GameMode;
		m_turnDirection = LevelSelectData.TurnDirection;

		m_isMultiplayer = (LevelEditorData.IsTestingLevel == false && LevelSelectData.IsMultiplayer);

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
			case EGameMode.Travel:
				break;
			case EGameMode.M_Bomb:
				break;
			case EGameMode.M_Chase:
				break;
			case EGameMode.M_Tanks:
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
		// [TODO][Q] Remove this check?
		if (m_mapData.GridLayout.width != m_mapData.GridLayout.height || 
			m_validGridSizes.Contains(m_mapData.GridLayout.width) == false)
		{
			return false;
		}

		m_gridDimension = LevelSelectData.GridDimension;

		// [NOTE][IMPORTANT] 0.2f at the end is because the border is *CURRENTLY* 1/10th the width of the walls (multiplied by two lots of borders, one each side of the screen)
		// [TODO][Q] Make sure we're calculating the difference?? Ask an artist about import settings??
		// Border should always be the same size, regardless of grid dimnsion!
		m_gridSizeMultiplier = (Camera.main.aspect * Camera.main.orthographicSize * 2.0f) / (m_gridDimension + 0.2f);

		m_gameSpaceParent.localScale = m_gridSizeMultiplier * Vector3.one;

		// We have already positioned the gamespace and border if multiplayer mode
		// ... Don't want repeated code in other places! How best do this?
		if (m_isMultiplayer == false)
		{
			float yOffset = -10.0f * ((Screen.currentResolution.height - Screen.safeArea.height) / Screen.currentResolution.height);    // [Q][IMPORTANT] Is this a correct and safe formula for all devices???
			m_gameSpaceParent.localPosition = new Vector3(0.0f, yOffset, 0.0f);
			m_borderParent.localPosition = new Vector3(0.0f, yOffset, 0.0f);
		}

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
						if (m_gameMode == EGameMode.Travel)
							PlaceOnGrid(m_travelSquarePrefab, x, y);
						break;

					case EMapPropertyName.Wall:
						PlaceWall(x, y);
						break;

					case EMapPropertyName.Item:
						if (m_gameMode == EGameMode.Items)
							PlaceOnGrid(m_itemPrefab, x, y);
						else if (m_gameMode == EGameMode.Travel)
							PlaceOnGrid(m_travelSquarePrefab, x, y);
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
						if (m_gameMode == EGameMode.Travel)
							PlaceOnGrid(m_travelSquarePrefab, x, y);
						if (LevelEditorData.IsTestingLevel && LevelEditorData.StartAtSecondSpawnPoint)
							break;
						EFacingDirection spawnDirectionPrimary = (m_turnDirection == ETurnDirection.Right) ? m_mapData.PlayerSpawnDirectionRight[0] : m_mapData.PlayerSpawnDirectionLeft[0];
						GameObject playerControllerPrimary = PlaceOnGrid(m_playerControllerPrefab, x, y, -5, spawnDirectionPrimary);
						playerControllerPrimary.GetComponent<OTController>().SetFacingDirection(spawnDirectionPrimary);
						playerControllerPrimary.GetComponent<OTController>().SetPlayerPrefab(m_playerPrefabs[0]);
						playerControllerPrimary.GetComponent<OTController>().RespawnPlayer();
						if (playerControllerPrimary.GetComponent<OTController>().IsButtonsGameMode)
						{
							m_inputButtonsP1.SetActive(true);
							playerControllerPrimary.GetComponent<OTController>().GetOnMoveForward(out UnityEngine.Events.UnityAction onMoveForward);
							m_buttonMoveForwardP1.onClick.AddListener(onMoveForward);
							playerControllerPrimary.GetComponent<OTController>().GetOnTurn(out UnityEngine.Events.UnityAction onTurn);
							m_buttonTurnP1.onClick.AddListener(onTurn);
						}
						if (m_isMultiplayer)
						{
							playerControllerPrimary.GetComponent<OTController>().SetInputBounds(m_multiplayerBounds[0]);
							if (m_gameMode == EGameMode.M_Bomb)
							{
								// Feels a little hacky?
								GameObject bomb = Instantiate(m_bombPrefab, playerControllerPrimary.GetComponentInChildren<BoxCollider2D>().transform);
								bomb.transform.localPosition = new Vector3(0.0f, 0.0f, -5.0f);
								playerControllerPrimary.GetComponent<OTController>().Stats.HasBomb = true;
							}
							else if (m_gameMode == EGameMode.M_Chase)       // && if it's the first round only! THAT'S SUPER IMPORTANT!
							{
								// TODO
							}
							else if (m_gameMode == EGameMode.M_Tanks)
							{
								// TODO
							}
						}
						break;

					case EMapPropertyName.SpawnPointSecondary:
						if (m_gameMode == EGameMode.Travel)
							PlaceOnGrid(m_travelSquarePrefab, x, y);
						if ((LevelEditorData.IsTestingLevel && LevelEditorData.StartAtSecondSpawnPoint) || m_isMultiplayer)
						{
							EFacingDirection spawnDirectionSecondary = (m_turnDirection == ETurnDirection.Right) ? m_mapData.PlayerSpawnDirectionRight[m_multiplayerSpawnIndex] : m_mapData.PlayerSpawnDirectionLeft[m_multiplayerSpawnIndex];
							GameObject playerControllerSecondary = PlaceOnGrid(m_playerControllerPrefab, x, y, -5, spawnDirectionSecondary);
							playerControllerSecondary.GetComponent<OTController>().SetFacingDirection(spawnDirectionSecondary);
							playerControllerSecondary.GetComponent<OTController>().SetPlayerPrefab(m_playerPrefabs[m_multiplayerSpawnIndex]);
							playerControllerSecondary.GetComponent<OTController>().RespawnPlayer();
							if (m_isMultiplayer)
							{
								playerControllerSecondary.GetComponent<OTController>().Index = m_multiplayerSpawnIndex;
								playerControllerSecondary.GetComponent<OTController>().SetInputBounds(m_multiplayerBounds[m_multiplayerSpawnIndex]);
								m_multiplayerSpawnIndex++;
								if (playerControllerSecondary.GetComponent<OTController>().IsButtonsGameMode)
								{
									m_inputButtonsP2.SetActive(true);
									playerControllerSecondary.GetComponent<OTController>().GetOnMoveForward(out UnityEngine.Events.UnityAction onMoveForward);
									m_buttonMoveForwardP2.onClick.AddListener(onMoveForward);
									playerControllerSecondary.GetComponent<OTController>().GetOnTurn(out UnityEngine.Events.UnityAction onTurn);
									m_buttonTurnP2.onClick.AddListener(onTurn);
								}
								else if (m_gameMode == EGameMode.M_Chase)		// && if it's the second round! THAT'S SUPER IMPORTANT!
								{
									// TODO
								}
								else if (m_gameMode == EGameMode.M_Tanks)
								{
									// TODO
								}
							}
							else
							{
								// We are testing in the level editor so only need the first set of buttons
								if (playerControllerSecondary.GetComponent<OTController>().IsButtonsGameMode)
								{
									m_inputButtonsP1.SetActive(true);
									playerControllerSecondary.GetComponent<OTController>().GetOnMoveForward(out UnityEngine.Events.UnityAction onMoveForward);
									m_buttonMoveForwardP1.onClick.AddListener(onMoveForward);
									playerControllerSecondary.GetComponent<OTController>().GetOnTurn(out UnityEngine.Events.UnityAction onTurn);
									m_buttonTurnP1.onClick.AddListener(onTurn);
								}
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

	private GameObject PlaceOnGrid(GameObject objectToPlace, int posX, int posY, int posZ, EFacingDirection direction)
	{
		// 0.6f below because of 0.5f * of our wall width plus our border width 0.1f
		GameObject placedObject = Instantiate(objectToPlace, Vector2.zero, Quaternion.identity, m_gameSpaceParent);
		Vector3 position = new Vector3(posX + 0.6f, posY + 0.6f, posZ);
		placedObject.transform.localPosition = position;

		int zRotation = 0;
		switch (direction)
		{
			case EFacingDirection.Right:
				zRotation = -90; break;
			case EFacingDirection.Down:
				zRotation = 180; break;
			case EFacingDirection.Left:
				zRotation = 90; break;
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
				// [TODO][IMPORTANT][Q] Consider making GameplayManager_MItems (child)						<<<<< <<<<< <<<<< <<<<< <<<<<
				case EGameMode.Items:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_Items>();
					break;
				case EGameMode.Exit:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_Exit>();
					break;
				case EGameMode.Travel:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_Travel>();
					break;
				case EGameMode.M_Bomb:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_MBomb>();
					break;
				case EGameMode.M_Chase:
					gameplayManager = gmGameObject.AddComponent<GameplayManager_MChase>();
					break;
				case EGameMode.M_Tanks:
					//gameplayManager = gmGameObject.AddComponent<GameplayManager_MTanks>();				// Create GameMode_MTanks
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
		/*
		// [TODO] If this works, rename the vars... Why does it work like this?
		Vector3 screenCentreY = new Vector3(0.0f, Screen.height * 0.5f, 0.0f);
		Vector3 worldPointCentreY = Camera.main.ScreenToWorldPoint(screenCentreY) * (Camera.main.aspect * 0.5f);
		Vector3 worldPointCentreY2 = Camera.main.ViewportToWorldPoint(screenCentreY) * (Camera.main.aspect * 0.5f);
		m_gameSpaceParent.position = new Vector3(0.0f, -worldPointCentreY.y, 0.0f);
		m_borderParent.position = new Vector3(0.0f, -worldPointCentreY.y, 0.0f);
		//*/

		// [TODO] Figure out how to get '5' from Camera or Screen etc.
		m_gameSpaceParent.localPosition = new Vector3(0.05f, -5 * k_multiplayerGamespaceScreenPercentage, 0.0f);
		m_borderParent.localPosition = new Vector3(0.05f, -5 * k_multiplayerGamespaceScreenPercentage, 0.0f);

		m_statsParentP1.localPosition = new Vector3(0.05f, -1080 * k_multiplayerStatsScreenPercentage, 0.0f);		// IMPORTANT! Note this is UI! Will want something different!
	}
}
