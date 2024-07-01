using System;
using UnityEngine;

public static class PlayerPrefsSystem
{
	#region Vars
	private const string MULTIPLAYER_WINS_P1_KEY = "multiplayer_wins_p1";
	private const string MULTIPLAYER_WINS_P2_KEY = "multiplayer_wins_p2";
	private const string MULTIPLAYER_DRAWS_KEY = "multiplayer_draws";
	private const string MULTIPLAYER_SCORE_P1_KEY = "multiplayer_score_p1";
	private const string MULTIPLAYER_SCORE_P2_KEY = "multiplayer_score_p2";
	
	private const string CHEAT_TYPE_LIVES_KEY = "cheat_type_lives";
	private const string CHEAT_TYPE_TIME_KEY = "cheat_type_time";
	private const string CHEAT_TYPE_EXIT_KEY = "cheat_type_exit";
	private const string CHEAT_TYPE_WALLS_KEY = "cheat_type_walls";
	private const string CHEAT_TYPE_SPAWN_KEY = "cheat_type_spawn";
	private const string CHEAT_TYPE_VFX_KEY = "cheat_type_vfx";
	#endregion


	#region INIT
	public static void InitAllPrefs()
	{
		PlayerPrefs.SetInt(MULTIPLAYER_WINS_P1_KEY, 0);
		PlayerPrefs.SetInt(MULTIPLAYER_WINS_P2_KEY, 0);
		PlayerPrefs.SetInt(MULTIPLAYER_DRAWS_KEY, 0);
		PlayerPrefs.SetInt(MULTIPLAYER_SCORE_P1_KEY, 0);
		PlayerPrefs.SetInt(MULTIPLAYER_SCORE_P2_KEY, 0);

		PlayerPrefs.SetInt(CHEAT_TYPE_LIVES_KEY, 0);
		PlayerPrefs.SetInt(CHEAT_TYPE_TIME_KEY, 0);
		PlayerPrefs.SetInt(CHEAT_TYPE_EXIT_KEY, 0);
		PlayerPrefs.SetInt(CHEAT_TYPE_WALLS_KEY, 0);
		PlayerPrefs.SetInt(CHEAT_TYPE_SPAWN_KEY, 0);
		PlayerPrefs.SetInt(CHEAT_TYPE_VFX_KEY, 0);
	}
	#endregion


	#region Multiplayer SET
	public static void MultiplayerAddWinP1() { int wins = PlayerPrefs.GetInt(MULTIPLAYER_WINS_P1_KEY) + 1; PlayerPrefs.SetInt(MULTIPLAYER_WINS_P1_KEY, wins); }
	public static void MultiplayerAddWinP2() { int wins = PlayerPrefs.GetInt(MULTIPLAYER_WINS_P2_KEY) + 1; PlayerPrefs.SetInt(MULTIPLAYER_WINS_P2_KEY, wins); }
	public static void MultiplayerAddDraw() { int draws = PlayerPrefs.GetInt(MULTIPLAYER_DRAWS_KEY) + 1; PlayerPrefs.SetInt(MULTIPLAYER_DRAWS_KEY, draws); }
	public static void MultiplayerAddScoreP1(int recentScore) { int totalScore = PlayerPrefs.GetInt(MULTIPLAYER_SCORE_P1_KEY) + recentScore; PlayerPrefs.SetInt(MULTIPLAYER_SCORE_P1_KEY, totalScore); }
	public static void MultiplayerAddScoreP2(int recentScore) { int totalScore = PlayerPrefs.GetInt(MULTIPLAYER_SCORE_P2_KEY) + recentScore; PlayerPrefs.SetInt(MULTIPLAYER_SCORE_P2_KEY, totalScore); }
	#endregion


	#region Multiplayer GET
	public static int MultiplayerGetWinsP1() => PlayerPrefs.GetInt(MULTIPLAYER_WINS_P1_KEY);
	public static int MultiplayerGetWinsP2() => PlayerPrefs.GetInt(MULTIPLAYER_WINS_P2_KEY);
	public static int MultiplayerGetDraws() => PlayerPrefs.GetInt(MULTIPLAYER_DRAWS_KEY);
	public static int MultiplayerGetScoreP1() => PlayerPrefs.GetInt(MULTIPLAYER_SCORE_P1_KEY);
	public static int MultiplayerGetScoreP2() => PlayerPrefs.GetInt(MULTIPLAYER_SCORE_P2_KEY);
	#endregion


