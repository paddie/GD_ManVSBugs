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

	// Use this for initialization
	void Start () {
		GameObject[] gos = GameObject.FindGameObjectsWithTag("DropZone");
		foreach ( GameObject go in gos ) {
			if ( go.name == "BUGzone" ) this.BugZones += 1;
			else if ( go.name == "MANzone" ) this.TrooperZones += 1;
		}
	}
	
		// Update is called once per frame
	void Update () {
		if ( this.TroopersWon || this.BugsWon ) return;
		
		if (this.BugZones == 0 && this.BugCount <= 0) {
			Debug.LogError("Troopers Won!");
			this.TroopersWon = true;
			this.gameOver = true;
		} else if ( this.TrooperZones == 0 && this.TrooperCount <= 0 ) {
			Debug.LogError("Bugs Won!");
			this.BugsWon = true;
			this.gameOver = true;
		}
	}
	
	public void DropZoneConquor(string tag) {
		if ( tag == "Bug" )	{ 
			this.TrooperZones -= 1;
			this.BugZones += 1;
		} else {
			this.TrooperZones += 1;
			this.BugZones -= 1;
		}	
	}
	
	public void UnitDied(string tag) {
		if ( tag == "Bug" )	{ 
			this.BugCount -= 1;
			this.TrooperKills += 1;
		} else {
			this.TrooperCount -= 1;
			this.BugKills += 1;
		}
	}
	
	public void UnitSpawned(string tag) {
		if ( tag == "Bug" )	this.BugCount += 1;
		else this.TrooperCount += 1;
	}
	

}
