using UnityEngine;
using UnityEngine.SceneManagement;
//using System.Collections;

public class LevelManager : MonoBehaviour {

	public string nextSplashLevel;
	public float autoLoadNextLevelAfter;
	public static string levelType;

	private string levelName;
	//private int normalLevelsUnlocked, specialLevelsUnlocked;



	void Start() {
		levelName = SceneManager.GetActiveScene().name;
		if (levelName == "00 Splash") { Invoke("LoadNextLevel", autoLoadNextLevelAfter); }
		//if (levelName == "01 Menu") { PlayerPrefsManager.SetMultiWins(0, 0); }
	}

	/*
	public void LinkFacebook () {
		Application.OpenURL("http://www.facebook.com/");
	}
		
	public void LinkTwitter () {
		Application.OpenURL("http://www.twitter.com/");
	}

	public void LinkWeb () {
		Application.OpenURL("http://www.auburnzone.co.uk/");
	}
	//*/

	public void LoadLevel (string name) {
		SceneManager.LoadScene(name);
	}

	/*
	public void ResetLevel () {
		LoadPuzzleLevel();
	}

	public static void LoadPuzzleLevel () {
		string levelType = PlayerPrefsManager.GetLevelType();
		if (levelType == "garden" || levelType == "garden M") {
			StatsManager.SetNumberOfItems(8);
			MultiItemDestroyer.itemsLeft = 8;
			if (levelType == "garden") { SceneManager.LoadScene("10 Garden"); }
			else { SceneManager.LoadScene("10 Garden M"); }
		} else if (levelType == "office" || levelType == "office M") {
			StatsManager.SetNumberOfItems(8);
			MultiItemDestroyer.itemsLeft = 8;
			if (levelType == "office") { SceneManager.LoadScene("11 Office"); }
			else { SceneManager.LoadScene("11 Office M"); }
		} else if (levelType == "airport" || levelType == "airport M") {
			StatsManager.SetNumberOfItems(12);
			MultiItemDestroyer.itemsLeft = 12;
			if (levelType == "airport") { SceneManager.LoadScene("20 Airport"); }
			else { SceneManager.LoadScene("20 Airport M"); }
		} else if (levelType == "construction" || levelType == "construction M") {
			StatsManager.SetNumberOfItems(12);
			MultiItemDestroyer.itemsLeft = 12;
			if (levelType == "construction") { SceneManager.LoadScene("21 Construction"); }
			else { SceneManager.LoadScene("21 Construction M"); }
		} else if (levelType == "farm" || levelType == "farm M") {
			StatsManager.SetNumberOfItems(12);
			MultiItemDestroyer.itemsLeft = 12;
			if (levelType == "farm") { SceneManager.LoadScene("22 Farm"); }
			else { SceneManager.LoadScene("22 Farm M"); }
		} else if (levelType == "island" || levelType == "island M") {
			StatsManager.SetNumberOfItems(17);
			MultiItemDestroyer.itemsLeft = 17;
			if (levelType == "island") { SceneManager.LoadScene("30 Island"); }
			else { SceneManager.LoadScene("30 Island M"); }
		} else if (levelType == "disco" || levelType == "disco M") {
			StatsManager.SetNumberOfItems(17);
			MultiItemDestroyer.itemsLeft = 17;
			if (levelType == "disco") { SceneManager.LoadScene("31 Disco"); }
			else { SceneManager.LoadScene("31 Disco M"); }
		} else if (levelType == "playground" || levelType == "playground M") {
			StatsManager.SetNumberOfItems(17);
			MultiItemDestroyer.itemsLeft = 17;
			if (levelType == "playground") { SceneManager.LoadScene("32 Playground"); }
			else { SceneManager.LoadScene("32 Playground M"); }
		} else if (levelType == "arctic" || levelType == "arctic M") {
			StatsManager.SetNumberOfItems(24);
			MultiItemDestroyer.itemsLeft = 24;
			if (levelType == "arctic") { SceneManager.LoadScene("40 Arctic"); }
			else { SceneManager.LoadScene("40 Arctic M"); }
		} else if (levelType == "city" || levelType == "city M") {
			StatsManager.SetNumberOfItems(24);
			MultiItemDestroyer.itemsLeft = 24;
			if (levelType == "city") { SceneManager.LoadScene("41 City"); }
			else { SceneManager.LoadScene("41 City M"); }
		} else if (levelType == "funfair" || levelType == "funfair M") {
			StatsManager.SetNumberOfItems(24);
			MultiItemDestroyer.itemsLeft = 24;
			if (levelType == "funfair") { SceneManager.LoadScene("42 Funfair"); }
			else { SceneManager.LoadScene("42 Funfair M"); }
		} else if (levelType == "kitchen" || levelType == "kitchen M") {
			StatsManager.SetNumberOfItems(24);
			MultiItemDestroyer.itemsLeft = 24;
			if (levelType == "kitchen") { SceneManager.LoadScene("43 Kitchen"); }
			else { SceneManager.LoadScene("43 Kitchen M"); }
		} else if (levelType == "stadium" || levelType == "stadium M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "stadium") { SceneManager.LoadScene("S10 Stadium"); }
			else { SceneManager.LoadScene("S10 Stadium M"); }
		} else if (levelType == "space" || levelType == "space M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "space") { SceneManager.LoadScene("S11 Space"); }
			else { SceneManager.LoadScene("S11 Space M"); }
		} else if (levelType == "cave" || levelType == "cave M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "cave") { SceneManager.LoadScene("S12 Cave"); }
			else { SceneManager.LoadScene("S12 Cave M"); }
		} else if (levelType == "warehouse" || levelType == "warehouse M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "warehouse") { SceneManager.LoadScene("S13 Warehouse"); }
			else { SceneManager.LoadScene("S13 Warehouse M"); }
		} else if (levelType == "motorway" || levelType == "motorway M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "motorway") { SceneManager.LoadScene("S14 Motorway"); }
			else { SceneManager.LoadScene("S14 Motorway M"); }
		} else if (levelType == "racetrack" || levelType == "racetrack M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "racetrack") { SceneManager.LoadScene("S15 Racetrack"); }
			else { SceneManager.LoadScene("S15 Racetrack M"); }
		} else if (levelType == "jungle" || levelType == "jungle M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "jungle") { SceneManager.LoadScene("S16 Jungle"); }
			else { SceneManager.LoadScene("S16 Jungle M"); }
		} else if (levelType == "temple" || levelType == "temple M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "temple") { SceneManager.LoadScene("S17 Temple"); }
			else { SceneManager.LoadScene("S17 Temple M"); }
		} else if (levelType == "cyber" || levelType == "cyber M") {
			StatsManager.SetNumberOfItems(40);
			MultiItemDestroyer.itemsLeft = 40;
			if (levelType == "cyber") { SceneManager.LoadScene("S18 Cyber"); }
			else { SceneManager.LoadScene("S18 Cyber M"); }
		} else if (levelType == "alien" || levelType == "alien M") {
			StatsManager.SetNumberOfItems(32);
			MultiItemDestroyer.itemsLeft = 32;
			if (levelType == "alien") { SceneManager.LoadScene("S19 Alien"); }
			else { SceneManager.LoadScene("S19 Alien M"); }
		} else if (levelType == "big") {
			StatsManager.SetNumberOfItems(64);
			MultiItemDestroyer.itemsLeft = 64;
			SceneManager.LoadScene("S20 Big");
		}
	}
	//*/

