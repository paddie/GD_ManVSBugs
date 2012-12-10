using UnityEngine;
using System.Collections;


public class BugTargets : MonoBehaviour {
	
	public GameObject target1 = null;
	public GameObject target2 = null;
	public GameObject target3 = null;
	
	public Transform currentTarget = null;
	
	// Use this for initialization
	void Start () {
		InvokeRepeating("retarget",0.0f,15.0f);
	}
	
	void retarget() {
		float prob = Random.Range(0.0f,100.0f);
		if ( prob < 33.0f ) {
			GetComponent<Selectable>().SetTargetBug(this.target1.transform);
			return;
		}
		if ( prob >= 33.0f && prob <= 66.0f ) {
			GetComponent<Selectable>().SetTargetBug(this.target2.transform);
			return;
		}
		if ( prob > 66.0f ) {
			GetComponent<Selectable>().SetTargetBug(this.target3.transform);
			return;
		}
		if (GetComponent<DropZone>().spawnTimer > 0.5f)
		GetComponent<DropZone>().spawnTimer -= 0.4f;
	}
	// Update is called once per frame
	
}
