using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	Rigidbody rigidBody;
	Vector3 move = Vector3.zero;
	public bool canMove = true;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<AIPath>().enabled && GetComponent<AIPath>().canMove) {
			rigidBody.velocity = move;
		} else {
			rigidBody.velocity = Vector3.zero;
		}
	}
	
	public void setVelocity(Vector3 v) {
		if (v.magnitude < 75)
			move = v;	
	}
}