	public void QuitRequest() {
		Application.Quit();
	}

	public void LoadNextLevel () {
		SceneManager.LoadScene(nextSplashLevel);
	}

	/*
	public void PauseButton () {
		Timer.PauseButton();
	}

	public void LeaveLevelButton (string player) {
		if (player == "single") {
			SceneManager.LoadScene("03a Single");
		} else {
			SceneManager.LoadScene("04a Multi");
		}
	}

	public void NextLevel (string player) {
		int levelChosen = PlayerPrefsManager.GetLevelNumber();
		if (levelChosen != 5) {
			PlayerPrefsManager.SetLevelNumber(levelChosen + 1);
			LoadPuzzleLevel();
		} else {
			LeaveLevelButton(player);
		}
	}

	public static void LeaveLevel (string winLose, string player) {
		if (player == "single") {
			if (winLose == "win") { SceneManager.LoadScene("99a Win"); }
			else { SceneManager.LoadScene("99b Lose"); }
		} else {
			if (winLose == "one") {
				MultiStatWriter.winner = "one";
				PlayerPrefsManager.SetMultiWins(PlayerPrefsManager.GetMultiWinsONE()+1, PlayerPrefsManager.GetMultiWinsTWO());
			}
			else if (winLose == "two") {
				MultiStatWriter.winner = "two";
				PlayerPrefsManager.SetMultiWins(PlayerPrefsManager.GetMultiWinsONE(), PlayerPrefsManager.GetMultiWinsTWO()+1);
			}
			else { MultiStatWriter.winner = "draw"; }
			SceneManager.LoadScene("99M End");
		}
	}
	//*/
}