	#region Cheats Enabled (GET/SET)
	public static void SetCheatEnabled(ECheatType cheatType, bool enabled)
	{
		switch (cheatType)
		{
			case ECheatType.UnlimitedLives:
				PlayerPrefs.SetInt(CHEAT_TYPE_LIVES_KEY, (enabled) ? 1 : 0);
				break;
			case ECheatType.UnlimitedTime:
				PlayerPrefs.SetInt(CHEAT_TYPE_TIME_KEY, (enabled) ? 1 : 0);
				break;
			case ECheatType.ExitAnyDirection:
				PlayerPrefs.SetInt(CHEAT_TYPE_EXIT_KEY, (enabled) ? 1 : 0);
				break;
			case ECheatType.WallTravel:
				PlayerPrefs.SetInt(CHEAT_TYPE_WALLS_KEY, (enabled) ? 1 : 0);
				break;
			case ECheatType.SecondarySpawn:
				PlayerPrefs.SetInt(CHEAT_TYPE_SPAWN_KEY, (enabled) ? 1 : 0);
				break;
			case ECheatType.PlayerVFX:
				PlayerPrefs.SetInt(CHEAT_TYPE_VFX_KEY, (enabled) ? 1 : 0);
				break;
			default:
				break;
		}
	}

	public static ECheatType[] GetCheatsEnabled()
	{
		ECheatType[] cheatsEnabled = new ECheatType[0];
		for (int i = 0; i < Enum.GetValues(typeof(ECheatType)).Length; ++i)
		{
			ECheatType cheatType = (ECheatType)i;
			switch (cheatType)
			{
				case ECheatType.UnlimitedLives:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_LIVES_KEY) == 1)	cheatsEnabled.Add(cheatType);	break;
				case ECheatType.UnlimitedTime:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_TIME_KEY) == 1)	cheatsEnabled.Add(cheatType);	break;
				case ECheatType.ExitAnyDirection:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_EXIT_KEY) == 1)	cheatsEnabled.Add(cheatType);	break;
				case ECheatType.WallTravel:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_WALLS_KEY) == 1)	cheatsEnabled.Add(cheatType);	break;
				case ECheatType.SecondarySpawn:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_SPAWN_KEY) == 1)	cheatsEnabled.Add(cheatType);	break;
				case ECheatType.PlayerVFX:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_VFX_KEY) == 1)	cheatsEnabled.Add(cheatType);	break;
				default:
					break;
			}
		}
		return cheatsEnabled;
	}
	#endregion


	#region Cheat Checks
	public static bool AnyCheatEnabled()
	{
		for (int i = 0; i < Enum.GetValues(typeof(ECheatType)).Length; ++i)
		{
			ECheatType cheatType = (ECheatType)i;
			switch (cheatType)
			{
				case ECheatType.UnlimitedLives:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_LIVES_KEY) == 1)	return true;
					break;
				case ECheatType.UnlimitedTime:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_TIME_KEY) == 1)	return true;
					break;
				case ECheatType.ExitAnyDirection:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_EXIT_KEY) == 1)	return true;
					break;
				case ECheatType.WallTravel:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_WALLS_KEY) == 1)	return true;
					break;
				case ECheatType.SecondarySpawn:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_SPAWN_KEY) == 1)	return true;
					break;
				case ECheatType.PlayerVFX:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_VFX_KEY) == 1)	return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static bool ScoreDisablingCheatsEnabled()
	{
		for (int i = 0; i < Enum.GetValues(typeof(ECheatType)).Length; ++i)
		{
			ECheatType cheatType = (ECheatType)i;
			switch (cheatType)
			{
				case ECheatType.UnlimitedLives:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_LIVES_KEY) == 1)	return true;
					break;
				case ECheatType.UnlimitedTime:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_TIME_KEY) == 1)	return true;
					break;
				case ECheatType.ExitAnyDirection:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_EXIT_KEY) == 1)	return true;
					break;
				case ECheatType.WallTravel:
					if (PlayerPrefs.GetInt(CHEAT_TYPE_WALLS_KEY) == 1)	return true;
					break;
				default:
					// Not concerned with secondary spawn or player vfx cheats
					break;
			}
		}
		return false;
	}
	#endregion
}
