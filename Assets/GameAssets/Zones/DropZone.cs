using UnityEngine;
using System.Collections;

public class DropZone : MonoBehaviour {

	public float spawnTimer = 10.0f;
	public Transform Unit;
	//var Target : Transform;

	public void Start () {
		InvokeRepeating("SpawnUnits", 0.0f, spawnTimer);
	}
	
	private void SpawnUnits() {
		Transform unit;
		//Debug.LogError("spawning Unit: " + Unit.name);
		if ( Unit.name == "Trooper" ) {
			if ( gameObject.GetComponent<Selectable>().PermissionToBoard(1) == 1 ) {
				//Debug.LogError("SPAWNING!");
				//Debug.Log ("Scanning - Process took "+(lastScanTime*1000).ToString ("0")+" ms to complete ");
				unit = (Transform) Instantiate(Unit, 
					new Vector3(transform.position.x, transform.position.y+1, transform.position.z),
					transform.rotation);
				//gameObject.GetComponent(Selectable).unitEntered(unit.gameObject);
			}
		} else {
			unit = (Transform) Instantiate(Unit, 
					new Vector3(transform.position.x, transform.position.y+1, transform.position.z),
					transform.rotation);
			//gameObject.GetComponent<Selectable>().unitEntered(unit2.transform);
		}
	}
	
}
