using UnityEngine;
using System.Collections;



public class GameState : MonoBehaviour {
	
	public int BugCount = 0;
	public int TrooperCount = 0;
	
	public int TrooperKills = 0;
	public int BugKills = 0;
	
	public int BugZones = 0;
	public int TrooperZones = 0;
	
	public bool TroopersWon = false;
	public bool BugsWon = false;
	
	public bool gameOver = false;
	
	public int currentTime;
	
	
	private readonly object statLock = new object();

	// Use this for initialization
	
	string currentLevel;
	
	public int guiMinutes;
	public int guiSeconds;
	
	void Start () {
		GameObject[] gos = GameObject.FindGameObjectsWithTag("DropZone");
		foreach ( GameObject go in gos ) {
			if ( go.name == "BUGzone" ) this.BugZones += 1;
			else if ( go.name == "MANzone" ) this.TrooperZones += 1;
		}
		currentLevel = Application.loadedLevelName;
		Debug.LogError("Level name: " + Application.loadedLevelName);
		
		currentTime = 0;
		
		InvokeRepeating("Tick", 0.0f, 1.0f);
		InvokeRepeating("CheckWinState", 5.0f, 0.5f);
		
				
	}
	
	private void Tick() {
		currentTime += 1;
		guiMinutes = currentTime / 60;
		guiSeconds = currentTime % 60;
	}
	
//	1: Win
//	2: Win without losing X amounts of soldiers
//	3: Kill X amounts of bugs
//	4: Survive for X amount of time
//	5: Indtag Bugs baser og dræb BOSS-BUG.
	void CheckWinState() {
		if ( gameOver ) return;
		
		switch (currentLevel)
		{
		    case "Level1": 
		        if ( Level1() ) PlayerWon();
		        break;
		    case "Level2":
		        if ( Level2() ) PlayerWon();
		        break;
		    case "Level3":
		        if ( Level3() ) PlayerWon();
		        break;
			case "Level4":
				if ( Level4() ) PlayerWon();
		        break;
			case "Level5":
				if ( Level4() ) PlayerWon();
		        break;
			case "Main":
				if ( Level1() ) PlayerWon();
		        break;
		}
		
	}
	
	//	1: Win
	bool Level1() {
		if (this.BugZones == 0 /*&& this.BugCount <= 0 */) {
			return true;
		} else if ( this.TrooperZones == 0 /*&& this.TrooperCount <= 0 */) {
			PlayerLost();
			return false;
		}
		
		
		return false;
	}
	
	//	2: Win without losing X amounts of soldiers
	bool Level2() {
		
		if ( this.BugKills > 25 ) {
			PlayerLost();
			return false;
		}
		
		if (this.BugZones == 0 /*&& this.BugCount <= 0 */) {
			return true;
		} else if ( this.TrooperZones == 0 /*&& this.TrooperCount <= 0 */) {
			PlayerLost();
			return false;
		}
		
		return false;
	}
	
	//	3: Kill X amounts of bugs
	bool Level3() {
		
		if ( this.TrooperKills >= 50 ) {
			return true;
		}
		
		if (this.BugZones == 0 || this.TrooperZones == 0) {
			PlayerLost();
			return false;
		}
		
		return false;
	}
	
	public int Level4SurvivalTime = 180;
	
	//	4: Survive for X amount of time
	bool Level4() {
		
		if ( currentTime >= Level4SurvivalTime ) return true;
		
		if (this.BugZones == 0 /*&& this.BugCount <= 0 */) {
			return true;
		} else if ( this.TrooperZones == 0 /*&& this.TrooperCount <= 0 */) {
			PlayerLost();
			return false;
		}
		
		return false;
	}
	
	void PlayerWon() {
		Debug.LogError("Player won!");
		this.gameOver = true;
		this.TroopersWon = true;
	}
	
	void PlayerLost() {
		Debug.LogError("Player lost!");
		this.gameOver = true;
		this.BugsWon = true;
	}
	
	
		// Update is called once per frame
//	void Update () {
//		//if ( this.TroopersWon || this.BugsWon ) return;
//		
////		lock ( statLock ) {
////			if (this.BugZones == 0 /*&& this.BugCount <= 0 */) {
////				Debug.LogError("Troopers Won!");
////				this.TroopersWon = true;
////				this.gameOver = true;
////			} else if ( this.TrooperZones == 0 /*&& this.TrooperCount <= 0 */) {
////				Debug.LogError("Bugs Won!");
////				this.BugsWon = true;
////				this.gameOver = true;
////			}
////		}
//	}
	
	public void DropZoneConquor(string tag) {
		lock ( statLock ) {
			if ( tag == "Bug" )	{ 
				this.TrooperZones -= 1;
				this.BugZones += 1;
			} else {
				this.TrooperZones += 1;
				this.BugZones -= 1;
			}
				
		}
	}
	
	public void UnitDied(string tag) {
		lock ( statLock ) {	
			if ( tag == "Bug" )	{ 
				this.BugCount -= 1;
				this.TrooperKills += 1;
			} else {
				this.TrooperCount -= 1;
				this.BugKills += 1;
			}
		}
	}
	
	public void UnitSpawned(string tag) {
		if ( tag == "Bug" )	this.BugCount += 1;
		else this.TrooperCount += 1;
	}
	

}
