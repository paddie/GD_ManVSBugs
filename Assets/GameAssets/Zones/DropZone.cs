using UnityEngine;
using System.Collections;

public class DropZone : MonoBehaviour {

	public float spawnTimer = 10.0f;
	public Transform Trooper;
	public Transform Bug;
	public Transform Unit;
	private int spawnCount = 0;
	public Vector3 displace = Vector3.zero;
	//var Target : Transform;

	public void Start () {
		InvokeRepeating("SpawnUnits", 1.0f, spawnTimer);
	}
	
	public void SetSpawnUnits(string type) {
		if (Unit.tag == type) return;
		
		if ( type  == "Trooper") {
			Unit = Trooper;
		} else {
			Unit = Bug;	
		}
		
		GameObject.Find("GameLogic").GetComponent<GameState>().DropZoneConquor(type);
		//return Unit.tag;
	}
	
	private void SpawnUnits() {
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
