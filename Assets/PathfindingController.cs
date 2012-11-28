using UnityEngine;
using System.Collections;

public class PathfindingController : MonoBehaviour {
	
	GraphUpdateScene graphUpdater;

	// Use this for initialization
	void Start () {
		graphUpdater = this.GetComponent<GraphUpdateScene>();
		InvokeRepeating("updateGraphcs", 0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void updateGraphcs() {
		graphUpdater.Apply ();
	}
}
