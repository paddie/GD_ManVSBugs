using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	Rigidbody rigidBody;
	Vector3 move = Vector3.zero;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {
		rigidBody.velocity = move;
	}
	
	public void setVelocity(Vector3 v) {
		move = v;	
	}
}