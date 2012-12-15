using UnityEngine;
using System.Collections;

public class DropZone : MonoBehaviour {

	public float spawnTimer = 10.0f;
	public Transform Trooper;
	public Transform Bug;
	public Transform Unit;
	private int spawnCount = 0;
	public Vector3 displace = Vector3.zero;
	public bool isBlocked = false;
	//var Target : Transform;

	public void Start () {
		InvokeRepeating("SpawnUnits", 1.0f, spawnTimer);
	}
	
	public void Update() {
		//bool tmp = isBlocked;
		string unit_dom = GetComponent<Selectable>().GetDominantUnit();
		if ( unit_dom != Unit.tag && !isBlocked ) {
			isBlocked = true;
			GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneConquor(unit_dom);
		} else if ( unit_dom == Unit.tag && isBlocked ) {
			isBlocked = false;	
			GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneConquor(Unit.tag);
		}
	}
	
//	public void UpdateState() {
//				
//	}
//	
//	public void UnBlock(string unit) {
//		// if an enemy unit inters the dropzone
//		// (determined by the type of unit.tag they produce)
//		this.isBlocked = false;
//		GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneConquor(unit);
//	}
//	
//	public void Block(string unit) {
//		// if an enemy unit inters the dropzone
//		// (determined by the type of unit.tag they produce)
//		this.isBlocked = true;
//		GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneConquor(unit);
//	}
	
//	public void TakeOver( string unit ) {
//		
//		if ( unit == "" ) {
//			this.isBlocked = false;
//		}
//		
//		if ( unit == Unit.tag ) {
//			this.isBlocked = false;	
//		} else {
//			this.isBlocked = true;
//		}	
//		GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneConquor(unit);
//	}
	
	// unit dies while docked at node
//	public void Neutral() {
//		
//		this.isBlocked = false;	
//		
//		GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneRelease(Unit.tag);
//	}
	
	
	private void SpawnUnits() {
		
		if ( this.isBlocked ) return;
		
		Transform unit;
		
		if ( GameObject.Find("GameLogic").GetComponent<GameState>().gameOver ) return;
		//Debug.LogError("spawning Unit: " + Unit.name);
		if ( GetComponent<Selectable>().PermissionToBoard(1, Unit.tag) == 1 ) {
			//Debug.LogError("SPAWNING!");
			//Debug.Log ("Scanning - Process took "+(lastScanTime*1000).ToString ("0")+" ms to complete ");
			unit = (Transform) Instantiate(Unit,
				new Vector3(transform.position.x+displace.x, transform.position.y+Unit.localScale.y+displace.y, transform.position.z+displace.z),
				transform.rotation);
			this.spawnCount += 1;
			GameObject.Find("GameLogic").GetComponent<GameState>().UnitSpawned(unit.tag);
			unit.GetComponent<UnitLogic>().id = this.spawnCount;
			unit.GetComponent<AIPath>().target = this.transform;
		}
	}
	
}
